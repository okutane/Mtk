using System;
using System.Collections.Generic;
using System.Text;

namespace Matveev.Mtk.Core
{
    public abstract class Energy
    {
        public abstract double Eval(Mesh mesh);
    }
}
