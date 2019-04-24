using System;

namespace SWEndor.Sound
{
  public partial class SoundManager
  {
    public partial class Piece
    {
      public bool CutIn;
      public string SoundName;
      public uint EntryPosition;
      public uint[] ExitPositions;
      public uint EndPosition;
      public string[][] MoodTransitions;

      private void UpdateSound()
      {
        if (SoundName == null)
          return;

        string name = SoundName;
        FMOD.Sound sound = Globals.Engine.SoundManager.music[name];

        if (sound == null)
          return;

        IntPtr ptr;
        foreach (uint ep in ExitPositions)
          if (ep > 0)
            sound.addSyncPoint(ep, FMOD.TIMEUNIT.MS, "exit", out ptr);

        if (EndPosition > 0)
          sound.addSyncPoint(EndPosition, FMOD.TIMEUNIT.MS, "end", out ptr);

        UpdateReuseSound();
      }

      private void UpdateReuseSound()
      {
        if (SoundName == null)
          return;

        string name = SoundName + "%";
        FMOD.Sound sound = Globals.Engine.SoundManager.music[name];

        if (sound == null)
          return;

        IntPtr ptr;
        foreach (uint ep in ExitPositions)
          if (ep > 0)
            sound.addSyncPoint(ep, FMOD.TIMEUNIT.MS, "exit", out ptr);

        if (EndPosition > 0)
          sound.addSyncPoint(EndPosition, FMOD.TIMEUNIT.MS, "end", out ptr);
      }
    }
  }
}
