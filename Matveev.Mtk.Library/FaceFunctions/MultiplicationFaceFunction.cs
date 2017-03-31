using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.FaceFunctions
{
    public sealed class MultiplicationFaceFunction : IPointsFunctionWithGradient
    {
        private readonly IPointsFunctionWithGradient _f1;
        private readonly IPointsFunctionWithGradient _f2;

        public MultiplicationFaceFunction(IPointsFunctionWithGradient f1, IPointsFunctionWithGradient f2)
        {
            _f1 = f1;
            _f2 = f2;
        }

        #region IPointsFunctionWithGradient Members

        public double Evaluate(Point[] argument)
        {
            return _f1.Evaluate(argument) * _f2.Evaluate(argument);
        }

        public void EvaluateGradient(Point[] argument, Vector[] result)
        {
            double value1 = _f1.EvaluateValueWithGradient(argument, result);
            Vector[] grad2 = new Vector[3];
            double value2 = _f2.EvaluateValueWithGradient(argument, grad2);
            for(int i = 0 ; i < 3 ; i++)
            {
                result[i] = value1 * grad2[i] + value2 * result[i];
            }
        }

        public double EvaluateValueWithGradient(Point[] argument, Vector[] result)
        {
            double value1 = _f1.EvaluateValueWithGradient(argument, result);
            Vector[] grad2 = new Vector[3];
            double value2 = _f2.EvaluateValueWithGradient(argument, grad2);
            for(int i = 0 ; i < 3 ; i++)
            {
                result[i] = value1 * grad2[i] + value2 * result[i];
            }
            return value1 * value2;
        }

        public IPointSelectionStrategy PointSelectionStrategy
        {
            get
            {
                return FacesSelectionStrategy.Instance;
            }
        }

        #endregion
    }
}
