using SWEndor.Core;
using Primrose.Primitives.Factories;


namespace SWEndor.Scenarios.Scripting.Expressions
{
  public delegate Val FunctionDelegate(Context context, Val[] param);

  public class Context
  {
    public readonly Registry<FunctionDelegate> Functions = new Registry<FunctionDelegate>();
    public readonly Engine Engine;

    public Context(Engine engine)
    {
      Engine = engine;
    }

    public virtual void Reset()
    {
    }
  }
}
