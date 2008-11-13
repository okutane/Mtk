using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library.EdgeFunctions;

namespace Matveev.Mtk.Library.Validators
{
    class DihedralAnglesValidator : IMeshValidator
    {
        private readonly double _maxValue;

        public DihedralAnglesValidator(double maxValue)
        {
            _maxValue = maxValue;
        }
        
        #region IMeshValidator Members

        public bool IsValid(Mesh mesh)
        {
            foreach (Edge edge in mesh.Edges)
            {
                if (DihedralAngle.Instance.Evaluate(edge) > _maxValue)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}
