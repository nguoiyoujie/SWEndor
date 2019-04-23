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

      public override void Process(SoundManager s)
      {
        if (s.music.ContainsKey(Name) && Position_ms > 0)
        {
          IntPtr ptr;
          s.music[Name].addSyncPoint(Position_ms, TIMEUNIT.MS, Label, out ptr);
          if (!Name.EndsWith("%"))
            new InstAddSyncPoint { Name = this.Name + "%", Label = this.Label, Position_ms = this.Position_ms }.Process(s);
        }
      }
    }
  }
}
