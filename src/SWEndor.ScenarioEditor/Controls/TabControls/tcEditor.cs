using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SWEndor.ScenarioEditor
{
  public partial class tcEditor : TabControl
  {
    public tcEditor()
    {
      InitializeComponent();
      //Padding = new Point(10, 3);
    }

    Bitmap bclose = new Bitmap(Properties.Resources._16x16button);
    string rightpad = "      ";
    Func<Rectangle, int> b_right => (r) => r.Right - bclose.Width - 2;
    Func<Rectangle, int> b_top => (r) => r.Top + 2; //(r.Height - bclose.Height) / 2;
    Func<Rectangle, int> b_width => (r) => bclose.Width;
    Func<Rectangle, int> b_height => (r) => bclose.Height;

    Queue<tpEditor> q_edit = new Queue<tpEditor>();

    private void tcEditor_DrawItem(object sender, DrawItemEventArgs e)
    {
      //Increase space
      TabPage p = TabPages[e.Index];
      Rectangle r = GetTabRect(e.Index);

      if (!p.Text.EndsWith(rightpad))
        p.Text = p.Text + rightpad;

      //Render close button 
      e.Graphics.DrawImage(bclose, b_right.Invoke(e.Bounds), b_top.Invoke(e.Bounds));
      TextRenderer.DrawText(e.Graphics, p.Text, p.Font, Padding + new Size(r.Location), p.ForeColor, TextFormatFlags.Left);

      e.DrawFocusRectangle();
    }

    private void tcEditor_MouseUp(object sender, MouseEventArgs e)
    {
      for (int i = 0; i < TabPages.Count; i++)
      {
        Rectangle r = GetTabRect(i);
        Rectangle closeButton = new Rectangle(b_right.Invoke(r), b_top.Invoke(r), b_width.Invoke(r), b_height.Invoke(r));
        if (closeButton.Contains(e.Location))
        {
          tpEditor tp = TabPages[i] as tpEditor;
          if (tp != null)
            if (tp.Close())
              Remove(tp);

          break;
        }
      }
    }

    public void Add(tpEditor p)
    {
      TabPages.Add(p);
    }

    public void Remove(tpEditor p)
    {
      TabPages.Remove(p);
      p.Dispose();
    }
  }
}
