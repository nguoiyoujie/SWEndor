using Primrose.Primitives.Extensions;
using System;
using System.IO;
using System.Windows.Forms;

namespace SWEndor.ScenarioEditor
{
  public partial class ScriptEditForm : Form
  {
    private string Title = "Script Editor";
    private string CurrPath = null;
    private string Cache = "";
    private Formatter Formatter = new Formatter();

    public ScriptEditForm()
    {
      InitializeComponent();
      rtb.CreateControl();

      ofd.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
      sfd.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
      UpdateTitle();
    }

    private void UpdateTitle()
    {
      bool same = CheckEquivalence(rtb.Text, Cache);

      if (CurrPath != null)
        Text = "{0} [{1}]".F(Title, CurrPath + (same ? "" : "*"));
      else
        Text = "{0} [{1}]".F(Title, "Untitled Document" + (same ? "" : "*"));
    }

    private void FileOpen(string path)
    {
      Cache = File.ReadAllText(path);
      rtb.Text = Cache;

      rtb._ignore = true;
      rtb._Paint = false;
      rtb.SuspendLayout();
      Formatter.Format(rtb);
      rtb._Paint = true;
      rtb.ResumeLayout();
      rtb._ignore = false;
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
          QuickSave();
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

    public void SaveAs()
    {
      if (sfd.ShowDialog() == DialogResult.OK)
      {
        CurrPath = sfd.FileName;
        FileSave(CurrPath);
      }
      UpdateTitle();
    }

    public void QuickSave()
    {
      if (CurrPath == null)
        SaveAs();
      else
        FileSave(CurrPath);
      UpdateTitle();
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
      if (FileClose())
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

      UpdateTitle();
      rtb._Paint = false;
      rtb.SuspendLayout();
      int ln1 = rtb.GetLineFromCharIndex((prevSel < rtb.SelectionStart) ? prevSel : rtb.SelectionStart);
      int ln2 = rtb.GetLineFromCharIndex((prevSel + prevLen > rtb.SelectionStart + rtb.SelectionLength) ? prevSel + prevLen : rtb.SelectionStart + rtb.SelectionLength);

      for (int l = ln1; l <= ln2; l++)
        Formatter.Format(rtb, l);
      rtb._Paint = true;
      rtb.ResumeLayout();
    }

    private void rtb_SelectionChanged(object sender, EventArgs e)
    {
      prevSel = Sel;
      prevLen = Len;
      Sel = rtb.SelectionStart;
      Len = rtb.SelectionLength;
    }
  }
}
