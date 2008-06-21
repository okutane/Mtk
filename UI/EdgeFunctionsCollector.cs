using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using Matveev.Mtk.Core;

namespace UI
{
    public class EdgeFunctionsCollector : CtorCollectorBase<EdgeFunction>
    {
        public string GetInfo(Edge edge)
        {
            string result = string.Empty;
            foreach (string key in this.Objects.Keys)
            {
                result += key + Environment.NewLine;
                result += this.Objects[key].Evaluate(edge) + Environment.NewLine;
            }

            return result;
        }
    }
}
