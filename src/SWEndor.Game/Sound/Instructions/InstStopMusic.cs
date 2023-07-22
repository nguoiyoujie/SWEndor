using FMOD;

namespace SWEndor.Game.Sound
{
  public partial class SoundManager
  {
    private class InstStopMusic : InstBase
    {
      public float FadeTime = 0.5f;

      public void Process(SoundManager s)
      {
        if (s.current_music_channel == null)
          return;
        //Channel fmodchannel;
        //s.musicgrp.getChannel(0, out fmodchannel);
        //s.musicgrp.getChannel(s.m_currMusicCh, out fmodchannel);
        s.current_music_channel.setCallback(null);

        if (FadeTime <= 0)
        {
          s.current_music_channel.stop();
        }
        else
        {
          DoFadeOut(s.current_music_channel);
        }
      }

      public void DoFadeOut(Channel fmodchannel)
      {
        ulong dspclock;
        ulong parentclock;
        int rate;
        FMOD.System sys;
        SPEAKERMODE mode;
        int numspeakers;
        fmodchannel.getSystemObject(out sys);
        sys.getSoftwareFormat(out rate, out mode, out numspeakers);
        fmodchannel.getDSPClock(out parentclock, out dspclock);
        fmodchannel.addFadePoint(dspclock, 1);
        fmodchannel.addFadePoint(dspclock + ((uint)(rate * FadeTime)), 0);
        fmodchannel.setDelay(0, dspclock + ((uint)(rate * FadeTime)), true);
      }
    }
  }
}
