using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.Utilities
{
    public static class UMesh
    {
        public static void CreateFan(this Mesh mesh, params Vertex[] vertices)
        {
            for (int i = 1; i < vertices.Length - 1; i++)
            {
                mesh.CreateFace(vertices[0], vertices[i], vertices[i + 1]);
            }
        }

        public static void CreateClosedFan(this Mesh mesh, params Vertex[] vertices)
        {
            mesh.CreateFan(vertices);
            mesh.CreateFace(vertices[0], vertices[vertices.Length - 1], vertices[1]);
        }
    }
}
