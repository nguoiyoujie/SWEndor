using Primrose.Primitives.ValueTypes;
using System.Drawing;
using System.Text.RegularExpressions;

namespace SWEndor.ScenarioEditor
{
  public class INIHighlighter : AHighlighter
  {
    public static INIHighlighter Instance = new INIHighlighter();
    RegexOptions ropt = RegexOptions.CultureInvariant | RegexOptions.Compiled;

    private INIHighlighter()
    {
      Regexes.Add(new Pair<Regex, Color>(new Regex(@"(#|//).*", ropt), Color.ForestGreen));
      Regexes.Add(new Pair<Regex, Color>(new Regex(@"^\s*\[.*\]", ropt), Color.Indigo));
      Regexes.Add(new Pair<Regex, Color>(new Regex(@"^([^\=])+", ropt), Color.Blue));
    }
  }
}
