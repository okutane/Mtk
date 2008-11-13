using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library.Validators;

namespace Matveev.Mtk.Tests
{
    static class UMeshTestHelper
    {
        public static void Validate(this Mesh mesh)
        {
            List<IMeshValidator> validators = new List<IMeshValidator>();
            validators.Add(new IsolatedVerticesValidator());
            validators.Add(new DuplicatedEdgesValidator());
            foreach (IMeshValidator validator in validators)
            {
                Assert.IsTrue(validator.IsValid(mesh), validator.GetType().Name);
            }
        }

        public static Vertex FindVertex(Mesh mesh, double x, double y)
        {
            return mesh.Vertices.Single(vertex => vertex.Point == new Point(x, y, 0));
        }
    }
}
