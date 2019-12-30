using Primrose.Expressions;

namespace Primrose.Expressions.UnitTests.Scripting
{
  public interface IValFunc { Val Execute(ITracker caller, string _funcName, Context c, Val[] p); }
}
