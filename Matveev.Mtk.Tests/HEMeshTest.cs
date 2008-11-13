using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library;

namespace Matveev.Mtk.Tests
{
    [TestFixture]
    public class HEMeshTest : MeshTest
    {
        protected override ISimpleFactory<Mesh> Factory
        {
            get
            {
                return HEMesh.Factory;
            }
        }
    }
}
