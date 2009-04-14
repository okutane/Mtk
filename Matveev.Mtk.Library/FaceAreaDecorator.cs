using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    class FaceAreaDecorator : AbstractPointsFunctionWithGradient
    {
        private readonly IPointsFunctionWithGradient _decorated;

        public FaceAreaDecorator(IPointsFunctionWithGradient decorated)
        {
            _decorated = decorated;
        }
        const double kappa = 0.0001;
        public override double Evaluate(Point[] argument)
        {
            return _decorated.Evaluate(argument) + kappa * Math.Pow(argument[0].AreaTo(argument[1], argument[2]), 2);
            return _decorated.Evaluate(argument) * argument[0].AreaTo(argument[1], argument[2]);
        }

        public override void EvaluateGradient(Point[] argument, Vector[] result)
        {
            //LocalGradientProvider.GetNumericalGradient2(Evaluate, 1e-3)(argument, result);
            //return;
            double decoratedValue = _decorated.Evaluate(argument);
            _decorated.EvaluateGradient(argument, result);
            double area = argument[0].AreaTo(argument[1], argument[2]);
            Vector[] edges = new Vector[3];
            for (int i = 0; i < 3; i++)
            {
                edges[i] = argument[(i + 1) % 3] - argument[i];
            }
            for (int i = 0; i < 3; i++)
            {
                result[i] += 2 * kappa * (edges[1] ^ edges[2]) ^ edges[(i + 1) % 3];
                //result[i] = area * result[i]
                  //  + (decoratedValue / area) * (edges[1] ^ edges[2]) ^ edges[(i + 1) % 3];
            }
        }
    }
}
