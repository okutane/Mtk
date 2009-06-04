using System;
using System.Collections.Generic;
using System.Text;

namespace Matveev.Mtk.Core
{
    public interface IEdgeFunction
    {
        double Evaluate(Edge edge);
    }
}
