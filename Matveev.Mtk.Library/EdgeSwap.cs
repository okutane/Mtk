using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library.EdgeFunctions;

namespace Matveev.Mtk.Library
{
    public class EdgeSwap : EdgeTransform
    {
        private EdgeSwap()
        {
        }

        public override bool IsPossible(Edge edge, IVertexConstraintsProvider constraintsProvider)
        {
            if (edge.Pair == null)
            {
                return false;
            }

            Vertex v1, v2;
            FindOtherVertices(edge, out v1, out v2);

            foreach (Vertex vertex in v1.Adjacent)
            {
                if (vertex == v2)
                {
                    return false;
                }
            }
            
            return true;
        }

        public override MeshPart Execute(Edge edge)
        {
            Mesh mesh = edge.Mesh;

            Vertex v1, v2, v3, v4;

            Face face;
            v1 = edge.End;
            v3 = edge.Begin;
            FindOtherVertices(edge, out v2, out v4);

            mesh.DeleteFace(edge.Pair.ParentFace);
            mesh.DeleteFace(edge.ParentFace);
            face = mesh.CreateFace(v1, v2, v4);
            face = mesh.CreateFace(v4, v2, v3);

            return face.Edges.First(e => e.End == v2);
        }

        private static void FindOtherVertices(Edge edge, out Vertex v2, out Vertex v4)
        {
            v2 = edge.Next.End;
            v4 = edge.Pair.Next.End;
        }

        public static readonly EdgeSwap Instance = new EdgeSwap();
    }
}
