using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public class Configuration
    {
        public ISimpleFactory<Mesh> MeshFactory = HEMesh.Factory;

        public ICollection<EdgeTransform> EdgeTransforms = new List<EdgeTransform>();

        public BoundingBox BoundingBox = new BoundingBox(-1, 1, -1, 1, -1, 1);

        public Configuration()
        {
            EdgeTransforms.Add(new EdgeCollapse(0.5));
            EdgeTransforms.Add(new EdgeCollapse(0));
            EdgeTransforms.Add(new EdgeCollapse(1));
            EdgeTransforms.Add(EdgeSwap.Instance);
            EdgeTransforms.Add(new EdgeSplit(0.5));
        }

        public static readonly Configuration Default = new Configuration();
    }
}
