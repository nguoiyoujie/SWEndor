using System;

namespace SWEndor.Sound
{
  public partial class SoundManager
  {
    public partial class Piece
    {
      public string Name;
      public int[] IntermissionTransitions;
      public string SoundName;
      public uint EntryPosition;
      public uint[] ExitPositions;
      public uint EndPosition;
      public string[][] MoodTransitions;
      public string[][] OnInterruptTransitions;

      private void UpdateSound()
      {
        if (SoundName == null)
          return;

        UpdateSound(SoundName);
        UpdateSound(SoundName + "%");
      }

      private void UpdateSound(string soundname)
      {
        FMOD.Sound sound = Globals.Engine.SoundManager.music[soundname];

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
