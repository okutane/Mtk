using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.Tests.Fields
{
    public class ImplicitParametrizedTest
    {
        protected IImplicitSurface _implicitSurface;
        protected IParametrizedSurface _parametrizedSurface;

        const int N = 8;

        private delegate void PointCommand(double u, double v);

        private void ForEachPointOnParametrizedSurface(PointCommand command)
        {
            double minU = this._parametrizedSurface.MinU;
            double minV = this._parametrizedSurface.MinV;
            double stepU = (this._parametrizedSurface.MaxU - minU) / N;
            double stepV = (this._parametrizedSurface.MaxV - minV) / N;

            for (int i = 0; i <= N; i++)
            {
                double u = minU + stepU * i;
                for (int j = 0; j <= N; j++)
                {
                    double v = minV + stepV * j;

                    command(u, v);
                }
            }
        }

        private void PointOnImplicit(double u, double v)
        {
            Point pointOnSurface = this._parametrizedSurface.Eval(u, v);
            Assert.AreEqual(0.0, this._implicitSurface.Eval(pointOnSurface), 1e-14);
        }

        private void EqualNormals(double u, double v)
        {
            Point pointOnSurface = this._parametrizedSurface.Eval(u, v);
            Vector normal = this._parametrizedSurface.Normal(u, v);
            Vector grad = this._implicitSurface.Grad(pointOnSurface);
            Assert.AreEqual(normal, Vector.Normalize(grad));
        }

        [Test]
        public void Points()
        {
            ForEachPointOnParametrizedSurface(PointOnImplicit);
        }

        [Test]
        public void Normals()
        {
            ForEachPointOnParametrizedSurface(EqualNormals);
        }

        [Test, Ignore]
        public void FaceNormalBetweenVertexNormals()
        {
            Assert.Fail("Write test");
        }
    }
}
