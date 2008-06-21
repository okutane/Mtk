using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Library.Fields;

namespace Matveev.Mtk.Library.Tests.Fields
{
    [TestFixture]
    public class SphereTest : ImplicitParametrizedTest
    {
        [SetUp]
        public void SetUp()
        {
            Sphere sphere = Sphere.Sample;
            this._implicitSurface = sphere;
            this._parametrizedSurface = sphere;
        }
    }
}
