using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIPP.NIR.Controls
{
    public class SpecDataGridViewRow:DataGridViewRow
    {
        /// <summary>
        /// 光谱实体
        /// </summary>
        public Spectrum Spec { set; get; }


        protected override void Dispose(bool disposing)
        {
            if (disposing && Spec != null)
                Spec.Dispose();
            base.Dispose(disposing);
        }

    }
}
