using Primrose.Primitives.Factories;
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
      public bool IsInterrupt;
      public uint EntryPosition;
      public uint[] ExitPositions;
      public uint EndPosition;
      public Registry<int, string[]> MoodTransitions;
      public Registry<int, int> ChangeMood;

      private void UpdateSound(SoundManager manager)
      {
        if (SoundName == null)
          return;

        UpdateSound(manager, SoundName);
        UpdateSound(manager, SoundName + "%");
      }

      private void UpdateSound(SoundManager manager, string soundname)
      {
        FMOD.Sound sound = manager.music[soundname];

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
