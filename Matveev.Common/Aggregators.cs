using System;
using System.Collections.Generic;
using System.Text;

namespace Matveev.Common
{
    public interface IAggregator<T>
    {
        void Init();

        void Aggregate(T arg);

        T Result
        {
            get;
        }
    }

    public static class Aggregators
    {
        private class SumAggregator : IAggregator<double>
        {
            private double _result;

            #region IAggregator<double> Members

            public void Init()
            {
                this._result = 0;
            }

            public void Aggregate(double arg)
            {
                this._result += arg;
            }

            public double Result
            {
                get
                {
                    return this._result;
                }
            }

            #endregion
        }

        private static IAggregator<double> _sum = new SumAggregator();

        public static IAggregator<double> Sum
        {
            get
            {
                return Aggregators._sum;
            }
        }
    }
}
