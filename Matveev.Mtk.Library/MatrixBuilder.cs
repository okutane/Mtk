using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Matveev.Mtk.Library
{
    public class MatrixBuilder
    {
        private int _n = 0;

        private double[] _di;

        private Dictionary<int, double>[] _rows;

        public MatrixBuilder(int n)
        {
            this._n = n;
            this._di = new double[n];
            this._rows = new Dictionary<int, double>[n];
            for (int i = 0; i < n; i++)
            {
                this._rows[i] = new Dictionary<int, double>();
            }
        }

        public double this[int i, int j]
        {
            get
            {
                if (i == j)
                    return this._di[i];
                double value = 0;
                this._rows[i].TryGetValue(j, out value);

                return value;
            }
            set
            {
                if (i == j)
                    this._di[i] = value;
                else
                    this._rows[i][j] = value;
            }
        }

        public void GetMatrix(out int n, out double[] di, out int[] ig, out int[] jg, out double[] gg)
        {
            n = this._n;
            di = (double[])this._di.Clone();
            ig = new int[this._n + 1];
            ig[0] = 0;
            for (int i = 0; i < this._n; i++)
            {
                ig[i + 1] = ig[i] + this._rows[i].Count;
            }
            jg = new int[ig[this._n]];
            gg = new double[ig[this._n]];
            int vsi = 0;
            for (int i = 0; i < this._n; i++)
            {
                foreach (var item in this._rows[i])
                {
                    jg[vsi] = item.Key;
                    gg[vsi] = item.Value;
                    vsi++;
                }
                Array.Sort(jg, gg, ig[i], ig[i + 1] - ig[i]);
            }
        }
    }
}
