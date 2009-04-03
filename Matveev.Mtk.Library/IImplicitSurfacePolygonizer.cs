using System;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public interface IImplicitSurfacePolygonizer
    {
        Mesh Create(ISimpleFactory<Mesh> factory, IImplicitSurface surface, 
            BoundingBox polygonizationRegion, int n, int m, int l);
    }
}
