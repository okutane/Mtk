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
            StepSize = 1;
            Tolerance = 0.1;
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
