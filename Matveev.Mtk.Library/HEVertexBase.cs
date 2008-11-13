using System;
using System.Collections.Generic;
using System.Linq;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    internal abstract class HEVertexBase : Vertex
    {
        private Mesh _mesh;

        public Vector normal;
        public HEEdge outEdge;
        public VertexType type;

        public HEVertexBase(Mesh mesh)
        {
            this._mesh = mesh;

            outEdge = null;
            type = VertexType.Isolated;
        }

        #region Inherited from Vertex

        public sealed override Vector Normal
        {
            get
            {
                return normal;
            }
            set
            {
                normal = value;
            }
        }

        public sealed override VertexType Type
        {
            get
            {
                return type;
            }
        }

        public sealed override IEnumerable<Vertex> Adjacent
        {
            get
            {
                HEEdge edge = outEdge;
                if (type == VertexType.Internal)
                {
                    do
                    {
                        yield return edge.end;
                        edge = edge.pair.next;
                    }
                    while (edge != outEdge);
                }
                else if (type == VertexType.Boundary)
                {
                    /*while (edge.next.next.pair != null)
                        edge = edge.next.next.pair;
                    do
                    {
                        yield return edge.next.end;
                        edge = edge.pair.next;
                    }
                    while (edge.pair != null);
                    yield return edge.next.end;
                    yield return edge.end;*/
                    while (edge.pair != null)
                        edge = edge.pair.next;
                    do
                    {
                        yield return edge.end;
                        if (edge.next.next.pair != null)
                            edge = edge.next.next.pair;
                        else
                        {
                            yield return edge.next.end;
                            break;
                        }
                    }
                    while (true);
                }
            }
        }

        public sealed override IEnumerable<Face> AdjacentFaces
        {
            get
            {
                HEEdge edge = outEdge;
                if (type == VertexType.Internal)
                {
                    do
                    {
                        yield return edge.face;
                        edge = edge.pair.next;
                    }
                    while (edge != outEdge);
                }
                else if (type == VertexType.Boundary)
                {
                    while (edge.next.next.pair != null)
                        edge = edge.next.next.pair;
                    while (edge.pair != null)
                    {
                        yield return edge.face;
                        edge = edge.pair.next;
                    }
                    yield return edge.face;
                }
            }
        }

        public sealed override IEnumerable<Edge> OutEdges
        {
            get
            {
                HEEdge edge = outEdge;
                do
                {
                    yield return edge;
                    edge = edge.pair.next;
                }
                while (edge != outEdge);
            }
        }

        public sealed override IEnumerable<Edge> InEdges
        {
            get
            {
                if (type == VertexType.Internal)
                {
                    HEEdge edge = outEdge;
                    do
                    {
                        yield return edge.pair;
                        edge = edge.pair.next;
                    }
                    while (edge != outEdge);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        #endregion

        public sealed override Mesh Mesh
        {
            get
            {
                return this._mesh;
            }
        }

        public sealed override Edge[] GetEdges(int radius)
        {
            if (radius == 0)
                return new Edge[0];

            return OutEdges.ToArray();
        }

        public sealed override Vertex[] GetVertices(int radius)
        {
            if (radius == 0)
                return new Vertex[] { this };
            if (radius == 1)
            {
                List<Vertex> result = new List<Vertex>();
                // TODO: use State pattern here.
                if (type == VertexType.Boundary)
                {
                    HEEdge edge = outEdge;
                    while (edge.pair != null)
                    {
                        edge = edge.pair.next;
                    }
                    do
                    {
                        result.Add(edge.end);
                        if (edge.next.next.pair == null)
                        {
                            break;
                        }
                        edge = edge.next.next.pair;
                    }
                    while(true);
                    result.Add(edge.next.end);
                }
                else
                {
                    HEEdge edge = outEdge;
                    do
                    {
                        result.Add(edge.end);
                        edge = edge.next.next.pair;
                    }
                    while (edge != outEdge);
                }
                return result.ToArray();
            }
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
