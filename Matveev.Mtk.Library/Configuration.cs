using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public class Configuration
    {
        public static readonly ISimpleFactory<Mesh> MeshFactory = HEMesh.Factory;
    }
}
