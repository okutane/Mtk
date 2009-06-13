using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Common.Utilities;
using Matveev.Mtk.Core;
using Matveev.Mtk.Library.Utilities;

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

        public override bool IsPossible(Edge edge, IVertexConstraintsProvider constraintsProvider)
        {
            if (edge.Begin.Type == VertexType.Boundary && _weight != 0)
            {
                if (!constraintsProvider.IsMovable(edge.Begin, edge.Begin.Point - edge.End.Point))
                    return false;
            }
            if (edge.End.Type == VertexType.Boundary && _weight != 1)
            {
                if (!constraintsProvider.IsMovable(edge.End, edge.Begin.Point - edge.End.Point))
                    return false;
                return false;
            }
            if (edge.Begin.Type == VertexType.Boundary && edge.End.Type == VertexType.Boundary)
            {
                return edge.Pair == null;
            }
            IEnumerable<Vertex> union =
                edge.Begin.Adjacent.Union(edge.End.Adjacent);
            IEnumerable<Vertex> intersection =
                edge.Begin.Adjacent.Intersect(edge.End.Adjacent);
            if (union.Count() == 4 || intersection.Count() != 2)
            {
                return false;
            }

            Point possibleResult = edge.Begin.Point.Interpolate(edge.End.Point, _weight);
            Vector normal = Vector.Normalize(_weight * edge.Begin.Normal + (1 - _weight) * edge.End.Normal);
            Plane3d plane = new Plane3d(possibleResult, normal);
            Vertex[] vertices;
            if (edge.Begin.Type == VertexType.Boundary)
            {
                return true;
                vertices = edge.End.GetVertices(1);
            }
            else if (edge.End.Type == VertexType.Boundary)
            {
                return true;
                vertices = edge.Begin.GetVertices(1);
            }
            else
            {
                return true;
                vertices = edge.GetVertices(1);
            }
            var uv = vertices.Select(v => plane.Trace(new Ray(v.Point, normal))).ToList();
            for (int i = 0; i < uv.Count; i++)
            {
                var p0 = uv[i];
                var p1 = uv[(i + 1) % uv.Count];
                /* Here we need to check that possibleResult is visible from any point of {uv} polygon,
                 * to check this we're checking that possibleResult is located on the left side of all
                 * {uv} polygon sides. possibleResult coordinates on the plane is always (0, 0) so they
                 * aren't present in code anythere. To perform side check we only evaluating z coordinate
                 * of normal for (0, p0, p1) triangle and comparing it to the zero. */
                if (p0.X * p1.Y - p0.Y * p1.X < 0)
                {
                    return false;
                }
            }
            return true;
        }

        public override MeshPart Execute(Edge edge)
        {
            Mesh mesh = edge.Mesh;

            if (edge.Pair == null)
            {
                Vertex result = edge.End;
                Vertex[] fan = edge.Begin.GetVertices(1);
                edge.Begin.AdjacentFaces.ToList().ForEach(mesh.DeleteFace);
                mesh.RemoveVertex(edge.Begin);
                mesh.CreateFan(fan);
                result.Point = edge.Begin.Point.Interpolate(edge.End.Point, _weight);
                result.Normal = Vector.Normalize(_weight * edge.Begin.Normal + (1 - _weight) * edge.End.Normal);
                return result;
            }
            else
            {
                if ((edge.Begin.Type == VertexType.Boundary && _weight != 0)
                    || (edge.End.Type == VertexType.Boundary && _weight != 1))
                {
                    throw new ArgumentException("Tried to change topology");
                }

                Vertex result;
                Vertex removed;
                if (edge.End.Type == VertexType.Boundary)
                {
                    result = edge.End;
                    removed = edge.Begin;
                }
                else
                {
                    result = edge.Begin;
                    removed = edge.End;
                }
                result.Point = edge.Begin.Point.Interpolate(edge.End.Point, _weight);
                result.Normal = Vector.Normalize(_weight * edge.Begin.Normal + (1 - _weight) * edge.End.Normal);
                Vertex[] vertices = removed.GetVertices(1);
                int index = Array.IndexOf(vertices, result);
                Vertex[] fan = new Vertex[vertices.Length];
                Array.Copy(vertices, index, fan, 0, vertices.Length - index);
                Array.Copy(vertices, 0, fan, vertices.Length - index, index);
                removed.AdjacentFaces.ToList().ForEach(mesh.DeleteFace);
                mesh.RemoveVertex(removed);
                mesh.CreateFan(fan);
                return result;
            }
        }

        public override string ToString()
        {
            return base.ToString() + '(' + _weight + ')';
        }
    }
}
