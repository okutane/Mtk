using System;
using System.Linq;
using System.Collections.Generic;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library.FaceFunctions;
using Matveev.Mtk.Library.Fields;
using Matveev.Mtk.Library.Validators;
using Matveev.Mtk.Library.Utilities;

namespace Matveev.Mtk.Library
{
    public static class OptimizeMesh
    {
        public static void ImproveVertexPositions(IEnumerable<Vertex> vertices, IImplicitSurface surface,
            IProgressMonitor monitor)
        {
            IPointsFunctionWithGradient localFunction = GetLocalFunctions(surface);
            ImproveVertexPositions(vertices, localFunction, monitor);
        }

        private static IPointsFunctionWithGradient GetLocalFunctions(IImplicitSurface surface)
        {
            IPointsFunctionWithGradient energyProvider =
                TriangleImplicitApproximations.GetApproximation(surface, "square");
            if (false)
            {
                IPointsFunctionWithGradient preciseEnergyProvider = surface as IPointsFunctionWithGradient;
                if (preciseEnergyProvider != null)
                {
                    energyProvider = preciseEnergyProvider;
                }
            }
            return energyProvider;
        }

        public static void ImproveVertexPositions(IEnumerable<Vertex> vertices, Func<Point[], double> evaluate,
            GradientDelegate<Point, Vector> evaluateGradient, IProgressMonitor monitor)
        {
            ImproveVertexPositions(vertices, new LocalPointsFunctionWithGradient(evaluate, evaluateGradient),
                monitor);
        }

        public static void ImproveVertexPositions(IEnumerable<Vertex> vertices,
            IPointsFunctionWithGradient localFunction, IProgressMonitor monitor)
        {
            Vertex[] verticesArray = vertices.ToArray();

            Func<Vertex, IPointStrategy> pointStrategyProvider = delegate(Vertex vertex)
            {
                int index = Array.IndexOf(verticesArray, vertex);
                if (index >= 0)
                {
                    if (vertex.Type == VertexType.Boundary)
                    {
                        bool[] locks = new bool[3];
                        for (int i = 0; i < 3; i++)
                        {
                            Vector direction = new Vector(0, 0, 0);
                            direction[i] = 1;
                            if (!Configuration.BoundingBox.IsMovable(vertex, direction))
                            {
                                locks[i] = true;
                            }
                        }
                        return new ConstrainedPointStrategy(index, locks);
                    }
                    return new NormalPointStrategy(index);
                }
                return new FixedPointStrategy(vertex.Point);
            };

            IEnumerable<Face> facesCollection = verticesArray.SelectMany(v => v.AdjacentFaces).Distinct();
            IPointStrategy[][] faces =
                facesCollection.Select(f => f.Vertices.Select(pointStrategyProvider).ToArray()).ToArray();

            Point[] buffer = Array.ConvertAll(verticesArray, v => v.Point);

            GlobalPointsFunctionWithGradient globalFunction = new GlobalPointsFunctionWithGradient(localFunction,
                faces);

            UGslMultimin.Optimize(globalFunction, buffer, 1e-7, 100, monitor);

            for (int i = 0; i < verticesArray.Length; i++)
            {
                verticesArray[i].Point = buffer[i];
            }
        }

        public static void ImproveVertexPositions(Mesh mesh, IImplicitSurface surface, IProgressMonitor monitor)
        {
            ImproveVertexPositions(mesh.Vertices, surface, monitor);
        }

        public static void OptimizeImplicit(Mesh mesh, IImplicitSurface field, double epsilon, double alpha,
            IProgressMonitor monitor)
        {
            ICollection<EdgeTransform> transforms = Configuration.EdgeTransforms;

            Dictionary<EdgeTransform, int> numbersOfUses = new Dictionary<EdgeTransform, int>();

            Func<Point[], double> faceEnergy =
                TriangleImplicitApproximations.GetApproximation(field, "square").Evaluate;
            if (field is QuadraticForm)
            {
                faceEnergy = ((QuadraticForm)field).FaceDistance;
            }
            Energy energy = new CompositeEnergy(new VertexEnergy(alpha), new FaceEnergy(faceEnergy));
            BoundingBox constraintsProvider = new BoundingBox(-1, 1, -1, 1, -1, 1);

            List<IMeshValidator> validators = new List<IMeshValidator>();
            validators.Add(new DihedralAnglesValidator(0.5));
            validators.Add(new FaceNormalsValidator());

            //Этап 1. Проецирование всех вершин сетки на поверхность
            ProjectAll(mesh, field, epsilon);

            //Этап 2. Улучшение позиций всех вершин сетки
            ImproveVertexPositions(mesh, field, monitor);

            //Этап 3. Преобразования над структурой сетки
            Random rand = new Random(42);
            bool changed;
            do
            {
                changed = false;
                List<Edge> candidats = new List<Edge>(mesh.Edges);
                Edge candidat, smCandidat;
                Mesh submesh;
                Dictionary<Edge, Edge> edgeMap = new Dictionary<Edge, Edge>();
                double E1, E2;
                while (candidats.Count != 0)
                {
                    candidat = candidats[rand.Next(candidats.Count - 1)];
                    candidats.RemoveAll(edge => edge == candidat || edge == candidat.Pair);

                    IEnumerable<Face> surrounding =
                        candidat.Begin.AdjacentFaces.Concat(candidat.End.AdjacentFaces).Distinct();

                    submesh = mesh.CloneSub(surrounding, null, edgeMap, null);
                    smCandidat = edgeMap[candidat];
                    E1 = energy.Eval(submesh);

                    foreach (EdgeTransform transform in transforms)
                    {
                        if (!transform.IsPossible(candidat, constraintsProvider))
                        {
                            continue;
                        }

                        IDictionary<Edge, Edge> edgeMap2 = new Dictionary<Edge, Edge>();
                        Mesh submesh2 = submesh.Clone(edgeMap2);
                        Edge smCandidat2 = edgeMap2[smCandidat];

                        MeshPart smResult;
                        try
                        {
                            smResult = transform.Execute(smCandidat2);
                        }
                        catch
                        {
                            continue;
                        }

                        if (!validators.TrueForAll(v => v.IsValid(submesh2)))
                        {
                            continue;
                        }

                        ImproveVertexPositions(smResult.GetVertices(0), field, null);

                        E2 = energy.Eval(submesh2);

                        if (E1 <= E2)
                        {
                            continue;
                        }

                        if (numbersOfUses.ContainsKey(transform))
                        {
                            numbersOfUses[transform] = numbersOfUses[transform] + 1;
                        }
                        else
                        {
                            numbersOfUses.Add(transform, 1);
                        }
                        /*foreach (Edge edge in edgeMap.Keys)
                        {
                            candidats.RemoveAll(e => e == edge);
                        }*/
                        candidats.RemoveAll(edgeMap.ContainsKey);

                        MeshPart result = transform.Execute(candidat);

                        ImproveVertexPositions(result.GetVertices(0), field, monitor);
                        candidats.AddRange(result.GetEdges(1));
                        changed = true;
                        break;
                    }
                }
            }
            while (changed);
        }

        public static void ProjectAll(Mesh mesh, IImplicitSurface field, double epsilon)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            foreach (Vertex vert in mesh.Vertices)
            {
                Point p = vert.Point;
                VertexOps.ProjectPointOnSurface(ref p, field, epsilon);
                vert.Point = p;
                vert.Normal = Vector.Normalize(field.Grad(p));
            }
        }

        private class FaceEnergy : Energy
        {
            private readonly Func<Point[], double> _faceEnergy;

            public FaceEnergy(Func<Point[], double> faceEnergy)
            {
                _faceEnergy = faceEnergy;
            }

            public override double Eval(Mesh mesh)
            {
                return mesh.Faces.Sum(face => _faceEnergy(face.Vertices.Select(vertex => vertex.Point).ToArray()));
            }
        }

        private interface IPointStrategy
        {
            Point GetPoint(Point[] points);

            void AddVector(Vector[] result, Vector vector);
        }

        private class NormalPointStrategy : IPointStrategy
        {
            private readonly int _index;

            public NormalPointStrategy(int index)
            {
                _index = index;
            }

            #region IPointStrategy Members

            public Point GetPoint(Point[] points)
            {
                return points[_index];
            }

            public void AddVector(Vector[] result, Vector vector)
            {
                result[_index] += vector;
            }

            #endregion
        }

        private class ConstrainedPointStrategy : IPointStrategy
        {
            private readonly int _index;
            private readonly bool[] _locks;

            public ConstrainedPointStrategy(int index, bool[] locks)
            {
                _index = index;
                _locks = locks;
            }

            #region IPointStrategy Members

            public Point GetPoint(Point[] points)
            {
                return points[_index];
            }

            public void AddVector(Vector[] result, Vector vector)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (!_locks[i])
                    {
                        result[_index][i] += vector[i];
                    }
                }
            }

            #endregion
        }

        private class FixedPointStrategy : IPointStrategy
        {
            private readonly Point _point;

            public FixedPointStrategy(Point point)
            {
                _point = point;
            }

            #region IPointStrategy Members

            public Point GetPoint(Point[] points)
            {
                return _point;
            }

            public void AddVector(Vector[] result, Vector vector)
            {
            }

            #endregion
        }

        private class LocalPointsFunctionWithGradient : AbstractPointsFunctionWithGradient
        {
            private readonly Func<Point[], double> _evaluate;
            private readonly GradientDelegate<Point, Vector> _evaluateGradient;

            public LocalPointsFunctionWithGradient(Func<Point[], double> evaluate,
                GradientDelegate<Point, Vector> evaluateGradient)
            {
                _evaluate = evaluate;
                _evaluateGradient = evaluateGradient;
            }

            public override double Evaluate(Point[] argument)
            {
                return _evaluate(argument);
            }

            public override void EvaluateGradient(Point[] argument, Vector[] result)
            {
                _evaluateGradient(argument, result);
            }
        }

        private class GlobalPointsFunctionWithGradient : AbstractPointsFunctionWithGradient
        {
            private readonly IPointStrategy[][] _faces;
            private readonly IPointsFunctionWithGradient _localFunction;

            public GlobalPointsFunctionWithGradient(IPointsFunctionWithGradient localFunction,
                IPointStrategy[][] faces)
            {
                _faces = faces;
                _localFunction = localFunction;
            }

            public override double Evaluate(Point[] argument)
            {
                double result = 0;
                foreach (IPointStrategy[] face in _faces)
                {
                    Point[] localPoints = Array.ConvertAll(face, strategy => strategy.GetPoint(argument));
                    result += _localFunction.Evaluate(localPoints);
                }
                return result;
            }

            public override void EvaluateGradient(Point[] argument, Vector[] result)
            {
                Array.Clear(result, 0, result.Length);
                foreach (IPointStrategy[] face in _faces)
                {
                    Point[] localPoints = Array.ConvertAll(face, strategy => strategy.GetPoint(argument));
                    Vector[] localGradientValue = new Vector[3];
                    _localFunction.EvaluateValueWithGradient(localPoints, localGradientValue);
                    for (int i = 0; i < 3; i++)
                    {
                        face[i].AddVector(result, localGradientValue[i]);
                    }
                }
            }
        }
    }
}
