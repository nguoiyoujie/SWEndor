using System.Text;
using Primrose.Primitives.Extensions;

namespace SWEndor.Game.Terminal.Commands.General
{
  public class Version : TCommandBase
  {
    private StringBuilder _sb = new StringBuilder();

    public Version()
    {
      Name = "version";
      Desc = "Prints the version number";
    }

    protected override TCommandFeedback Evaluate(string[] param)
    {
      _sb.Clear();
      _sb.AppendLine("App                  {0}".F(Globals.Version));
      _sb.AppendLine("Primrose             {0}.{1}".F(Primrose.Build.BuildDate, Primrose.Build.Revision));
      _sb.AppendLine("Primrose.Expressions {0}.{1}".F(Primrose.Expressions.Build.BuildDate, Primrose.Expressions.Build.Revision));

      return new TCommandFeedback(TCommandFeedbackType.NORMAL, _sb.ToString());
    }
  }
}
