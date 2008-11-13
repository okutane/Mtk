using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.Validators
{
    public class DuplicatedEdgesValidator : IMeshValidator
    {
        private class EdgeEqualityComparer : IEqualityComparer<Edge>
        {
            #region IEqualityComparer<Edge> Members

            public bool Equals(Edge x, Edge y)
            {
                return x.Begin.Equals(y.Begin) && x.End.Equals(y.End);

            }

            public int GetHashCode(Edge obj)
            {
                return obj.Begin.GetHashCode() ^ obj.End.GetHashCode();
            }

            #endregion
        }

        #region IMeshValidator Members

        public bool IsValid(Mesh mesh)
        {
            HashSet<Edge> set = new HashSet<Edge>(new EdgeEqualityComparer());
            foreach (Edge edge in mesh.Edges)
            {
                if (!set.Add(edge))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}
