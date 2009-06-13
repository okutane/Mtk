using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Matveev.Common
{
    public class Pair<T1, T2>
    {
        public T1 First
        {
            get;
            set;
        }

        public T2 Second
        {
            get;
            set;
        }
    }

    public class Triple<T1, T2, T3>
    {
        public T1 First
        {
            get;
            set;
        }

        public T2 Second
        {
            get;
            set;
        }

        public T3 Third
        {
            get;
            set;
        }
    }
}
