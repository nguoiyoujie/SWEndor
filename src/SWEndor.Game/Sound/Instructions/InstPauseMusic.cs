namespace SWEndor.Game.Sound
{
  public partial class SoundManager
  {
    private class InstPauseMusic : InstBase
    {
      public void Process(SoundManager s)
      {
        s.current_channel.setPaused(true);
        //Channel fmodchannel;
        //s.musicgrp.getChannel(0, out fmodchannel);
        //fmodchannel.setPaused(true);
      }
    }
  }
}
