using Primrose.Expressions;

namespace SWEndor.Scenarios.Scripting
{
  public interface IValFunc { Val Execute(ITracker caller, string _funcName, Context c, Val[] p); }
}
