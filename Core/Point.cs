using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Matveev.Mtk.Core
{
    public struct Point
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
            //return left.x == right.x && left.y == right.y && left.z == right.z;
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
                if(index == 0)
                    return X;
                else if(index == 1)
                    return Y;
                else
                    return Z;
            }
            set
            {
                if(index == 0)
                    X = value;
                else if(index == 1)
                    Y = value;
                else
                    Z = value;
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
            Point result = new Point();
            result.X = left.X - right.x;
            result.Y = left.Y - right.y;
            result.Z = left.Z - right.z;

            return result;
        }

        public static Point operator +(Point left, Vector right)
        {
            Point result = new Point();
            result.X = left.X + right.x;
            result.Y = left.Y + right.y;
            result.Z = left.Z + right.z;
            return result;
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
            return "(" + X + "," + Y + "," + Z + ")";
        }

        public static explicit operator Vector(Point p)
        {
            return new Vector(p.X, p.Y, p.Z);
        }

        public static Point Interpolate(Point p0, double v0, Point p1, double v1)
        {
            double t;
            Point result = new Point();

            t = v0 / (v0 - v1);

            if(t < 0 || t > 1)
                throw new ArgumentException("The point with zero value is not between p0 and p1.");

            result.X = p0.X * (1 - t) + t * p1.X;
            result.Y = p0.Y * (1 - t) + t * p1.Y;
            result.Z = p0.Z * (1 - t) + t * p1.Z;

            return result;
        }
    }
}
