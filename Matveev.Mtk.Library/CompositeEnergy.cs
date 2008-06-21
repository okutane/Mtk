using System;
using System.Collections.Generic;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public class CompositeEnergy:Energy
    {
        private Energy[] _children;

        public CompositeEnergy(params Energy[] children)
        {
            this._children = new Energy[children.Length];
            Array.Copy(children, this._children, children.Length);
        }

        public override double Eval(Mesh mesh)
        {
            double result = 0;

            foreach (Energy child in this._children)
            {
                result += child.Eval(mesh);
            }

            return result;
        }
    }
}
