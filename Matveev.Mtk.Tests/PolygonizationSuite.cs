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
            Mesh mesh = polygonizer.Create(Configuration.MeshFactory, surface, -1, 1, -1, 1, -1, 1, 4, 4, 4);
            YamlSerializerTest.TestSerialize(string.Format(filename, surface, polygonizer), mesh);
            YamlSerializerTest.TestSerialize(string.Format(filename, surface, polygonizer), mesh.Clone(null));
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
