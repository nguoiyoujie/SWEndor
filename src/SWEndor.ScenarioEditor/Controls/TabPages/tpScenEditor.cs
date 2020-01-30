using SWEndor.FileFormat.INI;
using SWEndor.ScenarioEditor.Checker;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SWEndor.ScenarioEditor
{
  public partial class tpScenEditor : UserControl
  {
    private const string _new = "new";
    public string CurrPath = null;
    public ScenarioFile Source = new ScenarioFile();

    public Action Editor_SelectionChanged;

    public tpScenEditor()
    {
      InitializeComponent();
      Text = _new;
    }

    private void FileOpen(string path)
    {
      Text = Path.GetFileName(path);
    }

    private void FileSave(string path)
    {
      //File.WriteAllText(path, rtb.Text);
      Text = Path.GetFileName(path);
    }

    public void New()
    {
      if (FileClose())
      {
        CurrPath = null;
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
      /*
      if (!CheckEquivalence(rtb.Text, Cache))
      {
        DialogResult d = MessageBox.Show("There are unsaved changes on this document. Save?", Globals.Title, MessageBoxButtons.YesNoCancel);
        if (d == DialogResult.Yes)
          return QuickSave();
        else if (d == DialogResult.Cancel)
          return false;
      }*/
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

    public bool IsChanged { get { return true; } } //!CheckEquivalence(rtb.Text, Cache); } }


    public bool CheckEquivalence(string s1, string s2)
    {
      return s1.Replace(Environment.NewLine, "\n").Equals(s2.Replace(Environment.NewLine, "\n"));
    }
  }
}
