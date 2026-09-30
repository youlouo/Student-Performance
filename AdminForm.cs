using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Student_Performance
{
    public partial class AdminForm : Form

    {
        private int _hoverIndex = -1;
        public AdminForm()
        {
            InitializeComponent();
            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl1.DrawItem += tabControl1_DrawItem;
            tabControl1.MouseMove += (s, e) =>
            {
                int newHover = GetTabRectFromPoint(tabControl1, e.Location);
                if (newHover != _hoverIndex)
                {
                    _hoverIndex = newHover;
                    tabControl1.Invalidate();
                }
            };

            tabControl1.MouseLeave += (s, e) =>
            {
                _hoverIndex = -1;
                tabControl1.Invalidate();
            };
            
        }
        private int GetTabRectFromPoint(TabControl tc, Point p)
        {
            for (int i = 0; i < tc.TabCount; i++)
            {
                if (tc.GetTabRect(i).Contains(p))
                    return i;
            }
            return -1;
        }
        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tc = (TabControl)sender;
            Rectangle tabRect = tc.GetTabRect(e.Index);
            bool isSelected = e.Index == tc.SelectedIndex;
            bool isHovered = e.Index == _hoverIndex;
            Color selectedColor = Color.FromArgb(128, 0, 128);
            Color selectedColorHi = Color.FromArgb(155, 50, 170); 
            Color unselectedColor = Color.FromArgb(230, 230, 240);  
            Color hoveredColor = Color.FromArgb(180, 120, 200); 
            Color textColorActive = Color.White;
            Color textColorIdle = Color.FromArgb(60, 0, 80);
            Color backColor;
            if (isSelected)
                backColor = selectedColorHi;
            else if (isHovered)
                backColor = hoveredColor;
            else
                backColor = unselectedColor;

            using (var brush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(brush, tabRect);
            }

            if (isSelected)
            {
                using (var pen = new Pen(Color.FromArgb(90, 0, 110), 2))
                {
                    e.Graphics.DrawLine(pen,
                        tabRect.Left, tabRect.Bottom - 1,
                        tabRect.Right, tabRect.Bottom - 1);
                }
            }

            string text = tc.TabPages[e.Index].Text;

            StringFormat sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            using (var textBrush = new SolidBrush(isSelected ? textColorActive : textColorIdle))
            {
                e.Graphics.DrawString(text, tc.Font, textBrush, tabRect, sf);
            }
        }
    }
}
