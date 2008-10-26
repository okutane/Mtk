using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public class EdgeCollapse : EdgeTransform
    {
        private readonly double _weight;

        public EdgeCollapse() : this(0.5)
        {
        }

        public EdgeCollapse(double weight)
        {
            this._weight = weight;
        }

        public override bool IsPossible(Edge edge)
        {
            if (edge.Begin.Type == VertexType.Boundary || edge.End.Type == VertexType.Boundary)
            {
                return false;
            }
            IEnumerable<Vertex> union =
                edge.Begin.Adjacent.Union(edge.End.Adjacent);
            IEnumerable<Vertex> intersection =
                edge.Begin.Adjacent.Intersect(edge.End.Adjacent);
            return union.Count() != 4 && intersection.Count() == 2;
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

                Vb = new List<Vertex>(b.Adjacent);
                Ve = new List<Vertex>(e.Adjacent);
                Vn = new List<Vertex>();
                F = new List<Face>(b.AdjacentFaces.Where(face => face != edge.Face));
                F.AddRange(e.AdjacentFaces);

                Vn.AddRange(Vb.Take(Vb.Count - 1));
                Vn.AddRange(Ve.Skip(2));

                F.ForEach(face => mesh.DeleteFace(face));
                mesh.RemoveVertex(b);
                mesh.RemoveVertex(e);

                v = mesh.AddVertex(b.Point + this._weight * (e.Point - b.Point),
                    Vector.Normalize(this._weight * b.Normal + (1 - this._weight) * e.Normal));
                for (int i = 0; i < Vn.Count - 1; i++)
                {
                    mesh.CreateFace(v, Vn[i + 1], Vn[i]);
                }
                return v;
            }
            else
            {
                if ((edge.Begin.Type == VertexType.Boundary && _weight != 0)
                    || (edge.End.Type == VertexType.Boundary && _weight != 1))
                {
                    throw new ArgumentException("Tried to change topology");
                }

                Vertex b, e, v;
                int k1, k2, n1, n2, n;
                b = edge.Begin;
                e = edge.End;

                List<Vertex> Vb, Ve, Vn;
                List<Face> F;
                Vb = new List<Vertex>(b.Adjacent);
                Ve = new List<Vertex>(e.Adjacent);
                Vn = new List<Vertex>();
                F = new List<Face>(b.AdjacentFaces.Where(f => f != edge.Face && f != edge.Pair.Face));
                F.AddRange(e.AdjacentFaces);

                k1 = Vb.IndexOf(e) + 1;
                k2 = Ve.IndexOf(b) + 1;

                n1 = Vb.Count;
                n2 = Ve.Count;
                for (int i = 0; i < n1 - 2; i++)
                {
                    Vn.Add(Vb[(k1 + i) % n1]);
                }
                for (int i = 0; i < n2 - 2; i++)
                {
                    Vn.Add(Ve[(k2 + i) % n2]);
                }
                n = Vn.Count;

                v = mesh.AddVertex(b.Point.Interpolate(e.Point, this._weight),
                    Vector.Normalize(this._weight * b.Normal + (1 - this._weight) * e.Normal));

                F.ForEach(face => mesh.DeleteFace(face));
                mesh.RemoveVertex(b);
                mesh.RemoveVertex(e);

                for (int i = 0; i < n; i++)
                {
                    mesh.CreateFace(v, Vn[(i + 1) % n], Vn[i]);
                }

                return v;
            }
        }
    }
}
