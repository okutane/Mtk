using System;
using System.Collections.Generic;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public class EdgeSplit:EdgeTransform
    {
        private double _t;

        public EdgeSplit() : this(0.5)
        {
        }

        public EdgeSplit(double t)
        {
            this._t = t;
        }

        public override bool IsPossible(Edge edge, IVertexConstraintsProvider constraintsProvider)
        {
            return true;
        }

        public override MeshPart Execute(Edge edge)
        {
            Mesh mesh = edge.Mesh;

            Vertex v, v0, v1, v2, v3;
            Face face;
            bool unpaired = edge.Pair == null;

            v0 = edge.Begin;
            v1 = edge.End;
            v2 = edge.Next.End;
            v = mesh.AddVertex(v0.Point + this._t * (v1.Point - v0.Point),
                Vector.Normalize(this._t * v0.Normal + (1 - this._t) * v1.Normal));
            if (!unpaired)
            {
                v3 = edge.Pair.Next.End;
                mesh.DeleteFace(edge.Pair.ParentFace);
                face = mesh.CreateFace(v3, v, v0);
                face = mesh.CreateFace(v1, v, v3);
            }
            mesh.DeleteFace(edge.ParentFace);
            face = mesh.CreateFace(v0, v, v2);
            face = mesh.CreateFace(v2, v, v1);

            return v;
        }
    }
}
