using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library;
using Matveev.Mtk.Library.Fields;

namespace Matveev.Mtk.Tests
{
    [TestFixture]
    public class EdgeCollapseTest
    {
        [Test]
        public void ExecuteBegin()
        {
            ExecuteWithWeight(0);
        }

        [Test]
        public void ExecuteMiddle()
        {
            ExecuteWithWeight(0.5);
        }

        [Test]
        public void ExecuteEnd()
        {
            ExecuteWithWeight(1);
        }

        private static void ExecuteWithWeight(double weight)
        {
            Mesh mesh = MC.Instance.Create(Plane.Sample, -1, 1, -1, 1, -1, 1, 3, 3, 3);
            Edge edge = (from e in mesh.Edges
                         where (Math.Abs(e.Begin.Point.X + e.End.Point.X) < 1e-4)
                            && (Math.Abs(e.Begin.Point.Y + e.End.Point.Y) < 1e-4)
                         select e).First();
            new EdgeCollapse(weight).Execute(edge);
            YamlSerializerTest.TestSerialize("EdgeCollapse_" + weight.ToString("G2",
                NumberFormatInfo.InvariantInfo) + ".yaml", mesh);
        }
    }
}
