using System;
using System.Collections.Generic;
using System.Diagnostics;

using NUnit.Framework;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library.Fields;

namespace Matveev.Mtk.Benchmark
{
    [TestFixture]
    public class FaceDistanceTest
    {
        [Test]
        public void CompactQuadraticFormSphere()
        {
            CompactQuadraticForm measured = CompactQuadraticForm.Sphere;
            Point[] points = new Point[3];
            Stopwatch stopwatch = new Stopwatch();
            for (int run = 0; run < 10; run++)
            {
                stopwatch.Reset();
                stopwatch.Start();
                for (int i = 0; i < 1000000; i++)
                {
                    measured.FaceEnergy(points);
                }
                stopwatch.Stop();
                Console.WriteLine("Run {0} finished after {1} ms.", run, stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
