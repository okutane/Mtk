using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

using Matveev.Common;

namespace Matveev.Mtk.Core
{
    public class Point : IPoint<Point>
    {
        public double X;
        public double Y;
        public double Z;

        public Point(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public static bool operator ==(Point left, Point right)
        {
            Vector v = right - left;
            return v.Norm < 0.000001;
        }
        public static bool operator !=(Point left, Point right)
        {
            return left.X != right.X || left.Y != right.Y || left.Z != right.Z;
        }

        [IndexerName("Coords")]
        public double this[int index]
        {
            get
            {
                if (index == 0)
                {
                    return X;
                }
                else if (index == 1)
                {
                    return Y;
                }
                else
                {
                    return Z;
                }
            }
            set
            {
                if (index == 0)
                {
                    X = value;
                }
                else if (index == 1)
                {
                    Y = value;
                }
                else
                {
                    Z = value;
                }
            }
        }
	
        public static Vector operator -(Point left, Point right)
        {
            Vector result;
            result.x = left.X - right.X;
            result.y = left.Y - right.Y;
            result.z = left.Z - right.Z;
            return result;
        }

        public static Point operator -(Point left, Vector right)
        {
            return new Point(left.X - right.x,
                left.Y - right.y, left.Z - right.z);
        }

        public static Point operator +(Point left, Vector right)
        {
            return new Point(left.X + right.x,
                left.Y + right.y, left.Z + right.z);
        }

        public override bool Equals(object obj)
        {
            try
            {
                Point p2 = (Point)obj;
                return this == p2;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override string ToString()
        {
            return string.Format(NumberFormatInfo.InvariantInfo, "({0},{1},{2})", X, Y, Z);
        }

        public static explicit operator Vector(Point p)
        {
            return new Vector(p.X, p.Y, p.Z);
        }

        public static Point Interpolate(Point p0, double v0, Point p1, double v1)
        {
            double t = v0 / (v0 - v1);

            if (t < 0 || t > 1)
            {
                throw new ArgumentException("The point with zero value is not between p0 and p1.");
            }

            return p0.Interpolate(p1, t);
        }

        #region IPoint<Point> Members

        public Point Interpolate(Point p1, double t)
        {
            double r = 1 - t;
            return new Point(X * r + t * p1.X, Y * r + t * p1.Y, Z * r + t * p1.Z);
        }

        public Point Interpolate(Point p1, Point p2, double u, double v)
        {
            double r = 1 - u - v;

            return new Point(X * r + p1.X * u + p1.X * v,
                Y * r + p1.Y * u + p2.Y * v, Z * r + p1.Z * u + p2.Z * v);
        }

        public double DistanceTo(Point p1)
        {
            throw new NotImplementedException();
        }

        public double AreaTo(Point p1, Point p2)
        {
            Vector vector1 = p1 - this;
            Vector vector2 = p2 - this;
            Vector vector3 = p1 - p2;

            double a = vector1.Norm;
            double b = vector2.Norm;
            double c = vector3.Norm;
            double p = (a + b + c) / 2;

            return Math.Sqrt(p * (p - a) * (p - b) * (p - c));
        }

        #endregion

        public readonly static Point ORIGIN = new Point(0, 0, 0);
    }
}
