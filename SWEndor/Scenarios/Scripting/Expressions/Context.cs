using SWEndor.Core;
using SWEndor.Primitives.Factories;

namespace SWEndor.Scenarios.Scripting.Expressions
{
  public delegate Val FunctionDelegate(Context context, Val[] param);

  public class Context
  {
    public readonly Registry<FunctionDelegate> Functions = new Registry<FunctionDelegate>();
    public readonly Registry<ContextVariable> Variables = new Registry<ContextVariable>();
    public readonly Engine Engine;

    public Context(Engine engine)
    {
      Engine = engine;
    }

    public class ContextVariable
    {
      public ContextVariable(string name) { Name = name; }
      public ContextVariable(string name, Val value) { Value = value; }

      public string Name { get; set; }
      public Val Value { get; set; }
    }

    public virtual void Reset()
    {
      Variables.Clear();
    }
  }
}
