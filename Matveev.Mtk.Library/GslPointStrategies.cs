using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GslNet;

using Matveev.Mtk.Core;
using GslNet.MultiMin;
using Matveev.Common;

namespace Matveev.Mtk.Library
{
    interface IPointStrategy<ArgType, GradType>
    {
        Point this[ArgType points]
        {
            get;
            set;
        }
        void AddVector(GradType result, Vector vector);
    }

    #region Simple point access strategies

    class NormalPointStrategy : IPointStrategy<Point[], Vector[]>
    {
        private readonly int _index;

        public NormalPointStrategy(int index)
        {
            _index = index;
        }

        #region IPointStrategy<Point[],Vector[]> Members

        public Point this[Point[] points]
        {
            get
            {
                return points[_index];
            }
            set
            {
                points[_index] = value;
            }
        }

        public void AddVector(Vector[] result, Vector vector)
        {
            result[_index] += vector;
        }

        #endregion
    }

    class ConstrainedPointStrategy : IPointStrategy<Point[], Vector[]>
    {
        private readonly int _index;
        private readonly bool[] _locks;

        public ConstrainedPointStrategy(int index, bool[] locks)
        {
            _index = index;
            _locks = locks;
        }

        #region IPointStrategy<Point[],Vector[]> Members

        public Point this[Point[] points]
        {
            get
            {
                return points[_index];
            }
            set
            {
                for(int i = 0 ; i < 3 ; i++)
                {
                    if(!_locks[i])
                    {
                        points[_index][i] = value[i];
                    }
                }
            }
        }

        public void AddVector(Vector[] result, Vector vector)
        {
            for(int i = 0 ; i < 3 ; i++)
            {
                if(!_locks[i])
                {
                    result[_index][i] += vector[i];
                }
            }
        }

        #endregion
    }

    #endregion

    class FixedPointStrategy : IPointStrategy<Point[], Vector[]>, IPointStrategy<GslVector, GslVector>
    {
        private readonly Point _point;

        public FixedPointStrategy(Point point)
        {
            _point = point;
        }

        #region IPointStrategy<Point[],Vector[]> Members

        public Point this[Point[] points]
        {
            get
            {
                return _point;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public void AddVector(Vector[] result, Vector vector)
        {
        }

        #endregion

        #region IPointStrategy<GslVector,GslVector> Members

        public Point this[GslVector points]
        {
            get
            {
                return _point;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public void AddVector(GslVector result, Vector vector)
        {
        }

        #endregion
    }

    class GslPointStrategy : IPointStrategy<GslVector, GslVector>
    {
        private readonly int _offset;

        public GslPointStrategy(ref int offset)
        {
            _offset = offset;
            offset += 3;
        }

        #region IPointStrategy<GslVector,GslVector> Members

        public void AddVector(GslVector result, Vector vector)
        {
            result[_offset] += vector.x;
            result[_offset + 1] += vector.y;
            result[_offset + 2] += vector.z;
        }

        public Point this[GslVector points]
        {
            get
            {
                return new Point(points[_offset], points[_offset + 1], points[_offset + 2]);
            }
            set
            {
                points[_offset] = value.X;
                points[_offset + 1] = value.Y;
                points[_offset + 2] = value.Z;
            }
        }

        #endregion
    }

    class GslFixedOnVector : IPointStrategy<GslVector, GslVector>
    {
        private readonly int _offset;
        private readonly Point _point;
        private readonly Vector _vector;

        public GslFixedOnVector(ref int offset, Point point, Vector vector)
        {
            _offset = offset;
            _point = point - (point * vector) * vector;
            _vector = vector;
            offset += 1;
        }

        #region IPointStrategy<GslVector,GslVector> Members

        public Point this[GslVector points]
        {
            get
            {
                return _point + points[_offset] * _vector;
            }
            set
            {
                points[_offset] = (value * _vector);
            }
        }

        public void AddVector(GslVector result, Vector vector)
        {
            result[_offset] += _vector * vector;
        }

        #endregion
    }

    class GslFixedOnPlane : IPointStrategy<GslVector, GslVector>
    {
        private readonly int _offset;
        private readonly Point _point;
        private readonly Vector _vector1;
        private readonly Vector _vector2;

        public GslFixedOnPlane(ref int offset, Point point, Vector vector1, Vector vector2)
        {
            _offset = offset;
            _point = point - (point * vector1) * vector1 - (point * vector2) * vector2;
            _vector1 = vector1;
            _vector2 = vector2;
            offset += 2;
        }

        #region IPointStrategy<GslVector,GslVector> Members

        public Point this[GslVector points]
        {
            get
            {
                return _point + points[_offset] * _vector1 + points[_offset + 1] * _vector2;
            }
            set
            {
                points[_offset] = (value * _vector1);
                points[_offset + 1] = (value * _vector2);
            }
        }

        public void AddVector(GslVector result, Vector vector)
        {
            result[_offset] += (vector * _vector1);
            result[_offset + 1] += (vector * _vector2);
        }

        #endregion
    }

    class GslFixedOnePointStrategy : IPointStrategy<GslVector, GslVector>
    {
        private readonly int _offset;

        public GslFixedOnePointStrategy(int offset)
        {
            _offset = offset;
        }

        #region IPointStrategy<GslVector,GslVector> Members

        public Point this[GslVector points]
        {
            get
            {
                return new Point(points[_offset], points[_offset + 1], points[_offset + 2]);
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void AddVector(GslVector result, Vector vector)
        {
            result[_offset] = vector.x;
            result[_offset + 1] = vector.y;
            result[_offset + 2] = vector.z;
        }

        #endregion
    }

    class GslGlobalPointsFunctionWithGradient : IFunctionWithGradient
    {
        private readonly Dictionary<Pair<double, IPointsFunctionWithGradient>, IPointStrategy<GslVector, GslVector>[][]> _functionsToStrategies;
        private Vector[] _localGradientValue = new Vector[16];

        public GslGlobalPointsFunctionWithGradient(
            Dictionary<Pair<double, IPointsFunctionWithGradient>, IPointStrategy<GslVector, GslVector>[][]> functionsToStrategies)
        {
            _functionsToStrategies = functionsToStrategies;
        }

        public double Evaluate(GslVector argument)
        {
            double result = 0;
            foreach(var functionWithStrategies in _functionsToStrategies)
            {
                var weight = functionWithStrategies.Key.First;
                var function = functionWithStrategies.Key.Second;
                double partialSum = 0;
                foreach(var set in functionWithStrategies.Value)
                {
                    Point[] pointsSet = Array.ConvertAll(set, strategy => strategy[argument]);
                    partialSum += function.Evaluate(pointsSet);
                }
                result += weight * partialSum;
            }
            return result;
        }

        public void EvaluateGradient(GslVector argument, GslVector result)
        {
            result.SetZero();
            foreach(var functionWithStrategies in _functionsToStrategies)
            {
                foreach(var set in functionWithStrategies.Value)
                {
                    if(set.Length > _localGradientValue.Length)
                    {
                        _localGradientValue = new Vector[set.Length * 2];
                    }
                    var weight = functionWithStrategies.Key.First;
                    var function = functionWithStrategies.Key.Second;
                    Point[] pointsSet = Array.ConvertAll(set, strategy => strategy[argument]);
                    function.EvaluateGradient(pointsSet, _localGradientValue);
                    for(int i = 0 ; i < set.Length ; i++)
                    {
                        set[i].AddVector(result, weight * _localGradientValue[i]);
                    }
                }
            }
        }

        public double EvaluateValueWithGradient(GslVector argument, GslVector result)
        {
            EvaluateGradient(argument, result);
            return Evaluate(argument);
        }
    }
}
