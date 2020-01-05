using Primrose.Primitives.ValueTypes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SWEndor.ScenarioEditor
{
  public struct HighlighterInfo
  {
    public string LineText;
    public int Line;
  }

  public abstract class AHighlighter : IHighlighter
  {
    public List<Pair<Regex, Color>> Regexes = new List<Pair<Regex, Color>>();
    public ConcurrentQueue<Quad<int, int, int, Color>> Result = new ConcurrentQueue<Quad<int, int, int, Color>>();

    protected AHighlighter() { }

    public void Highlight(FlickerFreeRichEditTextBox box)
    {
      box._ignore = true;
      box._Paint = false;
      box.Enabled = false;
      box.SuspendLayout();

      int p0 = box.SelectionStart;
      int p1 = box.SelectionLength;

      box.SelectAll();
      box.SelectionColor = Color.Black;

      HighlighterInfo[] h = new HighlighterInfo[box.Lines.Length];
      for (int i = 0; i < box.Lines.Length; i++)
        h[i] = new HighlighterInfo() { Line = i, LineText = box.Lines[i] };

      Parallel.ForEach(h, Calc);

      Quad<int, int, int, Color> r;
      while (Result.TryDequeue(out r))
      {
        box.Select(box.GetFirstCharIndexFromLine(r.t) + r.u, r.v);
        if (box.SelectionColor == Color.Black)
          box.SelectionColor = r.w;
      }
      
      box.SelectionStart = p0;
      box.SelectionLength = p1;

      box._Paint = true;
      box.ResumeLayout();
      box._ignore = false;
      box.Enabled = true;
      box.ClearUndo();
    }

    private void Calc(HighlighterInfo h)
    {
      Match m;
      foreach (Pair<Regex, Color> rc in Regexes)
      {
        m = rc.t.Match(h.LineText);
        while (m.Success)
        {
          Result.Enqueue(new Quad<int, int, int, Color>(h.Line, m.Index, m.Length, rc.u));
          m = m.NextMatch();
        }
      }
    }

    public void Highlight(FlickerFreeRichEditTextBox box, int line)
    {
      if (line < 0 || line >= box.Lines.Length)
        return;

      box.Select(box.GetFirstCharIndexFromLine(line), box.Lines[line].Length);
      box.SelectionColor = Color.Black;

      Match m;
      foreach (Pair<Regex, Color> rc in Regexes)
      {
        m = rc.t.Match(box.Lines[line]);
        while (m.Success)
        {
          box.Select(box.GetFirstCharIndexFromLine(line) + m.Index, m.Length);
          if (box.SelectionColor == Color.Black)
            box.SelectionColor = rc.u;
          m = m.NextMatch();
        }
      }
    }
  }
}