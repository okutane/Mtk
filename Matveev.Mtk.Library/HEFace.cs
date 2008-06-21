using System;
using System.Collections.Generic;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    internal sealed class HEFace : Face
    {
        public HEEdge mainEdge;

        public HEFace()
        {
            mainEdge = null;
        }

        #region Inherited from Face
        public override IEnumerable<Vertex> Vertices
        {
            get
            {
                HEEdge edge = mainEdge;
                do
                {
                    yield return edge.end;
                    edge = edge.next;
                }
                while (edge != mainEdge);
            }
        }

        public override IEnumerable<Edge> Edges
        {
            get
            {
                HEEdge edge = mainEdge;
                do
                {
                    yield return edge;
                    edge = edge.next;
                }
                while (edge != mainEdge);
            }
        }

        public override IEnumerable<Face> Adjacent
        {
            get
            {
                HEEdge edge = mainEdge;
                do
                {
                    yield return edge.pair.face;
                    edge = edge.next;
                }
                while (edge != mainEdge);
            }
        }

        public override Vector Normal
        {
            get
            {
                Point p0, p1, p2;
                p0 = mainEdge.end.Point;
                p1 = mainEdge.next.end.Point;
                p2 = mainEdge.next.next.end.Point;
                Vector v1, v2;
                v1 = p1 - p0;
                v2 = p2 - p0;
                return Vector.Normalize(v2 ^ v1);
            }
        }
        #endregion
    }
}
