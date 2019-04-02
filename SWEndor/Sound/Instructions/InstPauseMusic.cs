using FMOD;

namespace SWEndor.Sound
{
  public partial class SoundManager
  {
    private class InstPauseMusic : InstBase
    {
      public override void Process(SoundManager s)
      {
        Channel fmodchannel;
        s.musicgrp.getChannel(0, out fmodchannel);
        fmodchannel.setPaused(true);
      }
    }
  }
}
