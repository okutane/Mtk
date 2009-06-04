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
        protected override Configuration Configuration
        {
            get
            {
                return new Configuration
                {
                    MeshFactory = HEMesh.Factory
                };
            }
        }
    }
}
