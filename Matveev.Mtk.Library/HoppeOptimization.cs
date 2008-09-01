using System;
using System.Collections.Generic;
using System.Linq;

using Matveev.Common;
using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public class HoppeOptimization
    {
        public static void OptimizeMesh(Mesh mesh, IEnumerable<Point> pointSet)
        {
            ImproveVertexPositions(mesh, pointSet);
        }

        public static void ImproveVertexPositions(Mesh mesh, IEnumerable<Point> pointSet)
        {
            var projections = ProjectPoints(mesh, pointSet);

            int n = mesh.Vertices.Count();

            double[,] a = new double[3 * n, 3 * n];
            MatrixBuilder mb = new MatrixBuilder(3 * n);
            double[] f = new double[3 * n];

            foreach (var projection in projections)
            {
                double[,] localA;
                double[] localF;
                Vertex[] map;

                projection.GetMatrixAndVector(out localA, out localF, out map);

                int[] indexMap = new int[localF.Length];
                for (int i = 0; i < map.Length; i++)
                {
                    int vertexIndex = mesh.Vertices.IndexOf(map[i]);
                    indexMap[3 * i] = 3 * vertexIndex;
                    indexMap[3 * i + 1] = 3 * vertexIndex + 1;
                    indexMap[3 * i + 2] = 3 * vertexIndex + 2;
                }

                for (int i = 0; i < localF.Length; i++)
                {
                    int ii = indexMap[i];
                    for (int j = 0; j < localF.Length; j++)
                    {
                        int jj = indexMap[j];

                        a[ii, jj] += localA[i, j];
                        mb[ii, jj] += localA[i, j];
                    }

                    f[ii] += localF[i];
                }
            }

            Matrix aMatrix = new Matrix(f.Length, f.Length);
            Matrix fMatrix = new Matrix(f.Length, 1);
            for (int i = 0; i < f.Length; i++)
            {
                for (int j = 0; j < f.Length; j++)
                {
                    aMatrix[i, j] = a[i, j];
                }
                fMatrix[i, 0] = f[i];
            }

            LinSolve.Gauss(aMatrix, fMatrix);

            int k = 0;
            foreach (var vertex in mesh.Vertices)
            {
                vertex.Point = new Point(fMatrix[3 * k, 0], fMatrix[3 * k + 1, 0], fMatrix[3 * k + 2, 0]);
                k++;
            }
        }

        public static IEnumerable<PointProjection> ProjectPoints(Mesh mesh,
            IEnumerable<Point> pointSet)
        {
            // TODO: Перенести этот метод в более правильное место.

            IEnumerable<PointProjection> projections = from point in pointSet
                                                          select Project(point, mesh);
            return projections;
        }

        private static PointProjection Project(Point point, Mesh mesh)
        {
            PointProjection projection = null;
            double minDistance = double.MaxValue;

            foreach (Face face in mesh.Faces)
            {
                Ray ray = new Ray
                {
                    origin = point,
                    direction = face.Normal
                };

                double distance;
                double u;
                double v;

                ray.Trace(face, out distance, out u, out v);

                if (u < 0 || v < 0 || u + v > 1)
                    continue;

                distance = Math.Abs(distance);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    projection = new FacePointProjection(point, face, u, v);
                }
            }

            foreach (Edge edge in mesh.Edges)
            {
                // TODO: Реализовать данный функционал. Некритично.
            }

            foreach (Vertex vertex in mesh.Vertices)
            {
                double distance = (vertex.Point - point).Norm;
                if (distance < minDistance)
                {
                    projection = new VertexPointProjection(point, vertex);
                    minDistance = distance;
                }
            }

            return projection;
        }
    }
}
