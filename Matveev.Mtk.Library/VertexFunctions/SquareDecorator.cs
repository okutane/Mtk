using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Common.Utilities;
using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.VertexFunctions
{
    public class SquareDecorator : AbstractPointsFunctionWithGradient
    {
        private readonly IImplicitSurface _decorated;

        public SquareDecorator(IImplicitSurface decorated)
        {
            _decorated = decorated;
        }

        public override double Evaluate(Point[] argument)
        {
            return _decorated.Eval(argument[0]).Sq();
        }

        public override void EvaluateGradient(Point[] argument, Vector[] result)
        {
            result[0] = 2 * _decorated.Eval(argument[0]) * _decorated.Grad(argument[0]);
        }

        public override IPointSelectionStrategy PointSelectionStrategy
        {
            get
            {
                return VertexSelectionStrategy.Instance;
            }
        }
    }
}
