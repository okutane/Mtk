using System;

namespace Matveev.Mtk.Core
{
    public interface IImplicitSurface
    {
        double Eval(Point p);
        Vector Grad(Point p);

        double Eval(Face p);
    }
}
