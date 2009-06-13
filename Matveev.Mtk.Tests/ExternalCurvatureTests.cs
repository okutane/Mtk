using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library;
using Matveev.Mtk.Library.Fields;

namespace Matveev.Mtk.Tests
{
    [TestFixture]
    public class ExternalCurvatureTests
    {
        [Test]
        public void SphereTest()
        {
            Configuration.Default.Surface = Sphere.Sample;
            Mesh mesh = MC.Instance.Create(Configuration.Default, 4, 4, 4);
            var result = from v in mesh.Vertices
                         where VertexOps.ExternalCurvature(v) != 0
                         select new
                         {
                             Vertex = v,
                             ExternalCurvature = VertexOps.ExternalCurvature(v)
                         };
            YamlSerializerTest.TestSerialize("Sphere4_NonRegular.yaml", result);
        }
    }
}
