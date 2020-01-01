using Primrose.Primitives.ValueTypes;
using SWEndor.Scenarios.Scripting;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SWEndor.ScenarioEditor
{
  public class Formatter
  {
    public List<Pair<Regex, Color>> Regexes = new List<Pair<Regex, Color>>();
    public Regex colorreg = new Regex(@"{\\colortbl.*}", RegexOptions.CultureInvariant);
    public string colortbl;

    public Formatter()
    {
      Init();
    }

    public void Init()
    {
      Regexes.Add(new Pair<Regex, Color>(new Regex(@"^.*\:(?=\s*)$", RegexOptions.CultureInvariant), Color.Indigo));
      Regexes.Add(new Pair<Regex, Color>(new Regex(@"//.*", RegexOptions.CultureInvariant), Color.ForestGreen));
      //Regexes.Add(new Pair<Regex, Color>(new Regex(@"[a-zA-Z_][a-zA-Z0-9_\.]*", RegexOptions.CultureInvariant), Color.));
      Regexes.Add(new Pair<Regex, Color>(new Regex(@"\""(\""\""|[^\""])*\""", RegexOptions.CultureInvariant), Color.Brown));
      Regexes.Add(new Pair<Regex, Color>(new Regex(@"\b(bool|float|float2|float3|float4|int|string)\b", RegexOptions.CultureInvariant), Color.DodgerBlue));
      Regexes.Add(new Pair<Regex, Color>(new Regex(@"\b(true|false|null|if|then|else|while|foreach|in|for)\b", RegexOptions.CultureInvariant), Color.Blue));

      Context c = new Context(null);
      string reg = null;
      foreach (string s in c.ValFuncRef)
        reg = string.Concat("(", string.Join("|", c.ValFuncRef), @")(?=\s*\()");
      Regexes.Add(new Pair<Regex, Color>(new Regex(reg, RegexOptions.CultureInvariant), Color.MidnightBlue));

      /*
      StringBuilder sb = new StringBuilder(@"{\\colortbl ;\\red0\\green0\\blue0");
      foreach (Pair<Regex, Color> rc in Regexes)
      {
        sb.Append(@"\\red");
        sb.Append(rc.u.R);
        sb.Append(@"\\green");
        sb.Append(rc.u.G);
        sb.Append(@"\\blue");
        sb.Append(rc.u.B);
        sb.Append(@";");
      }
      sb.Append(@"}");
      colortbl = sb.ToString();
      */
    }

    public void Format(RichTextBox box)
    {
      int ln_offset = 0;
      int p0 = box.SelectionStart;
      int p1 = box.SelectionLength;

      box.SelectAll();
      box.SelectionColor = Color.Black;

      for (int i = 0; i < box.Lines.Length; i++)
      {
        Match m;
        foreach (Pair<Regex, Color> rc in Regexes)
        {
          m = rc.t.Match(box.Lines[i]);
          while (m.Success)
          {
            box.Select(ln_offset + m.Index, m.Length);
            if (box.SelectionColor == Color.Black)
              box.SelectionColor = rc.u;
            m = m.NextMatch();
          }
        }

        ln_offset += box.Lines[i].Length + 1;
      }
      box.SelectionStart = p0;
      box.SelectionLength = p1;
    }

    public void Format(RichTextBox box, int line)
    {
      int ln_offset = 0;
      if (line < 0 || line >= box.Lines.Length)
        return;

      for (int i = 0; i < line; i++)
      {
        ln_offset += box.Lines[i].Length + 1;
      }

      int p0 = box.SelectionStart;
      int p1 = box.SelectionLength;

      box.Select(box.GetFirstCharIndexFromLine(line), box.Lines[line].Length);
      box.SelectionColor = Color.Black;

      Match m;
      foreach (Pair<Regex, Color> rc in Regexes)
      {
        m = rc.t.Match(box.Lines[line]);
        while (m.Success)
        {
          box.Select(ln_offset + m.Index, m.Length);
          if (box.SelectionColor == Color.Black)
            box.SelectionColor = rc.u;
          m = m.NextMatch();
        }
      }

      box.SelectionStart = p0;
      box.SelectionLength = p1;
    }
  }
}
