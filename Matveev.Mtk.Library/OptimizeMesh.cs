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
        public static void ImproveVertexPositions(IEnumerable<Vertex> vertices, IImplicitSurface surface)
        {
            Func<Point[], double> faceEnergy;
            GradientDelegate<Point, Vector> localGradient;
            GetLocalFunctions(surface, out faceEnergy, out localGradient);
            ImproveVertexPositions(vertices, faceEnergy, localGradient);
        }

        private static void GetLocalFunctions(IImplicitSurface surface, out Func<Point[], double> faceEnergy,
            out GradientDelegate<Point, Vector> localGradient)
        {
            faceEnergy =
                TriangleImplicitApproximations.GetApproximation(surface.Eval, "square");

            QuadraticForm quadraticForm = surface as QuadraticForm;
            if (quadraticForm != null)
            {
                faceEnergy = quadraticForm.FaceDistance;
            }
            CompactQuadraticForm cqf = surface as CompactQuadraticForm;
            if (cqf != null)
            {
                faceEnergy = cqf.FaceDistance;
            }

            localGradient = LocalGradientProvider.GetNumericalGradient2(faceEnergy, 1e-6);

            if (cqf != null)
            {
                localGradient = delegate(Point[] facePoints, Vector[] result)
                {
                    double[] grad = cqf.GradOfFaceDistance(facePoints);
                    result[0] = new Vector(grad[0], grad[1], grad[2]);
                    result[1] = new Vector(grad[3], grad[4], grad[5]);
                    result[2] = new Vector(grad[6], grad[7], grad[8]);
                };
            }
        }

        public static void ImproveVertexPositions(IEnumerable<Vertex> vertices, Func<Point[], double> faceValue,
            GradientDelegate<Point, Vector> faceGradient)
        {
            Vertex[] verticesArray = vertices.ToArray();

            Func<Vertex, IPointStrategy> pointStrategyProvider = delegate(Vertex vertex)
            {
                int index = Array.IndexOf(verticesArray, vertex);
                if (index >= 0)
                {
                    return new NormalPointStrategy(index);
                }
                return new FixedPointStrategy(vertex.Point);
            };

            IEnumerable<Face> facesCollection = verticesArray.SelectMany(v => v.AdjacentFaces).Distinct();
            IPointStrategy[][] faces =
                facesCollection.Select(f => f.Vertices.Select(pointStrategyProvider).ToArray()).ToArray();

            Point[] buffer = verticesArray.Select(v => v.Point).ToArray();

            Func<Point[], double> globalEnergy = delegate(Point[] points)
            {
                double result = 0;
                foreach (IPointStrategy[] face in faces)
                {
                    Point[] localPoints = Array.ConvertAll(face, strategy => strategy.GetPoint(points));
                    double weight = localPoints[0].AreaTo(localPoints[1], localPoints[2]);
                    weight = 1;
                    result += weight * faceValue(localPoints);
                }
                return result;
            };

            GradientDelegate<Point, Vector> globalGradient = delegate(Point[] points, Vector[] result)
            {
                Array.Clear(result, 0, result.Length);
                foreach (IPointStrategy[] face in faces)
                {
                    Point[] localPoints = Array.ConvertAll(face, strategy => strategy.GetPoint(points));
                    Vector[] localGradientValue = new Vector[3];
                    faceGradient(localPoints, localGradientValue);
                    double weight = localPoints[0].AreaTo(localPoints[1], localPoints[2]);
                    weight = 1;
                    for (int i = 0; i < 3; i++)
                    {
                        face[i].AddVector(result, localGradientValue[i]);
                    }
                }
            };

            FunctionOptimization<Point, Vector>.GradientDescent(globalEnergy, globalGradient, buffer, 1e-8, 300);

            for (int i = 0; i < verticesArray.Length; i++)
            {
                verticesArray[i].Point = buffer[i];
            }
        }

        public static void ImproveVertexPositions(Mesh mesh, IImplicitSurface surface)
        {
            ImproveVertexPositions(mesh.Vertices.Where(v => v.Type == VertexType.Internal), surface);
        }

        public static void OptimizeImplicit(Mesh mesh, IImplicitSurface field, double epsilon, double alpha)
        {
            ICollection<EdgeTransform> transforms = Configuration.EdgeTransforms;

            Dictionary<EdgeTransform, int> numbersOfUses = new Dictionary<EdgeTransform, int>();

            Func<Point[], double> faceEnergy =
                TriangleImplicitApproximations.GetApproximation(field.Eval, "square");
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
            ImproveVertexPositions(mesh, field);

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

                        ImproveVertexPositions(smResult.GetVertices(0), field);

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

                        ImproveVertexPositions(result.GetVertices(0), field);
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
    }
}
