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
      public float FadeTime = -1;

      public void Process(SoundManager s)
      {
        if (Name == s.m_currMusic)
        {
            Name += "%";
        }

        if (s.music.ContainsKey(Name))
        {
          //if (!isInterruptMusic)
          //{
            new InstStopMusic() { FadeTime = 0.5f }.Process(s);
            s.fmodsystem.playSound(s.music[Name], s.musicgrp, true, out s.current_channel);

            if (FadeTime > 0)
              DoFadeIn(s.current_channel);

            if (Position_ms > 0)
              s.current_channel.setPosition(Position_ms, TIMEUNIT.MS);

            if (End_ms > 0)
              DoFadeOut(s.current_channel, End_ms / 1000f);


            s.current_channel.setCallback(isInterruptMusic ? s.m_icb : s.m_cb);
            s.current_channel.setPaused(false);

          if (!isInterruptMusic)
            s.m_currMusic = Name;
          else
            s.m_intrMusic = Name;

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
