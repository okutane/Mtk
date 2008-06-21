using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.FaceFunctions
{
    public sealed class MultiplicationFaceFunction : FaceFunction
    {
        private FaceFunction _f1, _f2;

        public MultiplicationFaceFunction(FaceFunction f1, FaceFunction f2)
        {
            this._f1 = f1;
            this._f2 = f2;
        }

        public override double Evaluate(Face face)
        {
            return this._f1.Evaluate(face) * this._f2.Evaluate(face);
        }

        public override void EvaluateGradientTo(Face face, double[] dest)
        {
            double value1 = this._f1.Evaluate(face);
            double value2 = this._f2.Evaluate(face);

            double[] grad1 = this._f1.EvaluateGradient(face);
            double[] grad2 = this._f2.EvaluateGradient(face);

            for (int i = 0; i < 9; i++)
            {
                dest[i] = value1 * grad2[i] + value2 * grad1[i];
            }
        }
    }
}
