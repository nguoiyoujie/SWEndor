using SWEndor.Primitives.Factories;

namespace SWEndor.Scenarios.Scripting.Expressions
{
  public delegate object FunctionDelegate(Context context, params object[] param);

  public class Context
  {
    public readonly Registry<FunctionDelegate> Functions = new Registry<FunctionDelegate>();
    public readonly Registry<ContextVariable> Variables = new Registry<ContextVariable>();

    public class ContextVariable
    {
      public ContextVariable(string name) { Name = name; }
      public ContextVariable(string name, dynamic value) { Value = value; }

      public string Name { get; set; }
      public dynamic Value { get; set; }
    }

  }
}
