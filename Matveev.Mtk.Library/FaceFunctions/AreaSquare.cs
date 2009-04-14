using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.FaceFunctions
{
    public class AreaSquare : AbstractPointsFunctionWithGradient
    {
        private AreaSquare()
        {
        }

        public override double Evaluate(Point[] argument)
        {
            return Math.Pow(2 * argument[0].AreaTo(argument[1], argument[2]), 2);
        }

        public override void EvaluateGradient(Point[] argument, Vector[] result)
        {
            Vector[] edges = new Vector[3];
            for (int i = 0; i < 3; i++)
            {
                edges[i] = argument[(i + 1) % 3] - argument[i];
            }
            Vector normal = edges[1] ^ edges[2];
            for (int i = 0; i < 3; i++)
            {
                result[i] = 2 * normal ^ edges[(i + 1) % 3];
            }
        }

        public static readonly IPointsFunctionWithGradient Instance = new AreaSquare();
    }
}
