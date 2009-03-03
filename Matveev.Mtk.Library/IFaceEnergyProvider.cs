using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public interface IFaceEnergyProvider
    {
        double FaceEnergy(Point[] points);

        void FaceEnergyGradient(Point[] points, Vector[] result);
    }
}
