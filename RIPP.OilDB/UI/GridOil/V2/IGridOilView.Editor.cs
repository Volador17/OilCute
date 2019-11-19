using RIPP.OilDB.UI.GridOil.V2.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIPP.OilDB.UI.GridOil.V2
{
    partial class IGridOilView<TOilInfo, TOilData> : IGridOilEditor
    {

        public void Redo()
        {
            if (CheckCommandState().HasFlag(GridOilEditorCommandType.Redo) == false)
                return;
            this.menuCellRedo.PerformClick();
        }

        public void Undo()
        {
            if (CheckCommandState().HasFlag(GridOilEditorCommandType.Undo) == false)
                return;
            this.menuCellUndo.PerformClick();
        }

        public void Cut()
        {
            if (CheckCommandState().HasFlag(GridOilEditorCommandType.Cut) == false)
                return;
            this.menuCellCut.PerformClick();
        }

        public void Copy()
        {
            if (CheckCommandState().HasFlag(GridOilEditorCommandType.Copy) == false)
                return;
            this.menuCellCopy.PerformClick();
        }

        public void Paste()
        {
            if (CheckCommandState().HasFlag(GridOilEditorCommandType.Paste) == false)
                return;
            this.menuCellPaste.PerformClick();
        }

        public void Empty()
        {
            if (CheckCommandState().HasFlag(GridOilEditorCommandType.Empty) == false)
                return;
            this.menuCellEmpty.PerformClick();
        }

        public void AddColumn(bool isLeft)
        {
            if (CheckCommandState().HasFlag(GridOilEditorCommandType.AddColumn) == false)
                return;
            if (isLeft)
                this.menuColumnAddOnLeft.PerformClick();
            else
                this.menuColumnAddOnRight.PerformClick();
        }

        public void DeleteColumn()
        {
            if (CheckCommandState().HasFlag(GridOilEditorCommandType.DeleteColumn) == false)
                return;
            this.menuColumnDelete.PerformClick();
        }

        public void Lab2Cal()
        {
            if (CheckCommandState().HasFlag(GridOilEditorCommandType.Lab2Cal) == false)
                return;
            this.menuCellLabToCalc.PerformClick();
        }
        public void ClearLinkTips()
        {
            if (CheckCommandState().HasFlag(GridOilEditorCommandType.ClearLinkTips) == false)
                return;
            this.menuCellClearLinkTip.PerformClick();
        }
        private bool CheckToolStripMenuItemState(ToolStripMenuItem m)
        {
            return m != null && m.Enabled;
        }

        public GridOilEditorCommandType CheckCommandState()
        {
            var t = new CancelEventArgs();
            contextMenuColumn_Opening(this, t);
            if (t.Cancel)
                return GridOilEditorCommandType.None;

            GridOilEditorCommandType cmd = GridOilEditorCommandType.None;
            if (CheckToolStripMenuItemState(menuColumnAddOnLeft))
                cmd = cmd | GridOilEditorCommandType.AddColumn;
            if (CheckToolStripMenuItemState(menuColumnDelete))
                cmd = cmd | GridOilEditorCommandType.DeleteColumn;
            if (CheckToolStripMenuItemState(menuCellRedo))
                cmd = cmd | GridOilEditorCommandType.Redo;
            if (CheckToolStripMenuItemState(menuCellUndo))
                cmd = cmd | GridOilEditorCommandType.Undo;
            if (CheckToolStripMenuItemState(menuCellCopy))
                cmd = cmd | GridOilEditorCommandType.Copy;
            if (CheckToolStripMenuItemState(menuCellPaste))
                cmd = cmd | GridOilEditorCommandType.Paste;
            if (CheckToolStripMenuItemState(menuCellSave))
                cmd = cmd | GridOilEditorCommandType.Save;

            if (CheckToolStripMenuItemState(menuCellCut))
                cmd = cmd | GridOilEditorCommandType.Cut;

            if (CheckToolStripMenuItemState(menuCellEmpty))
                cmd = cmd | GridOilEditorCommandType.Empty;

            if (CheckToolStripMenuItemState(menuCellLabToCalc))
                cmd = cmd | GridOilEditorCommandType.Lab2Cal;
            if (CheckToolStripMenuItemState(menuCellClearLinkTip))
                cmd = cmd | GridOilEditorCommandType.ClearLinkTips;
            return cmd;
        }



    }
}
