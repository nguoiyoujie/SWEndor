using FMOD;

namespace SWEndor.Game.Sound
{
  public partial class SoundManager
  {
    private class InstPrepMusic : InstBase
    {
      public string Name;

      public void Process(SoundManager s)
      {
        if (s.music.Contains(Name))
        {
          Channel dummy = null;
          s.fmodsystem.playSound(s.music[Name].GetSound(false), s.musicgrp, true, out dummy);
        }
      }
    }
  }
}
