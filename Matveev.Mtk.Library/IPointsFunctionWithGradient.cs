using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public interface IPointsFunctionWithGradient
    {
        double Evaluate(Point[] argument);
        void EvaluateGradient(Point[] argument, Vector[] result);
        double EvaluateValueWithGradient(Point[] argument, Vector[] result);
        IPointSelectionStrategy PointSelectionStrategy
        {
            get;
        }
    }

    public interface IPointSelectionStrategy
    {
        IEnumerable<IEnumerable<Vertex>> GetAllLinkedSets(Vertex[] vertices);
    }

    public class FacesSelectionStrategy : IPointSelectionStrategy
    {
        private FacesSelectionStrategy()
        {
        }

        public static readonly IPointSelectionStrategy Instance = new FacesSelectionStrategy();

        #region IPointSelectionStrategy Members

        public IEnumerable<IEnumerable<Vertex>> GetAllLinkedSets(Vertex[] vertices)
        {
            return vertices.SelectMany(v => v.AdjacentFaces).Distinct().Select(f => f.Vertices);
        }

        #endregion
    }

    public class EdgesSelectionStrategy : IPointSelectionStrategy
    {
        private EdgesSelectionStrategy()
        {
        }

        public static readonly IPointSelectionStrategy Instance = new EdgesSelectionStrategy();

        #region IPointSelectionStrategy Members

        public IEnumerable<IEnumerable<Vertex>> GetAllLinkedSets(Vertex[] vertices)
        {
            var edges = vertices.SelectMany(v => v.OutEdges).Distinct(new NotOrientedEdgeComparer());
            return edges.Select(e => (IEnumerable<Vertex>)e.GetVertices(0));
        }

        #endregion
    }

    public class VertexSelectionStrategy : IPointSelectionStrategy
    {
        private VertexSelectionStrategy()
        {
        }

        #region IPointSelectionStrategy Members

        public IEnumerable<IEnumerable<Vertex>> GetAllLinkedSets(Vertex[] vertices)
        {
            return vertices.Select(v => (IEnumerable<Vertex>)new Vertex[] { v });
        }

        #endregion

        public static readonly IPointSelectionStrategy Instance = new VertexSelectionStrategy();
    }
}
