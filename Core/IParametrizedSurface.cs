using System;
using System.Collections.Generic;
using System.Text;

namespace Matveev.Mtk.Core
{
    public interface IParametrizedSurface
    {
        Point Eval(double u, double v);

        Vector Normal(double u, double v);

        double MinU
        {
            get;
        }

        double MinV
        {
            get;
        }

        double MaxU
        {
            get;
        }

        double MaxV
        {
            get;
        }
    }
}
