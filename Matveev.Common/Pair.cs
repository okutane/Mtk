using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Matveev.Common
{
    public class Pair<T1,T2>
    {
        public readonly T1 First;
        public readonly T2 Second;

        public Pair(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }

        public override string ToString()
        {
            return string.Format("{{{0}, {1}}}", First, Second);
        }
    }
}
