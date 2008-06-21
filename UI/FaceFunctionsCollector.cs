using System;
using System.Collections.Generic;
using System.Text;

using Matveev.Mtk.Core;

namespace UI
{
    public class FaceFunctionsCollector : CtorCollectorBase<FaceFunction>
    {
        public string GetInfo(Face face)
        {
            string result = string.Empty;
            foreach (string key in this.Objects.Keys)
            {
                result += key + Environment.NewLine;
                try
                {
                    result += this.Objects[key].Evaluate(face) + Environment.NewLine;
                }
                catch (Exception ex)
                {
                    result += ex.Message;
                }
            }

            return result;
        }
    }
}
