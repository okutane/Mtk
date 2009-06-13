using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using NUnit.Framework;

using Matveev.Common;
using Matveev.Mtk.Core;
using Matveev.Mtk.Library;
using Matveev.Mtk.Library.Utilities;

namespace Matveev.Mtk.Tests
{
    [TestFixture]
    public class PolygonizationTests
    {
        [Test]
        public void PolygonizationTest(
            [ValueSource("GetPolygonizers")]string polygonizerKey,
            [ValueSource("GetSurfaces")]string surfaceKey)
        {
            string filename = string.Format("{0}[{1}].yaml", surfaceKey, polygonizerKey);
            IImplicitSurfacePolygonizer polygonizer =
                InstanceCollector<IImplicitSurfacePolygonizer>.Instances[polygonizerKey];
            IImplicitSurface surface = InstanceCollector<IImplicitSurface>.Instances[surfaceKey];
            Configuration.Default.Surface = surface;
            Mesh mesh = polygonizer.Create(Configuration.Default, 4, 4, 4);
            YamlSerializerTest.TestSerialize(filename, mesh);
            YamlSerializerTest.TestSerialize(filename, mesh.Clone(null));
        }

        [Test]
        public void RegularityTest(
            [ValueSource("GetPolygonizers")]string polygonizerKey,
            [ValueSource("GetSurfaces")]string surfaceKey)
        {
            string filename = string.Format("{0}[{1}]_NonRegular.yaml", surfaceKey, polygonizerKey);
            IImplicitSurfacePolygonizer polygonizer =
                InstanceCollector<IImplicitSurfacePolygonizer>.Instances[polygonizerKey];
            IImplicitSurface surface = InstanceCollector<IImplicitSurface>.Instances[surfaceKey];
            Configuration.Default.Surface = surface;
            var internalVertices =
                polygonizer.Create(Configuration.Default, 4, 4, 4).Vertices.Where(VertexOps.IsInternal);
            YamlSerializerTest.TestSerialize(filename,
                internalVertices.Where(v => VertexOps.ExternalCurvature(v) != 0).ToArray());
        }

        private IEnumerable GetPolygonizers()
        {
            return InstanceCollector<IImplicitSurfacePolygonizer>.Instances.Keys;
        }

        private IEnumerable GetSurfaces()
        {
            return InstanceCollector<IImplicitSurface>.Instances.Keys;
        }
    }
}
