using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Model ;
using System.Drawing;

namespace RIPP.OilDB.UI.GridOil
{
    /// <summary>
    /// 原油查询条件显示控件(包括A库，Ｂ库查询和原油应用查询)
    /// </summary>
    public partial class GridOilListView : ListView 
    {
        #region "私有变量"
        /// <summary>
        /// 输入控件
        /// </summary>
        private TextBox _textBox;
        /// <summary>
        /// 下拉菜单控件
        /// </summary>
        private ComboBox _comboBox;
        #endregion 

        #region "构造函数"
        /// <summary>
        /// 构造函数
        /// </summary>
        public GridOilListView()
        {
            InitializeComponent();
            this._textBox = new TextBox();
            this._comboBox = new ComboBox();
            this._textBox.Multiline = true;
            this._textBox.Visible = false;
            this._comboBox.Visible = false;
            this.GridLines = true;
            //this.CheckBoxes = false;
            this.FullRowSelect = true;
            this.Controls.Add(_textBox);
            this.Controls.Add(_comboBox);
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="container"></param>
        public GridOilListView(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            this._textBox = new TextBox();
            this._comboBox = new ComboBox();
            this._textBox.Multiline = true;
            this._textBox.Visible = false;
            this._comboBox.Visible = false;
            this.GridLines = true;
            //this.CheckBoxes = false;
            this.FullRowSelect = true;
            this.Controls.Add(this._textBox);
            this.Controls.Add(this._comboBox);
        }
        #endregion 

        #region "私有函数"
        /// <summary>
        /// 编辑ListViewSubItem元素
        /// </summary>
        /// <param name="subItem"></param>
        private void EditItem(ListViewItem.ListViewSubItem subItem)
        {
            if (this.SelectedItems.Count <= 0)
                return;

            Rectangle rect = subItem.Bounds;
            this._textBox.Bounds = rect;
            this._textBox.BringToFront();
            this._textBox.Text = subItem.Text;
            this._textBox.Leave += new EventHandler(tb_Leave);
            this._textBox.TextChanged += new EventHandler(m_tb_TextChanged);
            this._textBox.Visible = true;
            this._textBox.Tag = subItem;
            this._textBox.Select();
        }
        /// <summary>
        ///  编辑ListViewSubItem元素
        /// </summary>
        /// <param name="subItem"></param>
        /// <param name="rt"></param>
        private void EditItem(ListViewItem.ListViewSubItem subItem, Rectangle rt)
        {
            if (this.SelectedItems.Count <= 0)
                return;

            Rectangle rect = rt;
            this._comboBox.Bounds = rect;
            this._comboBox.BringToFront();
            this._comboBox.Items.Add(subItem.Text);
            this._comboBox.Text = subItem.Text;
            this._comboBox.Leave += new EventHandler(lstb_Leave);
            this._comboBox.TextChanged += new EventHandler(m_lstb_TextChanged);
            this._comboBox.Visible = true;
            this._comboBox.Tag = subItem;
            this._comboBox.Select();
        }
        #endregion

        #region "继承事件"
        /// <summary>
        /// 双击事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDoubleClick(EventArgs e)
        {
            Point tmpPoint = this.PointToClient(Cursor.Position);//获取SubItem的位置
            ListViewItem.ListViewSubItem subitem = this.HitTest(tmpPoint).SubItem;
            ListViewItem item = this.HitTest(tmpPoint).Item;
            if (subitem != null)
            {
                if (item.SubItems[0].Equals(subitem))
                {
                    //EditItem(subitem, new Rectangle(item.Bounds.Left, item.Bounds.Top, this.Columns[0].Width, item.Bounds.Height - 2));
                }
                else if (item.SubItems["下限"].Equals(subitem) || item.SubItems["上限"].Equals(subitem))
                {
                    EditItem(subitem);
                }
            }

            base.OnDoubleClick(e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref   Message m)
        {
            if (m.Msg == 0x115 || m.Msg == 0x114)
            {
                this._textBox.Visible = false;
            }
            base.WndProc(ref   m);
        }
        /// <summary>
        /// 键盘事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                if (this.SelectedItems.Count > 0)
                {
                    //ListViewItem lvi = this.SelectedItems[0];
                    //EditItem(lvi.SubItems[0], new Rectangle(lvi.Bounds.Left, lvi.Bounds.Top, this.Columns[0].Width, lvi.Bounds.Height - 2));
                }
            }
            base.OnKeyDown(e);
        }
        /// <summary>
        /// 下拉菜单事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            this._textBox.Visible = false;
            this._comboBox.Visible = false;
            base.OnSelectedIndexChanged(e);
        }

        #endregion 

        #region "私有事件"
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tb_Leave(object sender, EventArgs e)
        {
            this._textBox.TextChanged -= new EventHandler(m_tb_TextChanged);
            (sender as TextBox).Visible = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_tb_TextChanged(object sender, EventArgs e)
        {
            if ((sender as TextBox).Tag is ListViewItem.ListViewSubItem)
            {
                (this._textBox.Tag as ListViewItem.ListViewSubItem).Text = this._textBox.Text;
                (this._textBox.Tag as ListViewItem.ListViewSubItem).Tag = this._textBox.Text;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstb_Leave(object sender, EventArgs e)
        {
            this._comboBox.TextChanged -= new EventHandler(m_lstb_TextChanged);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_lstb_TextChanged(object sender, EventArgs e)
        {
            if ((sender as ListBox).Tag is ListViewItem.ListViewSubItem)
            {
                (this._comboBox.Tag as ListViewItem.ListViewSubItem).Text = this._comboBox.Text;
            }
        }
        #endregion 
    }
}
