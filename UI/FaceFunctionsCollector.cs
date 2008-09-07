using System;
using System.Collections.Generic;
using System.Text;

using Matveev.Mtk.Core;

namespace UI
{
    // TODO: Make generic class, move to Matveev.Common
    public class FaceFunctionsCollector : CtorCollectorBase<FaceFunction>
    {
        public string GetInfo(Face face)
        {
            StringBuilder resultBuilder = new StringBuilder();
            foreach (var item in Objects)
            {
                resultBuilder.AppendLine(item.Key);
                try
                {
                    resultBuilder.AppendLine(item.Value.Evaluate(face).ToString());
                }
                catch (Exception ex)
                {
                    resultBuilder.AppendLine(ex.Message);
                }
            }

            return resultBuilder.ToString();
        }
    }
}
