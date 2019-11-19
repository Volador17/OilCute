using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Timers;
using System.Runtime.InteropServices;
using System.Data;
using System.Drawing;
using System;
using System.Windows.Forms;
using System.Collections;


namespace RIPP.OilDB.UI
{
    public class WinAPI
    {
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern bool DeleteObject(IntPtr hObject);
    }

    public enum MatchEntryStyle
    {
        None,
        FirstLetterInsensitive,
        FirstLetterExact,
        FirstLetterBestMatch,
        CompleteInsensitive,
        CompleteSensitive,
        ColpleteBestMatch
    }
    #region DrawSubItem, MeasureSubItem EventArgs and EventHandlers defn
    public delegate void DrawSubItemEventHandler(object sender, DrawSubItemEventArgs e);
    public class DrawSubItemEventArgs : DrawItemEventArgs
    {
        private int subIndex;
        public int SubIndex { get { return subIndex; } }

        private PropertyDescriptor boundProperty;
        public PropertyDescriptor BoundProperty { get { return boundProperty; } }

        public DrawSubItemEventArgs(Graphics graphics, Font font, Rectangle rect, int index, int subIndex, PropertyDescriptor boundProperty, DrawItemState state)
            : base(graphics, font, rect, index, state)
        {
            this.subIndex = subIndex;
            this.boundProperty = boundProperty;
        }


    }

    public delegate void MeasureSubItemEventHandler(object sender, MeasureSubItemEventArgs e);
    public class MeasureSubItemEventArgs : MeasureItemEventArgs
    {
        protected int subIndex;
        public virtual int SubIndex
        {
            get { return subIndex; }
            set { subIndex = value; }
        }

        public MeasureSubItemEventArgs(Graphics g, int index, int subIndex)
            : base(g, index)
        {
            this.subIndex = subIndex;
        }

        public MeasureSubItemEventArgs(Graphics g, int index, int subIndex, int itemHeight)
            : base(g, index, itemHeight)
        {
            this.subIndex = subIndex;
        }
    }

    #endregion



    public partial class MultiColumnListBox : ListBox
    {

        //Fields
        protected int columnCount;
        protected ColumnWidthCollection columnWidths;
        protected DrawMode drawMode;
        protected string matchBuffer;
        protected double matchBufferTimeOut;
        protected MatchEntryStyle matchEntryStyle;
        protected int textIndex;
        protected string textMember;
        protected System.Timers.Timer timer;
        protected int valueIndex;


        public MultiColumnListBox()
        {
            InitializeComponent();

            //Set Field Default Values
            matchBuffer = "";
            matchBufferTimeOut = 1000;
            matchEntryStyle = MatchEntryStyle.FirstLetterInsensitive;
            textIndex = -1;
            timer = new System.Timers.Timer();
            valueIndex = -1;

            //Base class Draw mode must always be OwnerDrawn in order for OnDrawItem to Fire
            base.DrawMode = DrawMode.OwnerDrawFixed;

            //Probably shouldn't call this a collection because it realy isn't...
            columnWidths = new ColumnWidthCollection(this);

            timer.AutoReset = false;
            timer.Interval = matchBufferTimeOut;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
        }

        public MultiColumnListBox(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        //Public Properties
        public virtual int ColumnCount
        {
            get { return columnCount; }
            set
            {
                columnCount = value;
                //if # of columns changed, the horizonal extent has probably changed too...
                recalcHorizontalExtent();
            }
        }

        public virtual new int ColumnWidth
        {
            get { return base.ColumnWidth; }
            set
            {
                base.ColumnWidth = value;
                recalcHorizontalExtent();
            }
        }

        public virtual ColumnWidthCollection ColumnWidths
        {
            get { return columnWidths; }
        }

        public virtual new object DataSource
        {
            get { return base.DataSource; }
            set
            {
                EventArgs e = new EventArgs();
                base.DataSource = value;
                OnDataSourceChanged(e);
            }
        }

        public override DrawMode DrawMode
        {
            get { return drawMode; }
            set
            {
                // The parent ListBox.DrawMode is never DrawMode.Normal 
                // so that OnDrawItem will always be called
                if (value == DrawMode.OwnerDrawVariable)
                    base.DrawMode = DrawMode.OwnerDrawVariable;
                else
                    base.DrawMode = DrawMode.OwnerDrawFixed;

                drawMode = value;
            }
        }

        public virtual double MatchBufferTimeOut
        {
            get { return matchBufferTimeOut; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("MatchBufferTimeOut", value, "MatchBufferTimeout must be > 0");
                else
                   matchBufferTimeOut = timer.Interval = value;
            }
        }

        public virtual MatchEntryStyle MatchEntryStyle
        {
            get { return matchEntryStyle; }
            set { matchEntryStyle = value; }
        }

        public override string Text
        {
            get
            {
                if (SelectedIndex < 0 || DataSource == null)
                    return "";
                else
                {
                    object tempText = GetItemAt(realTextIndex);
                    if (tempText == null)
                        return "";
                    else return tempText.ToString();
                }
            }
        }

        public virtual int TextIndex
        {
            // This is the Column  index that maps to MultiColumnListBox.Text property.
            // TextIndex Takes Precedence over Text Member
            get { return textIndex; }
            set { textIndex = value; }
        }
        public virtual string TextMember
        {
            // This is the named Column  that maps to MultiColumnListBox.Text property.
            get { return textMember; }
            set { textMember = value; }
        }

        public virtual object Value
        {
            get
            {
                if (SelectedIndex < 0 || DataSource == null)
                    return null;
                else
                    return GetItemAt(realValueIndex);
            }
        }

        public virtual int ValueIndex
        {
            get { return valueIndex; }
            set { valueIndex = value; }
        }


        //Protected Properties
        protected virtual string MatchBuffer
        {
            get { return matchBuffer; }
            set
            {
                // Reset the timer that Auto-Clears the match buffer
                // whenever its set. Effect is there has to be a full
                // Cycle (default 1sec) from the last keystroke entered
                // For the match buffer to auto-clear.
                timer.Stop();
                matchBuffer = value;
                timer.Start();
            }
        }

        protected virtual int realColumnCount
        {
            // This Read-Only internal property is here because I need to know
            // How many actual columns should be displayed. If the client code
            // sets ColumnCount to more than the DataSource provides, return the
            // DataSource. If the client code specifies ColumnCount is less
            // than DataSource provides, return columnCount; 
            get
            {
                if (DataSource == null)
                    return 0;
                else
                {
                    int dataColumnCount = DataManager.GetItemProperties().Count;
                    if (columnCount > 0 && columnCount < dataColumnCount)
                        return columnCount;
                    else
                        return dataColumnCount;
                }
            }
        }

        protected virtual int realTextIndex
        {
            // This internal read-only property returns the true index of the Text Column
            // by validating ranges and preferring TextIndex over TextMember.
            get
            {
                if (DataSource == null)
                    return 0;
                else if (textIndex >= 0 && textIndex < realColumnCount)
                    return textIndex;
                else if (textMember == "" || textMember == null)
                    return 0;
                else
                {
                    PropertyDescriptorCollection pdc = DataManager.GetItemProperties();
                    PropertyDescriptor pd = pdc.Find(textMember, true);
                    if (pd == null)
                        return 0;
                    else
                        return pdc.IndexOf(pd);
                }
            }
        }

        protected virtual int realValueIndex
        {
            get
            {
                if (this.DataSource == null)
                    return 0;
                else if (valueIndex >= 0 && valueIndex < realColumnCount)
                    return valueIndex;
                else if (ValueMember == "" || ValueMember == null)
                    return 0;
                else
                {
                    PropertyDescriptorCollection pdc = DataManager.GetItemProperties();
                    PropertyDescriptor pd = pdc.Find(ValueMember, true);
                    if (pd == null)
                        return 0;
                    else
                        return pdc.IndexOf(pd);
                }
            }
        }



        //Public Methods
        public virtual new int FindString(string s)
        {
            return findString(s, 0, realTextIndex, false);
        }
        public virtual new int FindString(string s, int startIndex)
        {
            return findString(s, startIndex, realTextIndex, false);
        }
        public virtual int FindString(string s, int startIndex, int columnIndex)
        {
            return findString(s, startIndex, columnIndex, false);
        }

        public virtual new int FindStringExact(string s)
        {
            return findString(s, 0, realTextIndex, true);
        }

        public virtual new int FindStringExact(string s, int startIndex)
        {
            return findString(s, startIndex, realTextIndex, true);
        }

        public virtual int FindStringExact(string s, int startIndex, int columnIndex)
        {
            return findString(s, startIndex, columnIndex, true);
        }
        public object GetItemAt(int rowIndex, int columnIndex)
        {
            if (DataSource == null)
                throw new InvalidOperationException("DataSource Must be set to call GetItemAt()");
            else if (Items.Count < 1)
                throw new InvalidOperationException("There must be at least one Item in the list to call GetItemAt()");
            else if (rowIndex >= Items.Count || rowIndex < 0)
                throw new ArgumentOutOfRangeException("rowIndex", rowIndex, "rowIndex must be > 0 and < Items.Count");
            else
            {
                PropertyDescriptorCollection pdc = DataManager.GetItemProperties();
                if (columnIndex < 0 || columnIndex >= pdc.Count)
                    throw new ArgumentOutOfRangeException("columnIndex", columnIndex, "columnIndex must be > 0 and < Items.Count");
                else
                    return pdc[columnIndex].GetValue(Items[rowIndex]);
            }
        }

        public object GetItemAt(int rowIndex, string columnName)
        {
            if (DataSource == null)
                throw new InvalidOperationException("DataSource Must be set to call GetItemAt()");
            else
            {
                PropertyDescriptorCollection pdc = DataManager.GetItemProperties();
                PropertyDescriptor pd = pdc.Find(columnName, true);
                if (pd == null)
                    throw new ArgumentException("'" + columnName + "' not found in the column collection", "columnName");
                else
                    return GetItemAt(rowIndex, pdc.IndexOf(pd));
            }
        }

        public object GetItemAt(int columnIndex)
        {
            return GetItemAt(SelectedIndex, columnIndex);
        }

        public object GetItemAt(string columnName)
        {
            return GetItemAt(SelectedIndex, columnName);
        }

        //Protected Methods
        protected virtual void defaultDraw(object value, DrawSubItemEventArgs e)
        {
            e.Graphics.DrawString(value.ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds);
        }
        protected virtual int findString(string s, int startIndex, int columnIndex, bool caseSensitive)
        {
            int currentItemIndex;
            if (startIndex < 0 || startIndex > Items.Count)
                return -1;
            else if (columnIndex < 0 || columnIndex > realColumnCount)
                throw new ArgumentOutOfRangeException("columnIndex", columnIndex, "columnIndex must be >= 0 and < # of columns");
            else if (s == null)
                throw new ArgumentNullException("s");
            else
            {
                if (caseSensitive)
                {
                    for (currentItemIndex = startIndex; currentItemIndex < Items.Count; currentItemIndex++)
                        if (this.GetItemAt(currentItemIndex, columnIndex).ToString().StartsWith(s))
                            return currentItemIndex;
                }
                else
                {
                    for (currentItemIndex = startIndex; currentItemIndex < Items.Count; currentItemIndex++)
                        if (this.GetItemAt(currentItemIndex, columnIndex).ToString().ToLower().StartsWith(s.ToLower()))
                            return currentItemIndex;
                }
                return -1;
            }

        }

        protected override void OnDataSourceChanged(EventArgs e)
        {
            recalcHorizontalExtent();
            base.OnDataSourceChanged(e);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);
            if (!this.DesignMode)
            {
                Rectangle currentBounds = e.Bounds;

                if (this.drawMode == DrawMode.Normal)
                {
                    e.DrawBackground();
                    if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                        e.DrawFocusRectangle();
                }

                PropertyDescriptorCollection pdc = this.DataManager.GetItemProperties();
                for (int currentColumnIndex = 0; currentColumnIndex < realColumnCount; currentColumnIndex++)
                {
                    currentBounds.Width = ColumnWidths[currentColumnIndex];
                    e.Graphics.SetClip(currentBounds);
                    OnDrawSubItem(new DrawSubItemEventArgs(e.Graphics, e.Font, currentBounds, e.Index, currentColumnIndex, pdc[currentColumnIndex], e.State));
                    currentBounds.X += currentBounds.Width;
                }
            }

        }

        protected virtual void OnDrawSubItem(DrawSubItemEventArgs e)
        {
            object value = this.GetItemAt(e.Index, e.SubIndex);
            if (DrawMode == DrawMode.Normal && Items.Count > 0)
            {
                if ((value as Image) != null)
                    e.Graphics.DrawImageUnscaled((Image)value, e.Bounds);
                else if ((value as Bitmap) != null)
                {
                    IntPtr hBmp = ((Bitmap)value).GetHbitmap();
                    e.Graphics.DrawImageUnscaled(Image.FromHbitmap(hBmp), e.Bounds);
                    WinAPI.DeleteObject(hBmp);
                }
                else if ((value as byte[]) != null)
                {
                    try
                    {
                        e.Graphics.DrawImageUnscaled(Image.FromStream(new MemoryStream((byte[])value)), e.Bounds);
                    }
                    catch (Exception ex)
                    {
                        defaultDraw(value, e);
                    }
                }
                else
                    defaultDraw(value, e);
            }
            else if (DrawSubItem.GetInvocationList() != null)
                DrawSubItem.DynamicInvoke(new object[] { this, e });


        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {

            switch (matchEntryStyle)
            {

                case MatchEntryStyle.FirstLetterInsensitive:
                    matchSingleKey(e.KeyChar.ToString(), false);
                    break;
                case MatchEntryStyle.FirstLetterExact:
                    matchSingleKey(e.KeyChar.ToString(), true);
                    break;
                case MatchEntryStyle.FirstLetterBestMatch:
                    if (!matchSingleKey(e.KeyChar.ToString(), true))
                        matchSingleKey(e.KeyChar.ToString(), false);
                    break;
                case MatchEntryStyle.CompleteInsensitive:
                    MatchBuffer += e.KeyChar.ToString();
                    matchString(MatchBuffer, false);
                    break;
                case MatchEntryStyle.CompleteSensitive:
                    MatchBuffer += e.KeyChar.ToString();
                    matchString(MatchBuffer, true);
                    break;
                case MatchEntryStyle.ColpleteBestMatch:
                    MatchBuffer += e.KeyChar.ToString();
                    if (!matchString(MatchBuffer, true))
                        matchString(MatchBuffer, false);
                    break;

            }
            e.Handled = true;
            base.OnKeyPress(e);

        }

        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            base.OnMeasureItem(e);
            if (!this.DesignMode)
            {
                int maxSubItemHeight = 0;
                PropertyDescriptorCollection pdc = this.DataManager.GetItemProperties();
                for (int currentColumnIndex = 0; currentColumnIndex < realColumnCount; currentColumnIndex++)
                {
                    e.ItemWidth = ColumnWidths[currentColumnIndex];
                    MeasureSubItemEventArgs msiea = new MeasureSubItemEventArgs(e.Graphics, e.Index, currentColumnIndex);
                    OnMeasureSubItem(msiea);
                    if (msiea.ItemHeight > maxSubItemHeight)
                        maxSubItemHeight = msiea.ItemHeight;
                }
                e.ItemHeight = maxSubItemHeight;
            }

        }
        protected virtual void OnMeasureSubItem(MeasureSubItemEventArgs e)
        {
            if (MeasureSubItem != null && MeasureSubItem.GetInvocationList() != null)
                MeasureSubItem.DynamicInvoke(new object[] { this, e });
        }
        protected virtual bool matchSingleKey(string s, bool caseSensitive)
        {
            int matchIndex;
            int startIndex;

            if (SelectedIndex < 0 || !this.GetItemAt(SelectedIndex, realTextIndex).ToString().StartsWith(s))
                startIndex = 0;
            else
                startIndex = SelectedIndex + 1;

            matchIndex = findString(s, startIndex, realTextIndex, caseSensitive);
            if (matchIndex >= 0)
            {
                SelectMutex(matchIndex);
                return true;
            }
            else if (SelectedIndex > 0)
            {
                matchIndex = findString(s, 0, realTextIndex, caseSensitive);
                if (matchIndex >= 0 && matchIndex != SelectedIndex)
                {
                    SelectMutex(matchIndex);
                    return true;
                }
            }
            return false;
        }

        protected virtual bool matchString(string s, bool caseSensitive)
        {
            int matchIndex;

            matchIndex = findString(s, 0, realTextIndex, caseSensitive);
            if (matchIndex >= 0)
            {
                SelectMutex(matchIndex);
                return true;
            }
            else
                return false;
        }

        protected virtual void recalcHorizontalExtent()
        {
            int he = 0;
            for (int i = 0; i < realColumnCount; i++)
                he += ColumnWidths[i];
            HorizontalExtent = he;
            Invalidate();
        }

        protected virtual void SelectMutex(int rowIndex)
        {
            if (rowIndex == SelectedIndex && SelectedIndices.Count == 1)
                return;
            else if (this.SelectionMode == SelectionMode.One)
                SelectedIndex = rowIndex;
            else if (rowIndex == 0)
                ClearSelected();
            else if (SelectedIndices.Count == 1)
            {
                int oldSelected = SelectedIndex;
                SelectedIndex = rowIndex;
                SetSelected(oldSelected, false);

            }
            else
            {
                BeginUpdate();
                ClearSelected();
                SelectedIndex = rowIndex;
                SetSelected(0, false);
                EndUpdate();
            }
        }

        protected virtual void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // This method gets called at specified intervals (default 1sec) after the MatchBuffer
            // Has been set. Used for MatchEntryStyle.CompleteXXX typematic matching.
            matchBuffer = "";
        }


        // Events
        public event DrawSubItemEventHandler DrawSubItem;
        public event MeasureSubItemEventHandler MeasureSubItem;

        // Classes
        public class ColumnWidthCollection
        {
            // The indexed class that holds the client-code defined widths
            // The Indexer will return the client defined width if it has been set
            // or the default width of MultiColumnListBox.ColumnWidth if not set.

            protected Hashtable configuredColumns;
            protected MultiColumnListBox parent;
            public ColumnWidthCollection(MultiColumnListBox parent)
            {
                configuredColumns = new Hashtable();
                this.parent = parent;
            }

            public int this[int columnIndex]
            {
                get
                {
                    if (!configuredColumns.ContainsKey(columnIndex))
                        return parent.ColumnWidth;
                    int temp = (int)configuredColumns[columnIndex];
                    if (temp < 0)
                        return parent.ColumnWidth;
                    else
                        return temp;
                }
                set
                {
                    if (columnIndex < 0)
                        throw new IndexOutOfRangeException("columnIndex Must be > 0");
                    if (!configuredColumns.ContainsKey(columnIndex))
                        configuredColumns.Add(columnIndex, value);
                    else
                        configuredColumns[columnIndex] = value;
                }
            }
        }



    }
}
