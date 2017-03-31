using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using GslNet.MultiMin;

namespace Matveev.Mtk.Library
{
    public class Parameters
    {
        private Parameters()
        {
            UseGradient = true;
            StepSize = 1;
            Tolerance = 0.1;
            Epsilon = 1e-6;
            MaxIterations = 100;
            Grows = 3;
            SurfaceRangeValue = 1;
            SurfaceNormalsValue = 1;
            NumericalDiffStepSize = 1e-3;
            DoCleanUp = false;
        }

        public int Grows
        {
            get;
            set;
        }

        public bool DoCleanUp
        {
            get;
            set;
        }

        public double NumericalDiffStepSize
        {
            get;
            set;
        }

        [Category("Energy")]
        public double SurfaceRangeValue
        {
            get;
            set;
        }

        [Category("Energy")]
        public double SurfaceNormalsValue
        {
            get;
            set;
        }

        [Category("ImproveVertexPositions")]
        public bool UseGradient
        {
            get;
            set;
        }

        [Category("MultiMin")]
        public double Epsilon
        {
            get;
            set;
        }

        [Category("MultiMin")]
        public int MaxIterations
        {
            get;
            set;
        }

        [Category("MultiMin")]
        public double StepSize
        {
            get;
            set;
        }

        [Category("MultiMin")]
        public double Tolerance
        {
            get;
            set;
        }

        [Category("MultiMin")]
        public AlgorithmWithDerivatives Algorithm
        {
            get;
            set;
        }

        public static readonly Parameters Instance = new Parameters();
    }
}
