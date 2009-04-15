using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using Matveev.Common;

namespace Matveev.Mtk.Core
{
    public struct Vector : IAdditable<Vector, Vector>, ISizeable
    {
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
        public double x;
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y")]
        public double y;
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "z")]
        public double z;

        public Vector(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public double this[int index]
        {
            get
            {
                if(index == 0)
                    return x;
                else if(index == 1)
                    return y;
                else
                    return z;
            }
            set
            {
                if(index == 0)
                    x = value;
                else if(index == 1)
                    y = value;
                else
                    z = value;
            }
        }

        public double Norm
        {
            get
            {
                return Math.Sqrt(x * x + y * y + z * z);
            }
        }

        public static Vector operator +(Vector left, Vector right)
        {
            return new Vector(left.x + right.x, left.y + right.y, left.z + right.z);
        }

        public static Vector operator -(Vector left, Vector right)
        {
            return new Vector(left.x - right.x, left.y - right.y, left.z - right.z);
        }

        public static Vector operator *(double left, Vector right)
        {
            Vector result = right;
            result.x *= left;
            result.y *= left;
            result.z *= left;
            return result;
        }

        public static double operator *(Vector left, Vector right)
        {
            return left.x * right.x + left.y * right.y + left.z * right.z;
        }

        public static Vector operator^(Vector left, Vector right)
        {
            Vector result;
            result.x = left.y * right.z - left.z * right.y;
            result.y = left.z * right.x - left.x * right.z;
            result.z = left.x * right.y - left.y * right.x;
            return result;
        }

        public static Vector Normalize(Vector vector)
        {
            Vector result;
            double norm = vector.Norm;
            if (norm == 0)
            {
                return vector;
            }
            result.x = vector.x / norm;
            result.y = vector.y / norm;
            result.z = vector.z / norm;

            return result;
        }

        public override string ToString()
        {
            return string.Format(NumberFormatInfo.InvariantInfo, "({0},{1},{2})", x, y, z);
        }

        public static implicit operator Point(Vector v)
        {
            return new Point(v.x, v.y, v.z);
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector)
            {
                Vector other = (Vector)obj;
                return (this - other).Norm < 1e-15;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        #region ISizeable Members

        public double Size()
        {
            return Norm;
        }

        #endregion

        #region IAdditable<Vector,Vector> Members

        public Vector Add(Vector other, double weight)
        {
            return this + weight * other;
        }

        #endregion
    }
}
