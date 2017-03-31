using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library;
using Matveev.Mtk.Library.EdgeFunctions;

namespace Matveev.Mtk.ConsoleBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            Mesh mesh = MC.Instance.Create(Configuration.Default, 16, 16, 16);
            FunctionList energy = new FunctionList();
            energy.Add(EdgeLengthSquare.Instance);
            OptimizeMesh.ImproveVertexPositions(Configuration.Default, mesh.Vertices, new ConsoleProgressMonitor(), energy);
        }
    }

    class ConsoleProgressMonitor : IProgressMonitor
    {
        private int _lastValue = -1;

        #region IProgressMonitor Members

        public void ReportProgress(int value)
        {
            if(value == _lastValue)
                return;
            _lastValue = value;
            Console.WriteLine("{0}% work done.", value);
        }

        public bool IsCancelled
        {
            get
            {
                return false;
            }
        }

        #endregion
    }
}
