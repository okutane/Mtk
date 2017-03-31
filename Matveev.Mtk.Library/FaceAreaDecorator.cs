using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public class FaceAreaDecorator : AbstractPointsFunctionWithGradient
    {
        private readonly IPointsFunctionWithGradient _decorated;

        public FaceAreaDecorator(IPointsFunctionWithGradient decorated)
        {
            _decorated = decorated;
        }

        public override double Evaluate(Point[] argument)
        {
            return _decorated.Evaluate(argument) * argument[0].AreaTo(argument[1], argument[2]);
        }

        public override void EvaluateGradient(Point[] argument, Vector[] result)
        {
            double decoratedValue = _decorated.Evaluate(argument);
            _decorated.EvaluateGradient(argument, result);
            double area = argument[0].AreaTo(argument[1], argument[2]);
            Vector[] edges = new Vector[3];
            for(int i = 0 ; i < 3 ; i++)
            {
                edges[i] = argument[(i + 1) % 3] - argument[i];
            }
            for(int i = 0 ; i < 3 ; i++)
            {
                result[i] = area * result[i]
                    + (decoratedValue / area) * (edges[1] ^ edges[2]) ^ edges[(i + 1) % 3];
            }
        }
    }
}
