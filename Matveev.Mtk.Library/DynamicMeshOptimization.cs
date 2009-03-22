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
        public static void Optimize(Mesh mesh, IImplicitSurface surface)
        {
            int n = 200;
            Vertex[] vertices = mesh.Vertices.Where(v => v.Type == VertexType.Internal).ToArray();
            Point[] newPoints = new Point[vertices.Length];
            while (n-- != 0)
            {
                Dictionary<Face, double> areas = new Dictionary<Face, double>();
                for (int i = 0; i < vertices.Length; i++)
                {
                    double tau = 1.0 / (100 * vertices.Max(v => v.AdjacentFaces.Sum(f => f.Area()) * Math.Abs(surface.Eval(v.Point)) * surface.Grad(v.Point).Norm));
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
        }

        private static Vector DistanceOptimizationDirection(Vertex vertex, IImplicitSurface surface, double tau)
        {
            return -2 * tau * vertex.AdjacentFaces.Sum(f => f.Area()) * surface.Eval(vertex.Point)
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
            return (1 / vertex.AdjacentFaces.Sum(f => f.Area()))
                * vertex.AdjacentFaces.Aggregate(new Vector(), (vector, face) => vector + face.Area() * v(face));
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
