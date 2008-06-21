using System;
using System.Collections.Generic;
using System.Text;

using Matveev.Mtk.Library.PositionOptimizers;

namespace Matveev.Mtk.Library
{
    public static class Tools
    {
        public static VertexPositionOptimizer VertexPositionOptimizer = new PlaneVertexPositionOptimizer();
    }
}
