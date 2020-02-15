using SWEndor.ScenarioEditor.Checker;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SWEndor.ScenarioEditor
{
  public partial class tpEditor : TabPage
  {
    private const string _new = "new";
    public string CurrPath = null;
    private string Cache = "";
    private IHighlighter Highlighter = NoHighlighter.Instance;

    // rtb
    private int Sel = 0;
    private int Len = 0;
    private int prevSel = 0;
    private int prevLen = 0;

    // pLineNum
    private float _z;
    private Font _f;
    private Brush _b = Brushes.DarkOliveGreen;
    private List<int> _n = new List<int>();

    public Action Editor_SelectionChanged;

    public tpEditor()
    {
      InitializeComponent();
      Text = _new;
      rtb.MouseWheel += rtb_MouseWheel;
      _f = rtb.Font;
      _z = rtb.Font.Size * rtb.ZoomFactor;
    }

    private void FileOpen(string path)
    {
      Cache = File.ReadAllText(path);
      Highlighter = AutoHighlight(Path.GetExtension(path));
      rtb.TextChanged -= rtb_TextChanged;
      rtb.Text = Cache;
      Highlighter.Highlight(rtb);
      rtb.TextChanged += rtb_TextChanged;
      Text = Path.GetFileName(path);
    }

    private void FileSave(string path)
    {
      File.WriteAllText(path, rtb.Text);
      Cache = rtb.Text;
      Text = Path.GetFileName(path);
    }

    public void New()
    {
      if (FileClose())
      {
        rtb.Clear();
        rtb.ClearUndo();
        CurrPath = null;
        Cache = "";
      }
    }

    public bool Open()
    {
      if (FileClose())
      {
        if (ofd.ShowDialog() == DialogResult.OK)
        {
          CurrPath = ofd.FileName;
          FileOpen(CurrPath);
          return true;
        }
      }
      return false;
    }

    public bool Close()
    {
      return FileClose();
    }

    public bool SaveAs()
    {
      if (sfd.ShowDialog() == DialogResult.OK)
      {
        CurrPath = sfd.FileName;
        FileSave(CurrPath);
        return true;
      }
      return false;
    }

    public bool QuickSave()
    {
      if (CurrPath == null)
        return SaveAs();
      else
      {
        FileSave(CurrPath);
        return true;
      }
    }

    public bool FileClose()
    {
      if (!CheckEquivalence(rtb.Text, Cache))
      {
        DialogResult d = MessageBox.Show("There are unsaved changes on this document. Save?", Globals.Title, MessageBoxButtons.YesNoCancel);
        if (d == DialogResult.Yes)
          return QuickSave();
        else if (d == DialogResult.Cancel)
          return false;
      }
      return true;
    }

    public IHighlighter AutoHighlight(string ext)
    {
      switch (ext)
      {
        case ".ini":
        case ".scen":
          return INIHighlighter.Instance;

        case ".sw":
          return ScriptHighlighter.Instance;

        default:
          return NoHighlighter.Instance;

      }
    }

    public bool IsChanged { get { return !CheckEquivalence(rtb.Text, Cache); } }

    public int Line { get { return rtb.GetLineFromCharIndex(rtb.SelectionStart) + 1; } } // one-based
    public int Column { get { return rtb.SelectionStart - rtb.GetFirstCharIndexOfCurrentLine() + 1; } } // one-based
    public int Length { get { return rtb.SelectionLength; } }

    public bool CheckEquivalence(string s1, string s2)
    {
      return s1.Replace(Environment.NewLine, "\n").Equals(s2.Replace(Environment.NewLine, "\n"));
    }

    public void Higlight(IHighlighter h)
    {
      UseWaitCursor = true;
      Highlighter = h;
      h.Highlight(rtb);
      UseWaitCursor = false;
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

      string text = Path.GetFileName(CurrPath ?? _new) + (IsChanged ? "*" : "");
      if (Text.TrimEnd() != text)
        Text = text;

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

    private void rtb_SelectionChanged(object sender, EventArgs e)
    {
      prevSel = Sel;
      prevLen = Len;
      Sel = rtb.SelectionStart;
      Len = rtb.SelectionLength;
      Editor_SelectionChanged?.Invoke();
    }

    public void DoCheck()
    {
      // dump to file
      string path = @"./temp/check.txt";
      Directory.CreateDirectory(Path.GetDirectoryName(path));
      File.WriteAllText(path, rtb.Text);

      ScriptChecker checker = new ScriptChecker(path, Output);
      rtb_output.Clear();
      rtb_output.SelectionColor = Color.DodgerBlue;
      rtb_output.AppendText("Checking contents..." + Environment.NewLine);
      if (checker.Verify())
      {
        rtb_output.SelectionColor = Color.ForestGreen;
        rtb_output.AppendText("OK! File parsed without errors!" + Environment.NewLine);
        rtb_output.Focus();
        rtb_output.Select(rtb_output.TextLength - 1, 0);
      }
      else
      {
        int p0 = rtb_output.TextLength;
        rtb_output.SelectionColor = Color.Red;
        rtb_output.AppendText("An error has been encountered: " + Environment.NewLine);
        rtb_output.Focus();
        int p1 = rtb_output.TextLength - 1;
        rtb_output.AppendText(checker.Error);
        rtb_output.Select(p0, 0); // move the horizontal scroll to the left
        rtb_output.Select(p1, 0);
      }
    }

    private void Output(string message)
    {
      rtb_output.SelectionColor = Color.Navy;
      rtb_output.AppendText(message + Environment.NewLine);
    }
  }
}
