using System.ComponentModel;

using Matveev.Mtk.Core;

namespace UI
{
    class BackgroundWorkerProgressMonitor : IProgressMonitor
    {
        private readonly BackgroundWorker _worker;

        public BackgroundWorkerProgressMonitor(BackgroundWorker worker)
        {
            _worker = worker;
        }

        #region IProgressMonitor Members

        public void ReportProgress(int value)
        {
            _worker.ReportProgress(value);
        }

        public bool IsCancelled
        {
            get
            {
                return _worker.CancellationPending;
            }
        }

        #endregion
    }
}
