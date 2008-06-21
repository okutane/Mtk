using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace Matveev.Mtk.Library.Tests
{
    [TestFixture]
    public class ConjugateGradientsTest
    {
        [Test]
        public void Test()
        {
            MatrixBuilder mb = new MatrixBuilder(5);
            mb[0, 0] = 1;
            mb[1, 1] = 1;
            mb[2, 2] = 1;
            mb[3, 3] = 1;
            mb[4, 4] = 1;
            mb[0, 2] = 1;
            mb[2, 0] = 1;

            double[] f = new double[] { 2, 1, 2, 1, 1 };
            double[] expected = new double[] { 1, 1, 1, 1, 1 };

            int n;
            int[] ig, jg;
            double[] gg, d, x;

            mb.GetMatrix(out n, out d, out ig, out jg, out gg);

            x = new double[n];

            ConjugateGradients cg = new ConjugateGradients(5);
            cg.CG(n, ig, jg, gg, d, f, x, 100, 1e-3);

            Assert.AreEqual(expected, x);
        }
    }
}
