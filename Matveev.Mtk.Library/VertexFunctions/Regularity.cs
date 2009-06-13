using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Common;
using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.VertexFunctions
{
    public class Regularity : AbstractPointsFunctionWithGradient
    {
        GradientDelegate<Point, Vector> _numericalGradient;

        private Regularity()
        {
            _numericalGradient = LocalGradientProvider.GetNumericalGradient(Evaluate, 1e-6);
        }

        public override double Evaluate(Point[] points)
        {
            Vector normal = Vector.Normalize(Configuration.Default.Surface.Grad(points[0]));
            Plane3d plane = new Plane3d((Point)normal, normal);

            Ray ray = new Ray();
            var uv = new Point2d[points.Length - 1];
            for (int i = 0; i < points.Length - 1; i++)
            {
                Vector e1 = points[i + 1] - points[0];
                Vector e2 = points[(i + 1) % (points.Length - 1) + 1] - points[0];
                ray.direction = Vector.Normalize(e1 ^ e2);
                uv[i] = plane.Trace(ray);
            }

            for (int j = 2; j < uv.Length - 1; j++)
            {
                if (LineIntersectionTest2d(uv[0], uv[1], uv[j], uv[j + 1]))
                {
                    return 1;
                }
            }
            for (int i = 1; i < uv.Length - 2; i++)
            {
                for (int j = i + 2; j < uv.Length; j++)
                {
                    if (LineIntersectionTest2d(uv[i], uv[(i + 1) % uv.Length], uv[j], uv[(j + 1) % uv.Length]))
                    {
                        return 1;
                    }
                }
            }

            return 0;
        }

        public override void EvaluateGradient(Point[] argument, Vector[] result)
        {
            _numericalGradient(argument, result);
        }

        internal static bool TestBounds(double value)
        {
            return value >= VertexOps.INTERSECTION_THRESHOLD && value <= 1 - VertexOps.INTERSECTION_THRESHOLD;
        }

        private static bool LineIntersectionTest2d(Point2d p1, Point2d p2, Point2d p3, Point2d p4)
        {
            double a11, a12, a21, a22, f1, f2;
            a11 = p2.X - p1.X;
            a12 = p3.X - p4.X;
            f1 = p3.X - p1.X;
            a21 = p2.Y - p1.Y;
            a22 = p3.Y - p4.Y;
            f2 = p3.Y - p1.Y;
            double det = a11 * a22 - a12 * a21;
            if (TestBounds((f1 * a22 - f2 * a12) / det))
            {
                return TestBounds((a11 * f2 - a21 * f1) / det);
            }
            return false;
        }

        public static readonly IPointsFunctionWithGradient Instance = new Regularity();

        private class VertexWithNeighboursSelector : IPointSelectionStrategy
        {
            #region IPointSelectionStrategy Members

            public IEnumerable<IEnumerable<Vertex>> GetAllLinkedSets(Vertex[] vertices)
            {
                return vertices.Select(v => v.Adjacent.Prepend(v));
            }

            #endregion
        }
    }
}
