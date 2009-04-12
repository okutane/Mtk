using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public interface IPointsFunctionWithGradient
    {
        double Evaluate(Point[] argument);
        void EvaluateGradient(Point[] argument, Vector[] result);
        double EvaluateValueWithGradient(Point[] argument, Vector[] result);
    }
}
