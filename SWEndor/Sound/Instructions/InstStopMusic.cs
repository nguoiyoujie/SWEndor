using FMOD;

namespace SWEndor.Sound
{
  public partial class SoundManager
  {
    private class InstStopMusic : InstBase
    {
      public float FadeTime = 0.5f;

      public override void Process(SoundManager s)
      {
        Channel fmodchannel;
        s.musicgrp.getChannel(0, out fmodchannel);
        //s.musicgrp.getChannel(s.m_currMusicCh, out fmodchannel);
        fmodchannel.setCallback(null);

        if (FadeTime <= 0)
        {
          fmodchannel.stop();
        }
        else
        {
          DoFadeOut(fmodchannel);
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
