// Face.cs

using System;
using System.Collections.Generic;

namespace Matveev.Mtk.Core
{
    public abstract class Face
    {
        public abstract IEnumerable<Vertex> Vertices
        {
            get;
        }

        public abstract IEnumerable<Edge> Edges
        {
            get;
        }

        public abstract IEnumerable<Face> Adjacent
        {
            get;
        }
        public abstract Vector Normal
        {
            get;
        }

        public override string ToString()
        {
            List<string> vertices = new List<string>();
            foreach (Vertex vertex in this.Vertices)
            {
                vertices.Add(vertex.ToString());
            }

            return "[" + string.Join(",", vertices.ToArray()) + "]";
        }
    }
}
