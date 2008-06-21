// HEEdge.cs

using System;
using System.Collections.Generic;
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

        public override Face Face
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

        public override ICollection<Edge> GetEdges(int radius)
        {
            if (radius == 0)
                return new Edge[] { this };
            else
            {
                ICollection<Edge> edgesNearBegin, edgesNearEnd;
                edgesNearBegin = this.Begin.GetEdges(radius);
                edgesNearEnd = this.End.GetEdges(radius);

                return Utils.MergeCollections(edgesNearBegin, edgesNearEnd);
            }
        }

        public override ICollection<Vertex> GetVertices(int radius)
        {
            if (radius == 0)
                return new Vertex[] { this.Begin, this.End };
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
