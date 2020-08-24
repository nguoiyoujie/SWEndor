using Primrose.Primitives.Extensions;
using Primrose.Primitives.Factories;
using SWEndor.Game.Scenarios.Scripting;
using System;
using System.IO;
using System.Windows.Forms;

namespace SWEndor.ScenarioEditor
{
  public partial class ScriptEditForm : Form
  {
    private readonly Context Context;

    private readonly Registry<IHighlighter, ToolStripMenuItem> HighlightAssoc = new Registry<IHighlighter, ToolStripMenuItem>();

    public ScriptEditForm()
    {
      InitializeComponent();

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
    }

    private void ScriptEditForm_Load(object sender, EventArgs e)
    {
      newFileToolStripMenuItem_Click(null, null);
    }

    private void UpdateTitle()
    {
      tpEditor tp = GetCurrentEditor();
      if (tp != null)
      {
        bool chg = tp.IsChanged;
        if (tp.CurrPath != null)
          Text = "{0} [{1}]".F(Globals.Title, tp.CurrPath + (chg ? "*" : ""));
        else
          Text = "{0} [{1}]".F(Globals.Title, "Untitled Document" + (chg ? "*" : ""));
      }
      else
        Text = Globals.Title;

    }

    private void UpdateStats()
    {
      tpEditor tp = GetCurrentEditor();
      if (tp != null)
        rtfSelStatLabel.Text = "Ln: {0}    Col: {1}    Len: {2}".F(tp.Line, tp.Column, tp.Length);
    }

    private tpEditor GetCurrentEditor()
    {
      if (tcEditor.TabPages.Count == 0)
        return null;
      return tcEditor.SelectedTab as tpEditor;
    }

    private void newFileToolStripMenuItem_Click(object sender, System.EventArgs e)
    {
      tpEditor tp = new tpEditor();
      tp.New();
      tp.TextChanged += editor_TextChanged;
      tp.Editor_SelectionChanged = UpdateStats;
      tcEditor.Add(tp);

      tcEditor.SelectedTab = tp;
      UpdateTitle();
    }

    private void openFileToolStripMenuItem_Click(object sender, System.EventArgs e)
    {
      tpEditor tp = new tpEditor();
      if (tp.Open())
      {
        tp.TextChanged += editor_TextChanged;
        tp.Editor_SelectionChanged = UpdateStats;
        tcEditor.Controls.Add(tp);

        tp.Text = Path.GetFileName(tp.CurrPath);
        tcEditor.SelectedTab = tp;
        UpdateTitle();
      }
      else
        tp.Dispose();
    }

    private void saveFileToolStripMenuItem_Click(object sender, System.EventArgs e)
    {
      tpEditor tp = GetCurrentEditor();
      if (tp != null)
      {
        tp.QuickSave();
        tp.Text = Path.GetFileName(tp.CurrPath);
      }
      UpdateTitle();
    }

    private void saveAsToolStripMenuItem_Click(object sender, System.EventArgs e)
    {
      tpEditor tp = GetCurrentEditor();
      if (tp != null)
      {
        tp.SaveAs();
        tp.Text = Path.GetFileName(tp.CurrPath);
      }
      UpdateTitle();
    }

    private void exitToolStripMenuItem_Click(object sender, System.EventArgs e)
    {
      Close();
    }

    private void ScriptEditForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      foreach (TabPage p in tcEditor.TabPages)
      {
        if (p is tpEditor tp)
          if (!tp.FileClose())
            e.Cancel = true;
      }
    }

    private void editor_TextChanged(object sender, EventArgs e)
    {
      UpdateTitle();
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
      tpEditor tp = GetCurrentEditor();
      if (tp != null)
        tp.Higlight(INIHighlighter.Instance);
    }

    private void langScriptToolStripMenuItem_Click(object sender, EventArgs e)
    {
      tpEditor tp = GetCurrentEditor();
      if (tp != null)
        tp.Higlight(ScriptHighlighter.Instance);
    }

    private void langNoneToolStripMenuItem_Click(object sender, EventArgs e)
    {
      tpEditor tp = GetCurrentEditor();
      if (tp != null)
        tp.Higlight(NoHighlighter.Instance);
    }

    private void lboxFunctions_SelectedIndexChanged(object sender, EventArgs e)
    {
      lboxSig.Items.Clear();
      string s = lboxFunctions.SelectedItem?.ToString();
      if (lboxFunctions.SelectedItem != null)
      {
        for (int i = 0; i < 12; i++)
        {
          object iv = Context.GetFunction(s, i);
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
      tpEditor tp = GetCurrentEditor();
      if (tp != null)
        tp.DoCheck();
    }

    private void tcEditor_SelectedIndexChanged(object sender, EventArgs e)
    {
      UpdateTitle();
    }
  }
}
