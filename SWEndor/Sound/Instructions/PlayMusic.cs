using FMOD;

namespace SWEndor.Sound //.Instructions
{
  public partial class SoundManager
  {
    public class InstPlayMusic : InstBase
    {
      public string Name;
      public bool Interrupt;
      public bool Loop;
      public float Volume;
      public uint Position_ms;

      public override void Process(SoundManager s)
      {
        if (s.music.ContainsKey(Name))
        {
          new InstStopMusic().Process(s);
          //s.StopMusic();

          Channel fmodchannel;
          s.musicgrp.getChannel(0, out fmodchannel);
          s.musicgrp.setVolume(s.MasterMusicVolume);

          s.fmodsystem.playSound(s.music[Name], s.musicgrp, false, out fmodchannel);

          if (Position_ms > 0)
            fmodchannel.setPosition(Position_ms, TIMEUNIT.MS);

          fmodchannel.setCallback(s.m_cb);
        }
      }
    }
  }
}
