// Vertex.cs

using System;
using System.Collections.Generic;

namespace Matveev.Mtk.Core
{
    public abstract class Vertex : MeshPart
    {
        public abstract Point Point
        {
            get;
            set;
        }

        public abstract Vector Normal
        {
            get;
            set;
        }

        public abstract VertexType Type
        {
            get;
        }

        public abstract IEnumerable<Vertex> Adjacent
        {
            get;
        }

        public abstract IEnumerable<Face> AdjacentFaces
        {
            get;
        }

        public abstract IEnumerable<Edge> OutEdges
        {
            get;
        }

        public abstract IEnumerable<Edge> InEdges
        {
            get;
        }

        public override string ToString()
        {
            return Point.ToString()+" "+this.Type;
        }
    }

    public enum VertexType
    {
        Isolated,
        Boundary,
        Internal,
    }
}
