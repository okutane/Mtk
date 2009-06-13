using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Matveev.Mtk.Core;

namespace UI
{
    interface IControlBuilder
    {
        void AddButton(string text, Action action);

        void AddLabel(string text);

        void AddControl(object control);

        void AddButton(string text, Action<IProgressMonitor> action);
    }
}
