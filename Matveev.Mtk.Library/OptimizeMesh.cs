using System;
using System.Linq;
using System.Collections.Generic;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library.FaceFunctions;

namespace Matveev.Mtk.Library
{
    public static class OptimizeMesh
    {
        public static void ImproveVertexPositions(Mesh mesh, IImplicitSurface surface)
        {
            Vertex[] vertices = mesh.Vertices.ToArray();
            List<int> fixedPoints = new List<int>();
            double[] x = new double[3 * vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                Point point = vertices[i].Point;
                x[3 * i] = point.X;
                x[3 * i + 1] = point.Y;
                x[3 * i + 2] = point.Z;
                if (vertices[i].Type != VertexType.Internal)
                {
                    fixedPoints.Add(i);
                }
            }
            Func<Vertex, int> indexSelector = vertex => Array.IndexOf(vertices, vertex);
            List<int[]> faces =               
                new List<int[]>(mesh.Faces.Select(face => face.Vertices.Select(indexSelector).ToArray()));

            Func<Point[], double> faceEnergy =
                TriangleImplicitApproximations.GetApproximation(surface.Eval, "square");

            Func<double[], double> globalEnergy = delegate(double[] globalX)
            {
                double energy = 0;

                foreach (int[] face in faces)
                {
                    // TODO: Refactor, extract converting methods.
                    Point[] points = new Point[3];
                    for (int i = 0; i < points.Length; i++)
                    {
                        int index = face[i];
                        points[i] = new Point(x[3 * index], x[3 * index + 1], x[3 * index + 2]);
                    }
                    energy += faceEnergy(points);
                }

                return energy;
            };

            Func<Point[], Vector[]> localGradient = LocalGradientProvider.GetNumericalGradient2(faceEnergy, 1e-6);

            Func<double[], double[]> globalGradient = delegate(double[] globalX)
            {
                double[] result = new double[globalX.Length];

                foreach (int[] face in faces)
                {
                    // TODO: Refactor, extract converting methods.
                    Point[] points = new Point[3];
                    for (int i = 0; i < face.Length; i++)
                    {
                        int index = face[i];
                        points[i] = new Point(x[3 * index], x[3 * index + 1], x[3 * index + 2]);
                    }
                    Vector[] localGradValue = localGradient(points);
                    for (int i = 0; i < face.Length; i++)
                    {
                        int index = face[i];
                        result[3 * index] += localGradValue[i].x;
                        result[3 * index + 1] += localGradValue[i].y;
                        result[3 * index + 2] += localGradValue[i].z;
                    }
                }
                foreach (int fixedPoint in fixedPoints)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        result[3 * fixedPoint + i] = 0;
                    }
                }

                return result;
            };

            FunctionOptimization.GradientDescent(globalEnergy, globalGradient, x, 1e-8, 300);

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Point = new Point(x[3 * i], x[3 * i + 1], x[3 * i + 2]);
            }
        }

        public static void OptimizeImplicit(Mesh mesh, IImplicitSurface field, double epsilon, double alpha)
        {
            List<EdgeTransform> transforms = new List<EdgeTransform>(5);
            transforms.Add(new EdgeCollapse(0.5));
            transforms.Add(new EdgeCollapse(0));
            transforms.Add(new EdgeCollapse(1));
            transforms.Add(new EdgeSwap());
            transforms.Add(new EdgeSplit(0.5));
            Dictionary<EdgeTransform, int> numbersOfUses = new Dictionary<EdgeTransform, int>();

            Func<Point[], double> faceEnergy =
                TriangleImplicitApproximations.GetApproximation(field.Eval, "square");
            Energy energy = new CompositeEnergy(new VertexEnergy(alpha), new FaceEnergy(faceEnergy));

            //Этап 1. Проецирование всех вершин сетки на поверхность
            ProjectAll(mesh, field, epsilon);

            //Этап 2. Улучшение позиций всех вершин сетки
            ImproveVertexPositions(mesh, field);

            //Этап 3. Преобразования над структурой сетки
            Random rand = new Random(42);
            List<Edge> candidats = new List<Edge>(mesh.Edges);
            Edge candidat, smCandidat;
            List<Face> surrounding = new List<Face>();
            Mesh submesh;
            Dictionary<Edge, Edge> edgeMap = new Dictionary<Edge, Edge>();
            double E1, E2;
            while (candidats.Count != 0)
            {
                candidat = candidats[rand.Next(candidats.Count - 1)];
                candidats.RemoveAll(edge => edge == candidat || edge == candidat.Pair);

                surrounding.Clear();
                surrounding.AddRange(candidat.Begin.AdjacentFaces.Concat(candidat.End.AdjacentFaces).Distinct());

                submesh = mesh.CloneSub(surrounding, null, edgeMap, null);
                smCandidat = edgeMap[candidat];
                E1 = energy.Eval(submesh);

                foreach (EdgeTransform transform in transforms)
                {
                    if (!transform.IsPossible(candidat))
                        continue;

                    IDictionary<Edge, Edge> edgeMap2 = new Dictionary<Edge, Edge>();
                    Mesh submesh2 = submesh.Clone(edgeMap2);
                    Edge smCandidat2 = edgeMap2[smCandidat];

                    try
                    {
                        MeshPart smResult = transform.Execute(smCandidat2);

                        foreach (Vertex vertex in smResult.GetVertices(0))
                        {
                            VertexOps.OptimizePosition(vertex, field, epsilon);
                        }

                        E2 = energy.Eval(submesh2);

                        if (E1 > E2)
                        {
                            if (numbersOfUses.ContainsKey(transform))
                            {
                                numbersOfUses[transform] = numbersOfUses[transform] + 1;
                            }
                            else
                            {
                                numbersOfUses.Add(transform, 1);
                            }
                            candidats.RemoveAll(edgeMap.ContainsKey);

                            MeshPart result = transform.Execute(candidat);

                            foreach (Vertex vertex in smResult.GetVertices(0))
                            {
                                VertexOps.OptimizePosition(vertex, field, epsilon);
                            }
                            candidats.AddRange(result.GetEdges(1));
                            break;
                        }
                        else
                        {
                            Tools.VertexPositionOptimizer.OptimizeAll(submesh2, field, epsilon, energy);
                            double E3 = energy.Eval(submesh2);
                            if (E1 > E3)
                                throw new Exception("Should optimize vertex positions " + transform.ToString());
                        }
                    }
                    catch
                    {
                    }
                }
            }
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
