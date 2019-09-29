using SWEndor.Primitives;
using SWEndor.Primitives.Extensions;
using System;
using System.Text;

namespace SWEndor.Terminal
{
  public class TCommandBase
  {
    public int MinParameters = 0;
    public int MaxParameters = 0;
    public string Name = "base"; 
    public string Desc = "";
    public ThreadSafeList<string> ParameterNames = new ThreadSafeList<string>();
    public ThreadSafeList<string> ParameterDesc = new ThreadSafeList<string>();

    protected virtual TCommandFeedback CheckParamCount(int count)
    {
      if (MinParameters >= 0 && count < MinParameters)
        return new TCommandFeedback(TCommandFeedbackType.ERROR, "At least {0} parameter(s) expected, but {1} encountered!".F(MinParameters, count));
      else if (MaxParameters >= 0 && count > MaxParameters)
        return new TCommandFeedback(TCommandFeedbackType.ERROR, "At most {0} parameter(s) expected, but {1} encountered!".F(MinParameters, count));
      else
        return TCommandFeedback.NULL;
    }

    public TCommandFeedback Execute(params object[] param)
    {
      TCommandFeedback ret = CheckParamCount(param.Length);
      if (ret.IsError())
        return ret;
      if (param.Length == 1 && param[0].ToString().ToLower().Equals("help"))
        return GetHelp();
      return Evaluate(Array.ConvertAll(param, ConvertObjectToString));
    }

    public TCommandFeedback Execute(string[] param)
    {
      TCommandFeedback ret = CheckParamCount(param.Length);
      if (ret.IsError())
        return ret;
      if (param.Length == 1 && param[0].ToString().ToLower().Equals("help"))
        return GetHelp();
      return Evaluate(param);
    }

    protected TCommandFeedback GetHelp()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine(Name);
      sb.AppendLine(Desc);
      for (int i = 0; i < ParameterNames.Count; i++)
      {
        sb.AppendLine("--PARAMETERS--");
        if (i < ParameterDesc.Count && ParameterDesc[i] != null && ParameterDesc[i].Length > 0)
          if (i < MaxParameters)
            sb.AppendLine(string.Format(" {0,20} : {1}", ParameterNames[i], ParameterDesc[i]));
          else
            sb.AppendLine(string.Format("<{0,20}>: {1}", ParameterNames[i], ParameterDesc[i]));
        else
          sb.AppendLine(string.Format(" {0,20} ", ParameterNames[i]));
      }

      return new TCommandFeedback(TCommandFeedbackType.NORMAL, sb.ToString());
    }

    protected virtual TCommandFeedback Evaluate(string[] param)
    {
      return TCommandFeedback.NULL;
    }

    // helper classes
    private string ConvertObjectToString(object obj)
    {
      return obj?.ToString() ?? string.Empty;
    }
  }
}
