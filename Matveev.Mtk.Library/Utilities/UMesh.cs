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

        public static Mesh CloneSub(this Mesh mesh, IEnumerable<Face> faces, IDictionary<Vertex, Vertex> vertMap,
            IDictionary<Edge, Edge> edgeMap, IDictionary<Face, Face> faceMap)
        {
            Mesh result = Configuration.MeshFactory.Create();

            vertMap = GetCleanOrNew(vertMap);
            edgeMap = GetCleanOrNew(edgeMap);
            faceMap = GetCleanOrNew(faceMap);

            List<Vertex> verts = new List<Vertex>();

            foreach (Face face in faces)
            {
                foreach (Vertex vert in face.Vertices)
                {
                    if (!vertMap.ContainsKey(vert))
                        vertMap.Add(vert, result.AddVertex(vert.Point, vert.Normal));
                    verts.Add(vertMap[vert]);
                }
                Face newFace = result.CreateFace(verts[0], verts[1], verts[2]);
                verts.Clear();
                foreach (Edge oldEdge in face.Edges)
                    foreach (Edge newEdge in newFace.Edges)
                    {
                        if (newEdge.End == vertMap[oldEdge.End])
                        {
                            edgeMap.Add(oldEdge, newEdge);
                            break;
                        }
                    }
                faceMap.Add(face, newFace);
            }

            return result;
        }

        public static Mesh Clone(this Mesh mesh, IDictionary<Edge, Edge> edgeMap)
        {
            Mesh result = Configuration.MeshFactory.Create();

            IDictionary<Vertex, Vertex> vertMap = new Dictionary<Vertex, Vertex>();
            foreach (Vertex vertex in mesh.Vertices)
            {
                vertMap.Add(vertex, result.AddVertex(vertex.Point, vertex.Normal));
            }
            edgeMap = GetCleanOrNew(edgeMap);

            foreach (Face face in mesh.Faces)
            {
                List<Vertex> verts = new List<Vertex>(3);
                foreach (Vertex vert in face.Vertices)
                {
                    verts.Add(vertMap[vert]);
                }
                Face newFace = result.CreateFace(verts[0], verts[1], verts[2]);
                foreach (Edge oldEdge in face.Edges)
                {
                    Edge newEdge = newFace.Edges.Single(edge => edge.End == vertMap[oldEdge.End]);
                    edgeMap.Add(oldEdge, newEdge);
                }
            }

            return result;
        }

        private static IDictionary<T, T> GetCleanOrNew<T>(IDictionary<T, T> dictionary)
        {
            if (dictionary == null)
            {
                return new Dictionary<T, T>();
            }
            dictionary.Clear();
            return dictionary;
        }
    }
}
