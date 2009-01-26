using System;
using System.Collections.Generic;
using System.Linq;

namespace Matveev.Mtk.Core
{
    public abstract class Face : MeshPart
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

        public override Mesh Mesh
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override Vertex[] GetVertices(int radius)
        {
            if (radius == 0)
            {
                return Vertices.ToArray();
            }
            throw new NotImplementedException();
        }

        public override Edge[] GetEdges(int radius)
        {
            throw new NotImplementedException();
        }
    }
}
