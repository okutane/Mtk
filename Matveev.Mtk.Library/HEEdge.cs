// HEEdge.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    internal class HEEdge : Edge
    {
        private Mesh _mesh;

        public HEVertexBase end;
        public HEEdge pair, next;
        public HEFace face;

        public HEEdge(Mesh mesh)
        {
            this._mesh = mesh;
            end = null;
            pair = next = null;
            face = null;
        }

        #region Inherited from Edge
        public override Vertex Begin
        {
            get
            {
                return Prev.End;
            }
        }

        public override Vertex End
        {
            get
            {
                return end;
            }
        }

        public override Edge Next
        {
            get
            {
                return next;
            }
        }

        public override Edge Prev
        {
            get
            {
                return next.next;
            }
        }

        public override Edge Pair
        {
            get
            {
                return pair;
            }
        }

        public override Face ParentFace
        {
            get
            {
                return face;
            }
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        public override Mesh Mesh
        {
            get
            {
                return this._mesh;
            }
        }

        public override Edge[] GetEdges(int radius)
        {
            if (radius == 0)
                return new Edge[] { this };
            else
            {
                ICollection<Edge> edgesNearBegin, edgesNearEnd;
                edgesNearBegin = this.Begin.GetEdges(radius);
                edgesNearEnd = this.End.GetEdges(radius);

                return Begin.GetEdges(radius).Union(End.GetEdges(radius)).ToArray();
            }
        }

        public override Vertex[] GetVertices(int radius)
        {
            if (radius == 0)
            {
                return new Vertex[] { this.Begin, this.End };
            }
            if (radius == 1)
            {
                List<Vertex> result = new List<Vertex>();
                HEEdge edge = next;
                do
                {
                    result.Add(edge.end);
                    edge = edge.next.pair.next;
                    if (edge.end == end || edge.end == Begin)
                    {
                        edge = edge.pair.next;
                    }
                } while (edge.end != result[0]);
                return result.ToArray();
            }
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
