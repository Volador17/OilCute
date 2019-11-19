using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIPP.OilDB.UI.GridOil.V2
{
    public class WaitingPanel : IWaitingPanel
    {
        private Label tips;
        public Control Parent { get; private set; }
        public WaitingPanel(Control control)
        {
            Parent = control;
            tips = new Label()
            {
                Text = "正在处理数据，请稍等……",
                Location = new Point(10, 10),                
                Size = new Size(300, 60),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                BackColor = Color.LightBlue,
                ForeColor = Color.Red,
                Font = new System.Drawing.Font("微软雅黑", 15, FontStyle.Bold),
                BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
                Visible = false,
            };
            if (Parent != null)
                Parent.Controls.Add(tips);
        }

        /// <summary>
        /// 是否在繁忙状态
        /// </summary>
        public bool IsBusy
        {
            get
            {
                if (tips.InvokeRequired)
                {
                    Func<bool> fun = () => IsBusy;
                    return (bool)tips.Invoke(fun);
                }
                return tips.Visible;
            }
            set
            {
                if (tips.InvokeRequired)
                {
                    Action<bool> action = (o) => IsBusy = o;
                    tips.Invoke(action, value);
                    return;
                }
                if (this.Parent == null)
                    return;
                if (value)
                {
                    tips.Location = new Point((Parent.Width - tips.Width) / 2, (Parent.Height - tips.Height) / 2);
                }
                tips.Visible = value;
                Parent.UseWaitCursor = value;
                Parent.Enabled = !value;
                Application.DoEvents();
                if (!value)
                    Parent.Focus();
            }
        }
    }

    public interface IWaitingPanel
    {
        bool IsBusy { get; set; }
    }

}
