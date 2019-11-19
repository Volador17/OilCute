using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.NIR;

namespace RIPP.App.Chem.Forms.Predicter.Controls
{
    public abstract class IPanel : TabPage
    {
        public abstract void Clear();

        public abstract void Predict(List<string> files, object model,int numofId);

       // public abstract void Predict(Spectrum s,object model);

        protected abstract void addRow(object r, Spectrum s,int rowNum,int numofId);
    }
}
