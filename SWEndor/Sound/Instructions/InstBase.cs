using System;

namespace SWEndor.Sound //.Instructions
{
  public partial class SoundManager
  {
    public abstract class InstBase
    {
      //public Action Action;

      //public InstBase(Action action) { Action = action; }

      public abstract void Process(SoundManager s);
      //{
      //  Action?.Invoke();
      //}
    }
  }
}
