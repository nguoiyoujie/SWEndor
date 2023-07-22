using System.Text;

namespace SWEndor.Game.Terminal.Commands.General
{
  public class ListCommands : TCommandBase
  {
    private StringBuilder _sb = new StringBuilder();

    public ListCommands()
    {
      Name = "commands";
      Desc = "Lists the set of available commands";
    }

    protected override TCommandFeedback Evaluate(string[] param)
    {
      _sb.Clear();
      _sb.AppendLine("Command Listing");
      _sb.AppendLine("Commands with parameters are listed with a range denoting the accepted number of parameters:");
      _sb.AppendLine("<command>      [<min>,<max>]");
      _sb.AppendLine("Parameters can be added to commands by entering 'command:parameter1,parameter2,...'");
      foreach (string s in TCommandFunctions.List())
      {
        TCommandBase cmd = TCommandFunctions.Get(s);
        if (cmd.MinParameters == 0 && cmd.MaxParameters == 0)
        {
          _sb.AppendLine(s);
        }
        else
        {
          _sb.Append(s.PadRight(48));
          _sb.AppendFormat("[{0},{1}]", cmd.MinParameters.ToString(), cmd.MaxParameters == int.MaxValue ? "-" : cmd.MaxParameters.ToString());
          _sb.AppendLine();
        }
      }
      return new TCommandFeedback(TCommandFeedbackType.NORMAL, _sb.ToString());
    }
  }
}
