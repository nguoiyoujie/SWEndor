using System.Text;

namespace SWEndor.Game.Terminal.Commands.General
{
  public class Help : TCommandBase
  {
    public Help()
    {
      Name = "help";
      Desc = "Provides an entry help message";
    }

    protected override TCommandFeedback Evaluate(string[] param)
    {
      return new TCommandFeedback(TCommandFeedbackType.NORMAL, "Enter 'commands' for the list of commands\nParameters can be added to commands by entering 'command:parameter1,parameter2,...'");
    }
  }
}
