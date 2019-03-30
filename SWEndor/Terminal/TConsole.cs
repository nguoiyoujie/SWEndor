using System.Collections.Generic;

namespace SWEndor.Terminal
{
  public static class TConsole
  {
    private static bool m_Visible;
    public static bool Visible
    {
      get { return m_Visible; }
      set
      {
        if (value)
          Print("Terminal opened.");
        m_Visible = value;
      }
    }

    //public static bool LogCommands = true;
    //public static bool LogReplies = true;

    public static int MaxLineCount = 100;
    public static int DisplayLineCount = 30;
    private static List<string> m_Lines = new List<string>();
    public static string InputLine = "";

    public static string[] GetLines()
    {
      return m_Lines.ToArray();
    }
    
    public static void Print(string line)
    {
      string[] lns = line.Split('\n', '\r');
      foreach (string s in lns)
      {
        m_Lines.Add(s);
        //Log(s);
      }
      while (m_Lines.Count > MaxLineCount)
        m_Lines.RemoveAt(0);
    }

    public static void Execute()
    {
      string input = InputLine;
      InputLine = "";
      Execute(input);
    }

    public static void Execute(string input)
    {
      input = input.Trim();
      if (input.Length > 0)
      {
        Print(">>>  " + input);
        // Execute...
        TCommandFeedback feedback = TCommandParser.Execute(input);
        if (feedback.Content != null)
        {
          string[] feedbacklines = feedback.Content.ToString().Split('\n');
          foreach (string f in feedbacklines)
          {
            Print("  ~  " + f);
          }
        }
      }
      else
        Print(">>>");
    }




  }
}
