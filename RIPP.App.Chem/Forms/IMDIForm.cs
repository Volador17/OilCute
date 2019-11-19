using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIPP.App.Chem.Forms
{
    public interface IMDIForm
    {
        ToolStrip Tool {  get; }

        StatusStrip Status {  get; }

    }
}
