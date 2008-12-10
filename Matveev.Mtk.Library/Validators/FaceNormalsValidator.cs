using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.Validators
{
    public class FaceNormalsValidator : IMeshValidator
    {
        #region IMeshValidator Members

        public bool IsValid(Mesh mesh)
        {
            foreach (Face face in mesh.Faces)
            {
                Vector normal = face.Normal;
                foreach (Vertex vertex in face.Vertices)
                {
                    if (vertex.Normal * normal < 0.9)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        #endregion
    }
}
