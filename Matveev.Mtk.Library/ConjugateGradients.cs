using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Matveev.Mtk.Library
{
    public class ConjugateGradients
    {
        private int _size;

        public ConjugateGradients(int size)
        {
            this._size = size;
        }

        private static void ArrayCopy(double[] dest, double[] src, int length)
        {
            Array.Copy(src, dest, length);
        }

        private void mult(int[] ig, int[] jg, double[] gg, double[] d, double[] A, double[] res)
        {
            int k;
            for (int i = 0; i < _size; i++)
            {
                res[i] = d[i] * A[i];
                for (int j = ig[i]; j <= ig[i + 1] - 1; j++)
                {
                    k = jg[j];
                    res[i] += gg[j] * A[k];
                    res[k] += gg[j] * A[i];
                }
            }
        }

        private static void search(int[] ig, int[] jg, int g, int w, int ita)
        {
            int i, k = 1;
            ita = 0;
            for (i = ig[g] - 1; i < ig[g + 1] - 1 && k != 0; i++)
                if (jg[i] == w)
                {
                    ita = 1;
                    k = 0;
                }
        }
        
        private void factor(int[] ig, int[] jg, double[] gg, double[] d, double[] l, double[] ld)
        {
            double sum;
            int i, j, k, ki, kol, ld1, ld2, begin, ita1 = 0, ita2 = 0;
            ld[0] = Math.Sqrt(d[0]);
            //Цикл по уголкам
            ki = 0;
            for (i = 1; i < _size; i++)
            {
                kol = ig[i + 1] - ig[i];
                for (j = ig[i]; j <= ig[i + 1] - 1; j++)
                {
                    //Накопление суммы
                    sum = 0.0;
                    ld1 = jg[ki] - ig[jg[ki] + 1] + ig[jg[ki]];
                    ld2 = i - ig[i + 1] + ig[i];
                    if (ld1 > ld2)
                        begin = ld1;
                    else
                        begin = ld2;
                    for (k = begin; k <= jg[ki] - 1; k++)
                    {
                        search(ig, jg, jg[ki], k, ita1);
                        search(ig, jg, i, k, ita2);
                        if (ita1 == 1 && ita2 == 1)
                            sum += l[ig[jg[ki] + 1] + k - jg[ki]] * l[ig[i + 1] + k - i];
                    }
                    l[j] = (gg[j] - sum) / ld[jg[ki]];
                    ki++;
                }
                //Получение диагональных элементов
                sum = 0.0;
                for (k = ig[i]; k <= ig[i + 1] - 1; k++)
                    sum += l[k] * l[k];
                ld[i] = Math.Sqrt(Math.Abs(d[i] - sum));
            }
        }
        
        private void summ(double[] A, double[] B, double[] res, double ves)
        {
            for (int i = 0; i < _size; i++)
                res[i] = A[i] + ves * B[i];
        }

        private double innerproduct(double[] A, double[] B)
        {
            double f = 0;
            for (int i = 0; i < _size; i++)
                f += A[i] * B[i];
            return f;
        }

        private void system(int[] ig, int[] jg, double[] A, double[] di, double[] pr, double[] res, int point)
        {
            if (point == 0)
            {
                res[0] = pr[0] / di[0];
                for (int i = 1; i < _size; i++)
                {
                    double f = 0.0;
                    for (int j = ig[i]; j <= ig[i + 1] - 1; j++)
                        f += res[jg[j]] * A[j];
                    res[i] = (res[i] - f) / di[i];
                }
            }
            else
            {
                ArrayCopy(res, pr, _size);
                for (int i = _size - 1; i >= 0; i--)
                {
                    res[i] = res[i] / di[i];
                    for (int k = ig[i + 1] - 1; k >= ig[i]; k--)
                        res[jg[k]] = res[jg[k]] - res[i] * A[k];
                }
            }
        }

        public void CG(int n, int[] ig, int[] jg, double[] gg, double[] d, double[] f, double[] x,
            int maxiter, double eps)
        {
            int i, k = 1;

            double[] temp;
            double normF = 0.0, f1 = 0.0, f2 = 0.0, alfa = 0.0, betta = 0.0, fnev = 0.0;
            double[] r, z, p, s, q, l, ld;
            temp = new double[_size];
            r = new double[_size];
            z = new double[_size];
            p = new double[_size];
            s = new double[_size];
            q = new double[_size];
            l = new double[ig[_size]];
            ld = new double[_size];

            normF = innerproduct(f, f);
            normF = Math.Sqrt(normF);
            factor(ig, jg, gg, d, l, ld);
            //Получение r
            mult(ig, jg, gg, d, x, r);
            summ(f, r, r, -1.0);
            system(ig, jg, l, ld, r, r, 0);
            //Получение z
            system(ig, jg, l, ld, r, z, 1);
            mult(ig, jg, gg, d, z, temp);
            ArrayCopy(z, temp, _size);
            system(ig, jg, l, ld, z, z, 0);
            system(ig, jg, l, ld, z, z, 1);
            //Получение p
            mult(ig, jg, gg, d, z, p);
            system(ig, jg, l, ld, p, p, 0);
            //Итерационный процесс
            for (i = 0; i < maxiter && k!=0; i++)
            {
                //Получение x
                f1 = innerproduct(r, p);
                f2 = innerproduct(p, p);
                alfa = f1 / f2;
                summ(x, z, x, alfa);
                //Получение r
                summ(r, p, r, -alfa);
                //Получение q
                system(ig, jg, l, ld, r, q, 1);
                mult(ig, jg, gg, d, q, temp);
                ArrayCopy(q, temp, _size);
                system(ig, jg, l, ld, q, q, 0);
                system(ig, jg, l, ld, q, q, 1);
                //Получение s	
                mult(ig, jg, gg, d, q, s);
                system(ig, jg, l, ld, s, s, 0);
                //Получение z
                f1 = innerproduct(s, p);
                betta = -f1 / f2;
                summ(q, z, z, betta);
                //Получение p
                summ(s, p, p, betta);
                //Определение относительной невязки
                f1 = innerproduct(r, r);
                f1 = Math.Sqrt(f1);
                fnev = f1 / normF;
                if (fnev < eps)
                    k = 0;
            }
        }
    }
}
