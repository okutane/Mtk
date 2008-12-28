using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using NUnit.Core;
using NUnit.Framework;

using Matveev.Common;
using Matveev.Mtk.Core;
using Matveev.Mtk.Library;
using Matveev.Mtk.Library.Utilities;

namespace Matveev.Mtk.Tests
{
    public class PolygonizationSuite
    {
        [Suite]
        public static TestSuite Suite
        {
            get
            {
                IDictionary<string, IImplicitSurface> implicitSurfaces =
                    InstanceCollector<IImplicitSurface>.Instances;
                IDictionary<string, IImplicitSurfacePolygonizer> implicitPolygonizers =
                    InstanceCollector<IImplicitSurfacePolygonizer>.Instances;

                TestSuite suite = new TestSuite("Polygonization suite");
                foreach (var surface in implicitSurfaces)
                {
                    foreach (var polygonizer in implicitPolygonizers)
                    {
                        suite.Add(new PolygonizationTest(polygonizer.Value, surface.Value,
                            string.Format("{0}[{1}]", surface.Key, polygonizer.Key)));
                    }
                }

                return suite;
            }
        }

        public class PolygonizationTest : NUnit.Core.TestCase
        {
            private IImplicitSurfacePolygonizer _polygonizer;
            private IImplicitSurface _surface;
            private string _suffix;

            public PolygonizationTest(IImplicitSurfacePolygonizer polygonizer, IImplicitSurface surface,
                string suffix) : base("", suffix)
            {
                _polygonizer = polygonizer;
                _surface = surface;
                _suffix = suffix + ".yaml";
            }

            [Test]
            public void Polygonize()
            {
                Mesh mesh = _polygonizer.Create(Configuration.MeshFactory, _surface, -1, 1, -1, 1, -1, 1, 4, 4, 4);
                YamlSerializerTest.TestSerialize(string.Format(_suffix, _surface, _polygonizer), mesh);
            }

            public override void Run(TestCaseResult result)
            {
                try
                {
                    Mesh mesh = _polygonizer.Create(Configuration.MeshFactory, _surface, -1, 1, -1, 1, -1, 1, 4, 4, 4);
                    YamlSerializerTest.TestSerialize(string.Format(_suffix, _surface, _polygonizer), mesh);
                    YamlSerializerTest.TestSerialize(string.Format(_suffix, _surface, _polygonizer),
                        mesh.Clone(null));
                    result.Success();
                }
                catch (Exception ex)
                {
                    result.Failure(ex.Message, ex.StackTrace);
                }
            }
        }
    }
}
