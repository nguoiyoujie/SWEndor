using FMOD;

namespace SWEndor.Sound
{
  public partial class SoundManager
  {
    private class InstPlayMusic : InstBase
    {
      public string Name;
      public bool isInterruptMusic;
      public bool Loop;
      public uint Position_ms;
      public uint End_ms;
      public float FadeTime = 0.5f;

      public override void Process(SoundManager s)
      {
        if (Name == s.m_currMusic)
        {
            Name += "%";
        }

        if (s.music.ContainsKey(Name))
        {
          Channel fmodchannel = null;
          if (!isInterruptMusic)
          {
            new InstStopMusic() { FadeTime = this.FadeTime }.Process(s);
            s.fmodsystem.playSound(s.music[Name], s.musicgrp, true, out fmodchannel);

            if (FadeTime > 0)
              DoFadeIn(fmodchannel);

            if (Position_ms > 0)
              fmodchannel.setPosition(Position_ms, TIMEUNIT.MS);

            if (End_ms > 0)
              DoFadeOut(fmodchannel, End_ms / 1000f);

            fmodchannel.setCallback(s.m_cb);
            fmodchannel.setPaused(false);

            s.m_currMusic = Name;
          }
          else
          {
            s.interruptActive = true;
            s.musicgrp.getChannel(0, out fmodchannel);
            float duration = GetLength(s.music[Name]);
            DoFadeOut(fmodchannel);
            DoFadeIn(fmodchannel, duration - FadeTime * 2);

            Channel fmodchannel2 = null;
            s.fmodsystem.playSound(s.music[Name], s.interruptmusicgrp, true, out fmodchannel2);

            if (FadeTime > 0)
              DoFadeIn(fmodchannel2);

            if (Position_ms > 0)
              fmodchannel2.setPosition(Position_ms, TIMEUNIT.MS);

            DoFadeOut(fmodchannel2, duration - FadeTime);

            fmodchannel2.setCallback(s.m_icb);
            fmodchannel2.setPaused(false);
          }
        }
      }

      public void DoFadeIn(Channel fmodchannel)
      {
        DoFadeIn(fmodchannel, 0);
      }

      public float GetLength(FMOD.Sound sound)
      {
        uint ret;
        sound.getLength(out ret, TIMEUNIT.MS);
        return ret / 1000f;
      }

      public void DoFadeIn(Channel fmodchannel, float time)
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

        uint dely = (uint)(rate * time);
        uint disp = (uint)(rate * FadeTime);
        // sin progression
        fmodchannel.addFadePoint(dspclock, 0);
        fmodchannel.addFadePoint(dspclock + dely + disp / 4, 0.383f);
        fmodchannel.addFadePoint(dspclock + dely + disp / 2, 0.707f);
        fmodchannel.addFadePoint(dspclock + dely + disp * 3 / 4, 0.924f);
        fmodchannel.addFadePoint(dspclock + dely + disp, 1);
      }

      public void DoFadeOut(Channel fmodchannel)
      {
        DoFadeOut(fmodchannel, 0);
      }

    public void DoFadeOut(Channel fmodchannel, float time)
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

        uint dely = (uint)(rate * time);
        uint disp = (uint)(rate * FadeTime);
        // sin progression
        fmodchannel.addFadePoint(dspclock, 1);
        fmodchannel.addFadePoint(dspclock + dely + disp / 4, 0.924f);
        fmodchannel.addFadePoint(dspclock + dely + disp / 2, 0.707f);
        fmodchannel.addFadePoint(dspclock + dely + disp * 3 / 4, 0.383f);
        fmodchannel.addFadePoint(dspclock + dely + disp, 0);
      }
    }
  }
}
