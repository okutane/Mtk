using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library.Utilities;

namespace Matveev.Mtk.Library
{
    public static class DynamicMeshOptimization
    {
        private static Dictionary<Face, double> _AREAS;

        public static void Optimize(Mesh mesh, IImplicitSurface surface,
            IProgressMonitor monitor)
        {
            int N = 100;
            _AREAS = new Dictionary<Face, double>();
            Vertex[] vertices =
                mesh.Vertices.Where(v => v.Type == VertexType.Internal).ToArray();
            Point[] newPoints = new Point[vertices.Length];
            for(int k = 0; k < N && !monitor.IsCancelled; k++)
            {
                monitor.ReportProgress(100 * k / N);

                foreach (Face face in mesh.Faces)
                {
                    _AREAS[face] = face.Area();
                }
                for (int i = 0; i < vertices.Length; i++)
                {
                    double tau = 1.0 / (100 * vertices.Max(v => v.AdjacentFaces.Sum(f => _AREAS[f])
                        * Math.Abs(surface.Eval(v.Point)) * surface.Grad(v.Point).Norm));
                    Vertex vertex = vertices[i];
                    newPoints[i] = vertex.Point
                        + DistanceOptimizationDirection(vertex, surface, tau)
                        + NormalsOptimizationDirection(vertex, surface)
                        + RelaxationOptimizationDirection(vertex, surface);
                }
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i].Point = newPoints[i];
                }
            }
            _AREAS = null;
        }

        private static Vector DistanceOptimizationDirection(Vertex vertex, IImplicitSurface surface, double tau)
        {
            return -2 * tau * vertex.AdjacentFaces.Sum(f => _AREAS[f]) * surface.Eval(vertex.Point)
                * surface.Grad(vertex.Point);
        }

        private static Vector NormalsOptimizationDirection(Vertex vertex, IImplicitSurface surface)
        {
            Func<Face, Vector> v = delegate(Face f)
            {
                Point[] points = f.Vertices.Select(faceVertex => faceVertex.Point).ToArray();
                Point centroid = points[0].Interpolate(points[1], points[2], 1.0 / 3.0, 1.0 / 3.0);
                Vector pc = centroid - vertex.Point;
                Vector normal = Vector.Normalize(surface.Grad(centroid));
                return (pc * normal) * normal;
            };
            return (1 / vertex.AdjacentFaces.Sum(f => _AREAS[f]))
                * vertex.AdjacentFaces.Aggregate(new Vector(), (vector, face) => vector + _AREAS[face] * v(face));
        }

        private static Vector RelaxationOptimizationDirection(Vertex vertex, IImplicitSurface surface)
        {
            double c = 0.1;
            int count = 0;
            Vector u = vertex.Adjacent.Aggregate(new Vector(), (vector, neighbor) =>
            {
                count++;
                return vector + (neighbor.Point - vertex.Point);
            }, vector => (1.0 / count) * vector);
            Vector n = Vector.Normalize(surface.Grad(vertex.Point));
            return c * (u - (u * n) * n);
        }
    }
}
