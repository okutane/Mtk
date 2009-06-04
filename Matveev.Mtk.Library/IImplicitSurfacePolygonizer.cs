using System;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public interface IImplicitSurfacePolygonizer
    {
        Mesh Create(Configuration configuration, IImplicitSurface surface, int n, int m, int l);
    }
}
