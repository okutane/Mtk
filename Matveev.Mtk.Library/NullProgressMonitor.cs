using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public class NullProgressMonitor : IProgressMonitor
    {
        #region IProgressMonitor Members

        public void ReportProgress(int value)
        {
        }

        public bool IsCancelled
        {
            get
            {
                return false;
            }
        }

        #endregion

        public static readonly NullProgressMonitor Instance = new NullProgressMonitor();
    }
}
