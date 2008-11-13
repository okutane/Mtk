using System;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public interface IImplicitSurfacePolygonizer
    {
        Mesh Create(ISimpleFactory<Mesh> factory, IImplicitSurface surface, 
            double x0, double x1, double y0, double y1, double z0, double z1,
            int n, int m, int l);
    }
}
