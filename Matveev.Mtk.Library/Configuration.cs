using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public static class Configuration
    {
        public static ISimpleFactory<Mesh> MeshFactory = HEMesh.Factory;

        public static ICollection<EdgeTransform> EdgeTransforms = new List<EdgeTransform>();

        public static BoundingBox BoundingBox = new BoundingBox(-1, 1, -1, 1, -1, 1);

        static Configuration()
        {
            EdgeTransforms.Add(new EdgeCollapse(0.5));
            EdgeTransforms.Add(new EdgeCollapse(0));
            EdgeTransforms.Add(new EdgeCollapse(1));
            EdgeTransforms.Add(new EdgeSwap());
            EdgeTransforms.Add(new EdgeSplit(0.5));
        }
    }
}
