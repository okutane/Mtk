using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    abstract class AbstractPointsFunctionWithGradient : IPointsFunctionWithGradient
    {
        #region IPointsFunctionWithGradient Members

        public abstract double Evaluate(Point[] argument);

        public abstract void EvaluateGradient(Point[] argument, Vector[] result);

        public virtual double EvaluateValueWithGradient(Point[] argument, Vector[] result)
        {
            EvaluateGradient(argument, result);
            return Evaluate(argument);
        }

        #endregion
    }
}
