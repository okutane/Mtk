using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Library;
using Matveev.Mtk.Library.VertexFunctions;

namespace Matveev.Mtk.Tests
{
    [TestFixture]
    public class VertexOpsTests
    {
        [Test]
        public static void TestLineIntersectionTest2d()
        {
            double smallValue = 1e-5;
            Assert.IsTrue(Regularity.TestBounds(0.5));
            Assert.IsTrue(Regularity.TestBounds(smallValue));
            Assert.IsTrue(Regularity.TestBounds(1 - smallValue));
            Assert.IsFalse(Regularity.TestBounds(-smallValue));
            Assert.IsFalse(Regularity.TestBounds(1 + smallValue));
        }
    }
}
