using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using Matveev.Mtk.Core;

namespace UI
{
    public class EdgeFunctionsCollector : CtorCollectorBase<IEdgeFunction>
    {
        public string GetInfo(Edge edge)
        {
            string result = string.Empty;
            foreach (string key in this.Objects.Keys)
            {
                result += key + Environment.NewLine;
                try
                {
                    result += this.Objects[key].Evaluate(edge);
                }
                catch (Exception ex)
                {
                    result += ex;
                }
                result += Environment.NewLine;
            }

            return result;
        }
    }
}
