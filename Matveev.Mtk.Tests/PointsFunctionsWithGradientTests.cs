using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library;
using Matveev.Mtk.Library.FaceFunctions;
using Matveev.Mtk.Library.Fields;

namespace Matveev.Mtk.Tests
{
    [TestFixture]
    public class PointsFunctionsWithGradientTests
    {
        [Test]
        public static void TestAreaSquare()
        {
            IPointsFunctionWithGradient testedFunction = AreaSquare.Instance;
            GradientDelegate<Point, Vector> testedGradient = testedFunction.EvaluateGradient;
            GradientDelegate<Point, Vector> numericGradient =
                LocalGradientProvider.GetNumericalGradient2(testedFunction.Evaluate, 1e-5);
            Configuration.Default.Surface = CompactQuadraticForm.Sphere;
            Mesh mesh = MC.Instance.Create(Configuration.Default, 4, 4, 4);
            foreach (Face face in mesh.Faces)
            {
                Point[] points = face.Vertices.Select(v => v.Point).ToArray();
                Vector[] expected = new Vector[3];
                Vector[] actual = new Vector[3];
                numericGradient(points, expected);
                testedGradient(points, actual);
                for (int i = 0; i < 3; i++)
                {
                    Assert.AreEqual(0, (actual[i] - expected[i]).Norm, 1e-7);
                }
            }
        }
    }
}
