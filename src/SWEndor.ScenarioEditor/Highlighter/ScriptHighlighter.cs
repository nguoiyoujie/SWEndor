using Primrose.Primitives.ValueTypes;
using SWEndor.Game.Scenarios.Scripting;
using System.Drawing;
using System.Text.RegularExpressions;

namespace SWEndor.ScenarioEditor
{
  public class ScriptHighlighter : AHighlighter
  {
    public static ScriptHighlighter Instance = new ScriptHighlighter();
    RegexOptions ropt = RegexOptions.CultureInvariant | RegexOptions.Compiled;

    private ScriptHighlighter()
    {
      Regexes.Add(new Pair<Regex, Color>(new Regex(@"//.*", ropt), Color.ForestGreen));
      Regexes.Add(new Pair<Regex, Color>(new Regex(@"^.*\:(?=\s*)$", ropt), Color.Indigo));
      //Regexes.Add(new Pair<Regex, Color>(new Regex(@"[a-zA-Z_][a-zA-Z0-9_\.]*", ropt), Color.));
      Regexes.Add(new Pair<Regex, Color>(new Regex(@"\""(\""\""|[^\""])*\""", ropt), Color.Brown));
      Regexes.Add(new Pair<Regex, Color>(new Regex(@"\b(bool|float|float2|float3|float4|int|string)\b", ropt), Color.DodgerBlue));
      Regexes.Add(new Pair<Regex, Color>(new Regex(@"\b(return|new|true|false|null|if|else|while|foreach|in|for)\b", ropt), Color.Blue));

      Context c = new Context(null);
      string reg = null;
      foreach (string s in c.ValFuncRef)
        reg = string.Concat("(", string.Join("|", c.ValFuncRef), @")(?=\s*\()");
      if (reg != null)
        Regexes.Add(new Pair<Regex, Color>(new Regex(reg, ropt), Color.MidnightBlue));
    }
  }
}
