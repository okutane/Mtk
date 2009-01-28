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
            Vertex[] verticesArray = vertices.Where(v => v.Type == VertexType.Internal).ToArray();
            List<Vertex> constVertices = new List<Vertex>();
            Func<Vertex, int> indexSelector = delegate(Vertex v)
            {
                int vertexIndex = Array.IndexOf(verticesArray, v);
                if (vertexIndex >= 0)
                {
                    return vertexIndex;
                }
                vertexIndex = constVertices.IndexOf(v);
                if (vertexIndex >= 0)
                {
                    return -vertexIndex - 1;
                }
                constVertices.Add(v);
                return -constVertices.Count;
            };
            IEnumerable<Face> facesCollection = verticesArray.SelectMany(v => v.AdjacentFaces).Distinct();
            int[][] faces = facesCollection.Select(f => f.Vertices.Select(indexSelector).ToArray()).ToArray();

            Func<Point[], double> faceEnergy;
            GradientDelegate<Point, Vector> localGradient;
            GetLocalFunctions(surface, out faceEnergy, out localGradient);

            Point[] points = new Point[3];
            Func<Point[], double> globalEnergy = delegate(Point[] x)
            {
                double energy = 0;

                foreach (int[] face in faces)
                {
                    // TODO: Refactor, extract converting methods.
                    for (int i = 0; i < points.Length; i++)
                    {
                        int index = face[i];
                        if (index >= 0)
                        {
                            points[i] = x[index];
                        }
                        else
                        {
                            points[i] = constVertices[-index - 1].Point;
                        }
                    }
                    double weight = points[0].AreaTo(points[1], points[2]);
                    weight = 1;
                    energy += weight * faceEnergy(points);
                }

                return energy;
            };

            Vector[] localGradValue = new Vector[3];
            GradientDelegate<Point, Vector> globalGradient = delegate(Point[] x, Vector[] result)
            {
                Array.Clear(result, 0, result.Length);
                foreach (int[] face in faces)
                {
                    // TODO: Refactor, extract converting methods.
                    for (int i = 0; i < face.Length; i++)
                    {
                        int index = face[i];
                        if (index >= 0)
                        {
                            points[i] = x[index];
                        }
                        else
                        {
                            points[i] = constVertices[-index - 1].Point;
                        }
                    }
                    localGradient(points, localGradValue);
                    double weight = points[0].AreaTo(points[1], points[2]);
                    weight = 1;
                    for (int i = 0; i < face.Length; i++)
                    {
                        int index = face[i];
                        if (index >= 0)
                        {
                            result[index] = result[index].Add(localGradValue[i], weight);
                        }
                    }
                }
            };

            Point[] buffer = verticesArray.Select(v => v.Point).ToArray();

            FunctionOptimization<Point, Vector>.GradientDescent(globalEnergy, globalGradient, buffer, 1e-8, 300);

            for (int i = 0; i < verticesArray.Length; i++)
            {
                verticesArray[i].Point = buffer[i];
            }
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

        public static void ImproveVertexPositions(Mesh mesh, IImplicitSurface surface)
        {
            Vertex[] vertices = mesh.Vertices.ToArray();
            List<int> fixedPoints = new List<int>();
            for (int i = 0; i < vertices.Length; i++)
            {
                Point point = vertices[i].Point;
                if (vertices[i].Type != VertexType.Internal)
                {
                    fixedPoints.Add(i);
                }
            }
            Func<Vertex, int> indexSelector = vertex => Array.IndexOf(vertices, vertex);
            int[][] faces = mesh.Faces.Select(face => face.Vertices.Select(indexSelector).ToArray()).ToArray();

            Func<Point[], double> faceEnergy;
            GradientDelegate<Point, Vector> localGradient;
            GetLocalFunctions(surface, out faceEnergy, out localGradient);
            
            Point[] points = new Point[3];
            Func<Point[], double> globalEnergy = delegate(Point[] x)
            {
                double energy = 0;

                foreach (int[] face in faces)
                {
                    // TODO: Refactor, extract converting methods.
                    for (int i = 0; i < points.Length; i++)
                    {
                        int index = face[i];
                        points[i] = x[index];
                    }
                    double weight = points[0].AreaTo(points[1], points[2]);
                    weight = 1;
                    energy += weight * faceEnergy(points);
                }

                return energy;
            };

            Vector[] localGradValue = new Vector[3];
            GradientDelegate<Point, Vector> globalGradient = delegate(Point[] x, Vector[] result)
            {
                Array.Clear(result, 0, result.Length);
                foreach (int[] face in faces)
                {
                    // TODO: Refactor, extract converting methods.
                    for (int i = 0; i < face.Length; i++)
                    {
                        int index = face[i];
                        points[i] = x[index];
                    }
                    localGradient(points, localGradValue);
                    double weight = points[0].AreaTo(points[1], points[2]);
                    weight = 1;
                    for (int i = 0; i < face.Length; i++)
                    {
                        int index = face[i];
                        result[index] = result[index].Add(localGradValue[i], weight);
                    }
                }
                foreach (int fixedPoint in fixedPoints)
                {
                    result[fixedPoint] = new Vector();
                }
            };

            Point[] buffer = vertices.Select(v => v.Point).ToArray();

            FunctionOptimization<Point, Vector>.GradientDescent(globalEnergy, globalGradient, buffer, 1e-8, 300);

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Point = buffer[i];
            }
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
    }
}
