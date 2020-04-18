using FMOD;
using System;

namespace SWEndor.Sound
{
  public partial class SoundManager
  {
    private class InstAddSyncPoint : InstBase
    {
      public string Name;
      public string Label;
      public uint Position_ms;

      public void Process(SoundManager s)
      {
        if (s.music.Contains(Name) && Position_ms > 0)
        {
          IntPtr ptr;

          foreach (FMOD.Sound snd in s.music[Name].GetSounds())
            snd.addSyncPoint(Position_ms, TIMEUNIT.MS, Label, out ptr);
        }
      }
    }
  }
}
