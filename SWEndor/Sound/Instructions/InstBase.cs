using System;

namespace SWEndor.Sound //.Instructions
{
  public partial class SoundManager
  {
    public abstract class InstBase
    {
      public abstract void Process(SoundManager s);
    }
  }
}
