using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public class NotOrientedEdgeComparer : IEqualityComparer<Edge>
    {
        #region IEqualityComparer<Edge> Members

        public bool Equals(Edge x, Edge y)
        {
            return (x == y) || (x.Pair == y);
        }

        public int GetHashCode(Edge obj)
        {
            int result = obj.GetHashCode();
            if (obj.Pair != null)
            {
                result ^= obj.Pair.GetHashCode();
            }
            return result;
        }

        #endregion
    }
}
