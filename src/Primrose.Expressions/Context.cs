namespace Primrose.Expressions
{
  public abstract class AContext
  {
    public abstract Val RunFunction(ITracker caller, string fnname, Val[] param);
    public abstract void Reset();
  }
}
