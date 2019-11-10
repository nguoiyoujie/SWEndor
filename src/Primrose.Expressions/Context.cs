namespace Primrose.Expressions
{
  /// <summary>
  /// Provides a context for the script
  /// </summary>
  public abstract class AContext
  {
    /// <summary>
    /// Runs a user defined function. An EvalException should be thrown if errors arise from the function.
    /// </summary>
    /// <param name="caller">The script object that called this function</param>
    /// <param name="fnname">The function name</param>
    /// <param name="param">The list of parameters</param>
    /// <returns></returns>
    public abstract Val RunFunction(ITracker caller, string fnname, Val[] param);
  }
}
