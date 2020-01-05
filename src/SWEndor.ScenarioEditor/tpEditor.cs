using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SWEndor.ScenarioEditor
{
  public partial class tpEditor : TabPage
  {
    public string CurrPath = null;
    private string Cache = "";
    private IHighlighter Highlighter = NoHighlighter.Instance;

    // rtb
    private int prevSel = 0;
    private int prevLen = 0;
    private int Sel = 0;
    private int Len = 0;

    // pLineNum
    private float _z;
    private Font _f;
    private Brush _b = Brushes.DarkOliveGreen;
    private List<int> _n = new List<int>();

    public tpEditor()
    {
      InitializeComponent();
      rtb.MouseWheel += rtb_MouseWheel;
    }

    private void pLineNum_Paint(object sender, PaintEventArgs e)
    {
      pLineNum.SuspendLayout();
      int i0 = rtb.GetLineFromCharIndex(rtb.GetCharIndexFromPosition(new Point(2, 2)));
      int y0 = rtb.GetPositionFromCharIndex(rtb.GetFirstCharIndexFromLine(i0)).Y;
      _n.Clear();
      for (int i = i0; i < rtb.Lines.Length; i++)
      {
        if (rtb.GetPositionFromCharIndex(rtb.GetFirstCharIndexFromLine(i)).Y > pLineNum.Height)
          break;

        _n.Add(i + 1);
      }
      string s = string.Join(Environment.NewLine, _n);
      e.Graphics.DrawString(s, _f, _b, new PointF(2, y0));
      pLineNum.ResumeLayout();
    }

    private void rtb_VScroll(object sender, EventArgs e)
    {
      pLineNum.Invalidate();
    }

    private void rtb_SizeChanged(object sender, EventArgs e)
    {
      pLineNum.Invalidate();
    }

    private void rtb_MouseWheel(object sender, MouseEventArgs e)
    {
      // non-zoom mouse wheel are handled by VScroll
      if (_z != rtb.Font.Size * rtb.ZoomFactor)
      {
        pLineNum.Width = (int)(36 * rtb.ZoomFactor);
        _z = rtb.Font.Size * rtb.ZoomFactor;
        _f = new Font(rtb.Font.FontFamily, rtb.Font.Size * rtb.ZoomFactor);
        pLineNum.Invalidate();
      }
    }

    private void rtb_TextChanged(object sender, EventArgs e)
    {
      if (rtb._ignore)
        return;

      int p0 = rtb.SelectionStart;
      int p1 = rtb.SelectionLength;

      //UpdateTitle();
      rtb._Paint = false;
      rtb.SuspendLayout();
      int ln1 = rtb.GetLineFromCharIndex((prevSel < rtb.SelectionStart) ? prevSel : rtb.SelectionStart);
      int ln2 = rtb.GetLineFromCharIndex((prevSel + prevLen > rtb.SelectionStart + rtb.SelectionLength) ? prevSel + prevLen : rtb.SelectionStart + rtb.SelectionLength);

      for (int l = ln1; l <= ln2; l++)
        Highlighter.Highlight(rtb, l);
      rtb._Paint = true;
      rtb.ResumeLayout();

      rtb.SelectionStart = p0;
      rtb.SelectionLength = p1;

      pLineNum.Invalidate();
    }
  }
}
