using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Library.FaceFunctions;
using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    class MeshFunction : IFunctionWithGradient
    {
        private HeaMesh _mesh;
        private FaceFunction _faceFunction;

        public MeshFunction(HeaMesh mesh, FaceFunction faceFunction)
        {
            this._mesh = mesh;
            this._faceFunction = faceFunction;
        }

        #region IFunctionWithGradient Members

        public void GetGradient(double[] destination)
        {
            for (int i = 0; i < destination.Length; i++)
                destination[i] = 0;

            double[] localGrad = new double[9];

            foreach (var face in this._mesh.Faces)
            {
                this._faceFunction.EvaluateGradientTo(face, localGrad);

                int localIndex = 0;
                foreach (HeaVertex v in face.Vertices)
                {
                    int globalIndex = v._offset;
                    for (int k = 0; k < 3; k++)
                    {
                        destination[globalIndex++] += localGrad[localIndex++];
                    }
                }
            }
        }

        #endregion

        #region IFunction Members

        public double[] X
        {
            get
            {
                return this._mesh._a;
            }
        }

        public double Evaluate()
        {
            return this._mesh.Faces.Sum(face => this._faceFunction.Evaluate(face));
        }

        #endregion
    }
}
