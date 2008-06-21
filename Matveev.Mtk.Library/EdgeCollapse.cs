using System;
using System.Collections.Generic;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public class EdgeCollapse : EdgeTransform
    {
        private double _t;

        public EdgeCollapse() : this(0.5)
        {
        }

        public EdgeCollapse(double t)
        {
            this._t = t;
        }

        public override bool IsPossible(Edge edge)
        {
            return edge.Begin.Type != VertexType.Boundary && edge.End.Type != VertexType.Boundary;
        }

        public override MeshPart Execute(Edge edge)
        {
            Mesh mesh = edge.Mesh;

            if (edge.Pair == null)
            {
                Vertex b, e, v;
                b = edge.Begin;
                e = edge.End;
                List<Vertex> Vb, Ve, Vn;
                List<Face> F;

                Vb = new List<Vertex>();
                Ve = new List<Vertex>();
                Vn = new List<Vertex>();
                F = new List<Face>();

                foreach (Vertex vert in b.Adjacent)
                    Vb.Add(vert);
                foreach (Vertex vert in e.Adjacent)
                    Ve.Add(vert);
                foreach (Face face in b.AdjacentFaces)
                    if (face != edge.Face)
                        F.Add(face);
                foreach (Face face in e.AdjacentFaces)
                    F.Add(face);

                int k1 = Vb.FindIndex(delegate(Vertex vert)
                {
                    return vert == e;
                }) + 1;

                int k2 = Ve.FindIndex(delegate(Vertex vert)
                {
                    return vert == b;
                }) + 1;

                for (int i = 0; i < Vb.Count - 1; i++)
                    Vn.Add(Vb[i]);
                for (int i = 2; i < Ve.Count; i++)
                    Vn.Add(Ve[i]);

                foreach (Face face in F)
                    mesh.DeleteFace(face);
                mesh.RemoveVertex(b);
                mesh.RemoveVertex(e);

                v = mesh.AddVertex(b.Point + this._t * (e.Point - b.Point),
                    Vector.Normalize(this._t * b.Normal + (1 - this._t) * e.Normal));
                for (int i = 0; i < Vn.Count - 1; i++)
                {
                    Face face = mesh.CreateFace(v, Vn[i + 1], Vn[i]);
                }
                return v;
            }
            else
            {
                if ((edge.Begin.Type == VertexType.Boundary && this._t != 0)
                    || (edge.End.Type == VertexType.Boundary && this._t != 1))
                    throw new Exception("Tried to change topology");

                Vertex b, e, v;
                int k1, k2, n1, n2, n;
                b = edge.Begin;
                e = edge.End;

                List<Vertex> Vb, Ve, Vn;
                List<Face> F;
                Vb = new List<Vertex>();
                Ve = new List<Vertex>();
                Vn = new List<Vertex>();
                F = new List<Face>();
                foreach (Vertex vert in b.Adjacent)
                    Vb.Add(vert);
                foreach (Vertex vert in e.Adjacent)
                    Ve.Add(vert);
                foreach (Face face in b.AdjacentFaces)
                    if (face != edge.Face && face != edge.Pair.Face)
                        F.Add(face);
                foreach (Face face in e.AdjacentFaces)
                    F.Add(face);

                k1 = Vb.FindIndex(delegate(Vertex vert)
                {
                    return vert == e;
                }) + 1;

                k2 = Ve.FindIndex(delegate(Vertex vert)
                {
                    return vert == b;
                }) + 1;

                n1 = Vb.Count;
                n2 = Ve.Count;
                for (int i = 0; i < n1 - 2; i++)
                    Vn.Add(Vb[(k1 + i) % n1]);
                for (int i = 0; i < n2 - 2; i++)
                    Vn.Add(Ve[(k2 + i) % n2]);
                n = Vn.Count;

                v = mesh.AddVertex(b.Point + this._t * (e.Point - b.Point),
                    Vector.Normalize(this._t * b.Normal + (1 - this._t) * e.Normal));

                foreach (Face face in F)
                    mesh.DeleteFace(face);
                mesh.RemoveVertex(b);
                mesh.RemoveVertex(e);

                for (int i = 0; i < n; i++)
                {
                    Face face = mesh.CreateFace(v, Vn[(i + 1) % n], Vn[i]);
                }

                return v;
            }
        }
    }
}
