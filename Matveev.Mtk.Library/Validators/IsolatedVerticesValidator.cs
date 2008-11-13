using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.Validators
{
    public class IsolatedVerticesValidator : IMeshValidator
    {
        #region IMeshValidator Members

        public bool IsValid(Mesh mesh)
        {
            return mesh.Vertices.Count(v => v.Type == VertexType.Isolated) == 0;
        }

        #endregion
    }
}
