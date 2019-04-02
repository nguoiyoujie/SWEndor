using System;
using System.Collections.Generic;
using System.IO;
using FMOD;
using System.Collections.Concurrent;

namespace SWEndor.Sound
{
  public partial class SoundManager
  {
    private static SoundManager _instance;
    public static SoundManager Instance()
    {
      if (_instance == null) { _instance = new SoundManager(); }
      return _instance;
    }

    private SoundManager() { }

    public void Dispose() { }


    private FMOD.System fmodsystem = null;
    private int channels = 32; // 0 = music. 1-31 = sounds. ?
    private ChannelGroup musicgrp;
    private Dictionary<string, ChannelGroup> soundgrps = new Dictionary<string, ChannelGroup>();

    private Dictionary<string, FMOD.Sound> music = new Dictionary<string, FMOD.Sound>();
    private Dictionary<string, FMOD.Sound> sounds = new Dictionary<string, FMOD.Sound>();

    private CHANNEL_CALLBACK m_cb;
    private string m_musicLoopName = "";
    private uint m_musicLoopPosition = 0;

    private float m_MasterMusicVolume = 1;
    private float m_MasterSFXVolume = 1;
    private float m_MasterSFXVolumeScenario = 1;

    private ConcurrentQueue<InstBase> m_queuedInstructions = new ConcurrentQueue<InstBase>();

    public float MasterMusicVolume
    {
      get { return m_MasterMusicVolume; }
      set
      {
        if (musicgrp != null)
          musicgrp.setVolume(value);
        m_MasterMusicVolume = value;
      }
    }
    public float MasterSFXVolume
    {
      get { return m_MasterSFXVolume; }
      set
      {
        m_MasterSFXVolume = value;
        foreach (ChannelGroup soundgrp in soundgrps.Values)
          soundgrp.setVolume(m_MasterSFXVolume * m_MasterSFXVolumeScenario);
      }
    }
    public float MasterSFXVolumeScenario
    {
      get { return m_MasterSFXVolumeScenario; }
      set
      {
        m_MasterSFXVolumeScenario = value;
        foreach (ChannelGroup soundgrp in soundgrps.Values)
          soundgrp.setVolume(m_MasterSFXVolume * m_MasterSFXVolumeScenario);
      }
    }


    public void Initialize()
    {
      //Create FMOD System
      FMOD.Factory.System_Create(out fmodsystem);

      //Initialise FMOD
      fmodsystem.init(channels, INITFLAGS.NORMAL, (IntPtr)null);

      fmodsystem.createChannelGroup("music", out musicgrp);
      for (int i = 0; i < channels; i++)
      {
        Channel ch;
        if (fmodsystem.getChannel(i, out ch) == RESULT.OK)
        {
          if (i == 0)
          {
            ch.setChannelGroup(musicgrp);
          }
          else
          {
            //ChannelGroup grp;
            //fmodsystem.createChannelGroup(soundgrps.Count.ToString(), out grp);
            //ch.setChannelGroup(grp);
            //soundgrps.Add(grp);
          }
        }
      }

      m_cb = new CHANNEL_CALLBACK(SoundEndCallback);
    }

    public void Load()
    {
      string[] soundfiles = Directory.GetFiles(Globals.SoundPath, "*.wav", SearchOption.TopDirectoryOnly);
      int i = 0;
      //bool rpt = false;
      foreach (string sdfl in soundfiles)
      {
        string sdname = Path.GetFileNameWithoutExtension(sdfl);

        if (i >= soundgrps.Count)
        {
          i = 0;
          //rpt = true;
        }
        FMOD.Sound sd = null;
        fmodsystem.createStream(sdfl, FMOD.MODE.CREATESAMPLE | FMOD.MODE.ACCURATETIME, out sd);
        sounds.Add(sdname, sd);

        Channel ch;
        fmodsystem.getChannel(i, out ch);
        ChannelGroup grp;
        fmodsystem.createChannelGroup(i.ToString(), out grp);
        ch.setChannelGroup(grp);
        soundgrps.Add(sdname, grp);

        i++;
      }

      string[] musicfiles = Directory.GetFiles(Globals.MusicPath, "*.mp3", SearchOption.TopDirectoryOnly);
      foreach (string mufl in musicfiles)
      {
        string muname = Path.GetFileNameWithoutExtension(mufl);

        FMOD.Sound mu = null;
        fmodsystem.createStream(mufl, FMOD.MODE.CREATESTREAM | FMOD.MODE.ACCURATETIME, out mu);
        music.Add(muname, mu);
      }
    }

    public bool SetMusic(string name, bool loop = false, uint position_ms = 0)
    {
      m_queuedInstructions.Enqueue(new InstPlayMusic { Name = name, Loop = loop, Interrupt = true, Position_ms = position_ms });
      if (loop)
        m_musicLoopName = name;

      return true;
    }

    public void SetMusicLoop(string name, uint position_ms = 0)
    {
      m_musicLoopName = name;
      m_musicLoopPosition = position_ms;
    }

    public void Process() //, uint position = 0)
    {
      InstBase instr;
      while (m_queuedInstructions.TryDequeue(out instr))
        instr.Process(this);
    }

    public void SetMusicStop()
    {
      m_queuedInstructions.Enqueue(new InstStopMusic());
    }

    public void SetMusicPause()
    {
      m_queuedInstructions.Enqueue(new InstPauseMusic());
    }

    public void SetMusicResume()
    {
      m_queuedInstructions.Enqueue(new InstResumeMusic());
    }

    private RESULT SoundEndCallback(IntPtr channelraw, CHANNELCONTROL_TYPE controltype, CHANNELCONTROL_CALLBACK_TYPE type, IntPtr commanddata1, IntPtr commanddata2)
    {
      if (type == FMOD.CHANNELCONTROL_CALLBACK_TYPE.END)
      {
        if (m_musicLoopName.Length > 0)
          SetMusic(m_musicLoopName);
      }
      return FMOD.RESULT.OK;
    }

    public bool SetSound(string name, bool interrupt = true, float volume = 1.0f, bool loop = true)//, uint position = 0)
    {
      m_queuedInstructions.Enqueue(new InstPlaySound { Name = name, Loop = loop, Interrupt = interrupt, Volume = volume });
      return true;
    }

        
        //else if (sound.name.startswith("update_vol_") && sounds.containskey(sound.name.substring(10)))
        //{
        //  channelgroup soundgrp = soundgrps[sound.name.substring(10)];
        //  soundgrp.setvolume(sound.volume * mastersfxvolume);
        //}
        //else if (sound.name.startswith("update_vol+_") && sounds.containskey(sound.name.substring(11)))
        //{
        //  channelgroup soundgrp = soundgrps[sound.name.substring(11)];
        //  float vol = 0;
        //  soundgrp.getvolume(out vol);
        //  if (vol > sound.volume * mastersfxvolume)
        //    soundgrp.setvolume(sound.volume * mastersfxvolume);
        //}
        //else if (sound.name.startswith("update_vol-_") && sounds.containskey(sound.name.substring(11)))
        //{
        //  channelgroup soundgrp = soundgrps[sound.name.substring(11)];
        //  float vol = 0;
        //  soundgrp.getvolume(out vol);
        //  if (vol < sound.volume * mastersfxvolume)
        //    soundgrp.setvolume(sound.volume * mastersfxvolume);
        //}
  
    /*
    public void SetSoundUpdateVolume(string name, float volume)
    {
      mu_queuedSounds.WaitOne();
      m_queuedSounds.Enqueue(new SoundInfo { Name = "update_vol_" + name, Volume = volume });
      mu_queuedSounds.ReleaseMutex();

      //Update();
    }

    public void SetSoundIncreaseToVolume(string name, float volume)
    {
      mu_queuedSounds.WaitOne();
      m_queuedSounds.Enqueue(new SoundInfo { Name = "update_vol+_" + name, Volume = volume });
      mu_queuedSounds.ReleaseMutex();

      //Update();
    }

    public void SetSoundDecreaseToVolume(string name, float volume)
    {
      mu_queuedSounds.WaitOne();
      m_queuedSounds.Enqueue(new SoundInfo { Name = "update_vol-_" + name, Volume = volume });
      mu_queuedSounds.ReleaseMutex();

      //Update();
    }
    */

    public void SetSoundStop(string name)
    {
      m_queuedInstructions.Enqueue(new InstStopOneSound { Name = name });
    }

    public void SetSoundStopAll()
    {
      m_queuedInstructions.Enqueue(new InstStopAllSound());
    }


    public void Update()
    {
      Process();
    }

    public bool PendingUpdate
    {
      get
      {
        fmodsystem.update();
        return (m_queuedInstructions.Count > 0);
      }
    }
  }
}
