﻿namespace SWEndor.Game.Sound
{
  public partial class SoundManager
  {
    private class InstResumeMusic : InstBase
    {
      public void Process(SoundManager s)
      {
        s.current_music_channel.setPaused(false);
        //Channel fmodchannel;
        //s.musicgrp.getChannel(0, out fmodchannel);
        //fmodchannel.setPaused(false);
      }
    }
  }
}
