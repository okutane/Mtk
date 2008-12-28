using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public partial class HEMesh : Mesh
    {
        private class HEMeshFactory : ISimpleFactory<Mesh>
        {
            #region ISimpleFactory<Mesh> Members

            public Mesh Create()
            {
                return new HEMesh();
            }

            #endregion
        }

        protected List<Vertex> _vertices;
        List<HEEdge> edges;
        List<HEFace> faces;
        List<HEEdge> unpairedEdges;

        public static readonly ISimpleFactory<Mesh> Factory = new HEMeshFactory();

        protected HEMesh()
        {
            this._vertices = new List<Vertex>();
            edges = new List<HEEdge>();
            faces = new List<HEFace>();
            unpairedEdges = new List<HEEdge>();
        }

        #region Inherited from Mesh

        public override IEnumerable<Vertex> Vertices
        {
            get
            {
                foreach (HEVertexBase vertex in this._vertices)
                    yield return vertex;
            }
        }

        public override IEnumerable<Edge> Edges
        {
            get
            {
                foreach (HEEdge edge in edges)
                    yield return edge;
            }
        }

        public override IEnumerable<Face> Faces
        {
            get
            {
                foreach (HEFace face in faces)
                    yield return face;
            }
        }

        public sealed override Vertex AddVertex(Point p, Vector n)
        {
            HEVertexBase vertex;

            vertex = CreateVertex();
            vertex.Point = p;
            vertex.normal = n;

            this._vertices.Add(vertex);
            return vertex;
        }

        public override void RemoveVertex(Vertex vert)
        {
            this._vertices.Remove((HEVertexBase)vert);
        }

        public override Face CreateFace(Vertex v1, Vertex v2, Vertex v3)
        {
            if (v1 == v2 || v2 == v3 || v1 == v3)
                return null;

            HEVertexBase[] vertices = new HEVertexBase[3];
            vertices[0] = (HEVertexBase)v1;
            vertices[1] = (HEVertexBase)v2;
            vertices[2] = (HEVertexBase)v3;

            foreach (HEVertexBase v in vertices)
            {
                if (v.type == VertexType.Internal)
                    throw new Exception("Creating face through internal vertex");
            }

            HEFace face = new HEFace();
            HEEdge[] newEdges = new HEEdge[3];

            for (int i = 0; i < 3; i++)
                newEdges[i] = new HEEdge(this);

            face.mainEdge = newEdges[2];
            for (int i = 0; i < 3; i++)
            {
                HEEdge edge = newEdges[i];
                HEVertexBase begin = vertices[i];

                edge.end = vertices[(i + 1) % 3];
                edge.face = face;
                edge.next = newEdges[(i + 1) % 3];

                if (begin.type == VertexType.Isolated)
                {
                    begin.outEdge = edge;
                    begin.type = VertexType.Boundary;
                }

                foreach (HEEdge unpaired in unpairedEdges)
                {
                    if (unpaired.end == begin && unpaired.Prev.End == edge.end)
                    {
                        edge.pair = unpaired;
                        unpaired.pair = edge;
                        unpairedEdges.Remove(unpaired);
                        break;
                    }
                }
            }

            foreach (HEEdge edge in newEdges)
            {
                edges.Add(edge);
                if (edge.pair == null)
                    unpairedEdges.Add(edge);
            }

            foreach (HEVertexBase v in vertices)
            {
                if (v.type == VertexType.Boundary)
                {
                    HEEdge edge = v.outEdge;
                    do
                    {
                        edge = edge.pair;
                        if (edge != null)
                            edge = edge.next;
                    }
                    while (edge != null && edge != v.outEdge);
                    if (edge != null)
                    {
                        v.type = VertexType.Internal;
                    }
                }
            }

            faces.Add(face);
            return face;
        }

        public override void DeleteFace(Face face)
        {
            HEEdge curEdge = ((HEFace)face).mainEdge;
            do
            {
                if (curEdge.end.outEdge == curEdge.next)
                {
                    curEdge.end.type = VertexType.Isolated;
                    curEdge.end.outEdge = null;
                }
                else
                {
                    if (curEdge.end.type == VertexType.Internal)
                        curEdge.end.type = VertexType.Boundary;
                }
                if (curEdge.pair != null)
                {
                    curEdge.pair.pair = null;
                    unpairedEdges.Add(curEdge.pair);
                }
                curEdge = curEdge.next;
            }
            while (curEdge != ((HEFace)face).mainEdge);

            faces.Remove((HEFace)face);
            foreach (HEEdge edge in face.Edges)
            {
                edges.Remove(edge);
                unpairedEdges.Remove(edge);
                edge.Dispose();
            }
            foreach (HEVertexBase vert in face.Vertices)
            {
                HEEdge inEdge = edges.Find(delegate(HEEdge edge)
                {
                    return edge.end == vert;
                });
                if (inEdge != null)
                {
                    vert.outEdge = inEdge.next;
                    vert.type = VertexType.Boundary;
                }
            }
        }

        #endregion

        internal virtual HEVertexBase CreateVertex()
        {
            return new HEVertex(this);
        }
    }
}
