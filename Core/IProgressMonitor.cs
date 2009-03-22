using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Matveev.Mtk.Core
{
    public interface IProgressMonitor
    {
        void ReportProgress(int value);

        bool IsCancelled
        {
            get;
        }
    }
}
