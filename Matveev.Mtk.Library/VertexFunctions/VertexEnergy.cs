using System;
using System.Collections.Generic;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.VertexFunctions
{
    public class VertexEnergy : IPointsFunctionWithGradient
    {
        private VertexEnergy()
        {
        }

        #region IPointsFunctionWithGradient Members

        public double Evaluate(Point[] argument)
        {
            return argument.Length;
        }

        public void EvaluateGradient(Point[] argument, Vector[] result)
        {
            Array.Clear(result, 0, result.Length);
        }

        public double EvaluateValueWithGradient(Point[] argument, Vector[] result)
        {
            Array.Clear(result, 0, result.Length);
            return argument.Length;
        }

        public IPointSelectionStrategy PointSelectionStrategy
        {
            get
            {
                return AllVertices.Instance;
            }
        }

        #endregion

        public static readonly VertexEnergy Instance = new VertexEnergy();

        private class AllVertices : IPointSelectionStrategy
        {
            #region IPointSelectionStrategy Members

            public IEnumerable<IEnumerable<Vertex>> GetAllLinkedSets(Vertex[] vertices)
            {
                return new Vertex[][] { vertices };
            }

            #endregion

            public static readonly IPointSelectionStrategy Instance = new AllVertices();
        }
    }
}
