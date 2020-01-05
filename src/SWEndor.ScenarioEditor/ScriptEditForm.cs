using Primrose.Primitives.Extensions;
using Primrose.Primitives.Factories;
using Primrose.Primitives.ValueTypes;
using SWEndor.ScenarioEditor.Checker;
using SWEndor.Scenarios.Scripting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SWEndor.ScenarioEditor
{
  public partial class ScriptEditForm : Form
  {
    private string Title = "Script Editor";
    private string CurrPath = null;
    private string Cache = "";
    private Context Context;

    private IHighlighter _highlighter = NoHighlighter.Instance;
    private IHighlighter Highlighter
    {
      get { return _highlighter; }
      set
      {
        _highlighter = value;
        ToolStripMenuItem t = HighlightAssoc.Get(value);
        if (t != null && !t.Checked)
        {
          foreach (ToolStripMenuItem m in languageToolStripMenuItem.DropDownItems)
            m.Checked = false;
          t.Checked = true;
        }
      }
    }
    private Registry<IHighlighter, ToolStripMenuItem> HighlightAssoc = new Registry<IHighlighter, ToolStripMenuItem>();

    public ScriptEditForm()
    {
      InitializeComponent();
      rtb.CreateControl();
      rtb.MouseWheel += Rtb_MouseWheel;

      ofd.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
      sfd.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
      UpdateTitle();
      UpdateStats();

      HighlightAssoc.Add(NoHighlighter.Instance, langNoneToolStripMenuItem);
      HighlightAssoc.Add(INIHighlighter.Instance, langINIToolStripMenuItem);
      HighlightAssoc.Add(ScriptHighlighter.Instance, langScriptToolStripMenuItem);

      Context = new Context(null);
      foreach (string s in Context.ValFuncRef)
        lboxFunctions.Items.Add(s);

      _f = rtb.Font;
      _z = rtb.Font.Size * rtb.ZoomFactor;
    }

    private void UpdateTitle()
    {
      bool same = CheckEquivalence(rtb.Text, Cache);

      if (CurrPath != null)
        Text = "{0} [{1}]".F(Title, CurrPath + (same ? "" : "*"));
      else
        Text = "{0} [{1}]".F(Title, "Untitled Document" + (same ? "" : "*"));
    }

    private void UpdateStats()
    {
      int ln = rtb.GetLineFromCharIndex(rtb.SelectionStart);
      rtfSelStatLabel.Text = "Ln: {0}    Col: {1}    Len: {2}".F(ln + 1, rtb.SelectionStart - rtb.GetFirstCharIndexOfCurrentLine() + 1, rtb.SelectionLength);
    }

    private void FileOpen(string path)
    {
      Cache = File.ReadAllText(path);
      Highlighter = AutoHighlight(Path.GetExtension(path));
      rtb.TextChanged -= rtb_TextChanged;
      rtb.Text = Cache;
      Highlighter.Highlight(rtb);
      rtb.TextChanged += rtb_TextChanged;
    }

    private void FileSave(string path)
    {
      File.WriteAllText(path, rtb.Text);
      Cache = rtb.Text;
    }

    private bool FileClose()
    {
      if (!CheckEquivalence(rtb.Text, Cache))
      {
        DialogResult d = MessageBox.Show("There are unsaved changes on this document. Save?", Title, MessageBoxButtons.YesNoCancel);
        if (d == DialogResult.Yes)
          return QuickSave();
        else if (d == DialogResult.Cancel)
          return false;
      }
      return true;
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
      UpdateTitle();
    }

    public void Open()
    {
      if (FileClose())
      {
        if (ofd.ShowDialog() == DialogResult.OK)
        {
          CurrPath = ofd.FileName;
          FileOpen(CurrPath);
        }
      }
      UpdateTitle();
    }

    public bool SaveAs()
    {
      if (sfd.ShowDialog() == DialogResult.OK)
      {
        CurrPath = sfd.FileName;
        FileSave(CurrPath);
        UpdateTitle();
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
        UpdateTitle();
        return true;
      }
    }

    public bool CheckEquivalence(string s1, string s2)
    {
      return s1.Replace(Environment.NewLine, "\n").Equals(s2.Replace(Environment.NewLine, "\n"));
    }

    private void newFileToolStripMenuItem_Click(object sender, System.EventArgs e)
    {
      New();
    }

    private void openFileToolStripMenuItem_Click(object sender, System.EventArgs e)
    {
      Open();
    }

    private void saveFileToolStripMenuItem_Click(object sender, System.EventArgs e)
    {
      QuickSave();
    }

    private void saveAsToolStripMenuItem_Click(object sender, System.EventArgs e)
    {
      SaveAs();
    }

    private void exitToolStripMenuItem_Click(object sender, System.EventArgs e)
    {
      Close();
    }

    private int prevSel = 0;
    private int prevLen = 0;
    private int Sel = 0;
    private int Len = 0;
    private void rtb_TextChanged(object sender, EventArgs e)
    {
      if (rtb._ignore)
        return;

      int p0 = rtb.SelectionStart;
      int p1 = rtb.SelectionLength;

      UpdateTitle();
      rtb._Paint = false;
      rtb.SuspendLayout();
      //rtb.Enabled = false;
      int ln1 = rtb.GetLineFromCharIndex((prevSel < rtb.SelectionStart) ? prevSel : rtb.SelectionStart);
      int ln2 = rtb.GetLineFromCharIndex((prevSel + prevLen > rtb.SelectionStart + rtb.SelectionLength) ? prevSel + prevLen : rtb.SelectionStart + rtb.SelectionLength);

      for (int l = ln1; l <= ln2; l++)
        Highlighter.Highlight(rtb, l);
      rtb._Paint = true;
      rtb.ResumeLayout();
      //rtb.Enabled = true;

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
      UpdateStats();
    }

    private void rtb_VScroll(object sender, EventArgs e)
    {
      pLineNum.Invalidate();
    }

    private void rtb_SizeChanged(object sender, EventArgs e)
    {
      pLineNum.Invalidate();
    }

    private void Rtb_MouseWheel(object sender, MouseEventArgs e)
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

    float _z;
    Font _f;
    Brush _b = Brushes.DarkOliveGreen;
    List<int> _n = new List<int>();

    private void pLineNum_Paint(object sender, PaintEventArgs e)
    {
      pLineNum.SuspendLayout();
      int i0 = rtb.GetLineFromCharIndex(rtb.GetCharIndexFromPosition(new Point(2,2)));
      int y0 = rtb.GetPositionFromCharIndex(rtb.GetFirstCharIndexFromLine(i0)).Y;
      _n.Clear();
      for (int i = i0; i < rtb.Lines.Length; i++)
      {
        if (rtb.GetPositionFromCharIndex(rtb.GetFirstCharIndexFromLine(i)).Y > pLineNum.Height)
          break;

        _n.Add(i + 1);
        //e.Graphics.DrawString((i + 1).ToString(), f, b, new PointF(2, y));
      }
      string s = string.Join(Environment.NewLine, _n);
      e.Graphics.DrawString(s, _f, _b, new PointF(2, y0));
      pLineNum.ResumeLayout();
    }

    private void ScriptEditForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (!FileClose())
        e.Cancel = true;
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


    private void langINIToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Highlighter = INIHighlighter.Instance;
      Highlighter.Highlight(rtb);
    }

    private void langScriptToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Highlighter = ScriptHighlighter.Instance;
      Highlighter.Highlight(rtb);
    }

    private void langNoneToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Highlighter = NoHighlighter.Instance;
      Highlighter.Highlight(rtb);
    }

    private void lboxFunctions_SelectedIndexChanged(object sender, EventArgs e)
    {
      lboxSig.Items.Clear();
      string s = lboxFunctions.SelectedItem?.ToString();
      if (lboxFunctions.SelectedItem != null)
      {
        for (int i = 0; i < 12; i++)
        {
          IValFunc iv = Context.ValFuncs.Get(new Pair<string, int>(s, i));
          if (iv != null)
          {
            Type t = iv.GetType();
            if (!t.IsGenericType)
              lboxSig.Items.Add(s + "()");
            else
            {
              Type[] ts = t.GetGenericArguments();
              string[] ss = new string[i];
              for (int j = 0; j < i; j++)
              {
                ss[j] = ts[j].Name;
              }
              lboxSig.Items.Add(s + "(" + string.Join(", ", ss) + ")");
            }
          }
        }
      }
    }

    private void checkToolStripMenuItem_Click(object sender, EventArgs e)
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
        rtb_output.SelectionColor = Color.Red;
        rtb_output.AppendText("An error has been encountered: " + Environment.NewLine);
        rtb_output.Focus();
        rtb_output.Select(rtb_output.TextLength - 1, 0);
        rtb_output.AppendText(checker.Error);
      }
    }

    private void Output(string message)
    {
      rtb_output.SelectionColor = Color.Navy;
      rtb_output.AppendText(message + Environment.NewLine);
    }
  }
}
