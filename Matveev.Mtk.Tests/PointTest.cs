using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Tests
{
    [TestFixture]
    public class PointTest
    {
        [Test]
        public void TestAreaTo()
        {
            Assert.AreEqual(Math.Sqrt(3) / 2, new Point(1, 0, 0).AreaTo(new Point(0, -1, 0), new Point(0, 0, 1)),
                1e-5);
        }
    }
}
