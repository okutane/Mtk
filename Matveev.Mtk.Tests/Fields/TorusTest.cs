using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library.Fields;

namespace Matveev.Mtk.Library.Tests.Fields
{
    [TestFixture]
    public class TorusTest : ImplicitParametrizedTest
    {
        [SetUp]
        public void SetUp()
        {
            Torus thorus = Torus.Sample;
            this._implicitSurface = thorus;
            this._parametrizedSurface = thorus;
        }

        [Test]
        public void Normal()
        {
            Assert.AreEqual(new Vector(1, 0, 0), this._parametrizedSurface.Normal(0, 0));
            Assert.AreEqual(new Vector(0, 0, 1), this._parametrizedSurface.Normal(0, Math.PI / 2));
        }
    }
}
