using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Matveev.Mtk.Core
{
    public abstract class FaceFunction
    {
        public abstract double Evaluate(Face face);

        public double[] EvaluateGradient(Face face)
        {
            double[] result = new double[9];
            this.EvaluateGradientTo(face, result);
            return result;
        }

        public virtual void EvaluateGradientTo(Face face, double[] dest)
        {
            if (face == null)
                throw new ArgumentNullException("face");
            if (dest == null)
                throw new ArgumentNullException("dest");
            if (dest.Length != 9)
                throw new ArgumentException("dest.Length != 9", "dest");

            // TODO: Place linear approximation here.
        }
    }
}
