﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Matveev.Common;
using Matveev.Mtk.Core;
using Matveev.Mtk.Library;
using Matveev.Mtk.Library.Fields;

namespace Matveev.Mtk.Tests
{
    [TestFixture]
    public class BoundingBoxTests
    {
        private readonly BoundingBox _target = new BoundingBox(-1, 1, -1, 1, -1, 1);
        private readonly Vector[] _basis = new Vector[] {
            new Vector(1, 0, 0),
            new Vector(0, 1, 0),
            new Vector(0, 0, 1)
        };

        [Test]
        public void PlaneTest()
        {
            TestConstraints(Plane.Sample, "Plane");
        }

        [Test]
        public void SphereTest()
        {
            TestConstraints(Sphere.Sample, "Sphere");
        }

        [Test]
        public void HyperboloidTest()
        {
            TestConstraints(QuadraticForm.HyperboloidOne, "Hyperboloid");
        }

        private void TestConstraints(IImplicitSurface surface, string name)
        {
            Mesh mesh = MC.Instance.Create(HEMesh.Factory, surface, _target, 4, 4, 4);
            var forbiddenMoves = from v in mesh.Vertices
                                 from e in _basis
                                 where !_target.IsMovable(v, e)
                                 select new
                                 {
                                     Vertex = v,
                                     Direction = e
                                 };
            YamlSerializerTest.TestSerialize(name + ".BoundingBox.yaml", forbiddenMoves);
        }
    }
}
