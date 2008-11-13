using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Matveev.Mtk.Core
{
    public interface ISimpleFactory<T>
    {
        T Create();
    }
}
