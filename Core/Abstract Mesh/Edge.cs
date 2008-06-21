// Edge.cs

using System;
using System.Collections.Generic;

namespace Matveev.Mtk.Core
{
    public abstract class Edge:MeshPart
    {
        public abstract Vertex Begin
        {
            get;
        }

        public abstract Vertex End
        {
            get;
        }

        public abstract Edge Next
        {
            get;
        }

        public abstract Edge Prev
        {
            get;
        }

        public abstract Edge Pair
        {
            get;
        }

        public abstract Face Face
        {
            get;
        }

        public override string ToString()
        {
            return string.Format("Begin=({0}), End=({1})", this.Begin, this.End);
        }
    }
}
