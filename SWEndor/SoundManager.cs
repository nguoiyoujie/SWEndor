using System;
using System.Collections.Generic;
using System.IO;
using FMOD;
using System.ComponentModel;
using System.Threading;

namespace SWEndor
{
  public class SoundInfo
  {
    public string Name;
    public bool Interrupt;
    public bool Loop;
    public float Volume;
    public uint Position_ms;
  }


  public class SoundManager
  {
    private static SoundManager _instance;
    public static SoundManager Instance()
    {
      if (_instance == null) { _instance = new SoundManager(); }
      return _instance;
    }

    private SoundManager()
    {
      //m_musicLoopWorker.DoWork += MusicLoop;
    }

    public void Dispose()
    {
    }


    private FMOD.System fmodsystem = null;
    private int channels = 32; // 0 = music. 1-31 = sounds.
    private ChannelGroup musicgrp;
    private Dictionary<string, ChannelGroup> soundgrps = new Dictionary<string, ChannelGroup>();

    private Dictionary<string, FMOD.Sound> music = new Dictionary<string, FMOD.Sound>();
    private Dictionary<string, FMOD.Sound> sounds = new Dictionary<string, FMOD.Sound>();

    private CHANNEL_CALLBACK m_cb;
    private BackgroundWorker m_musicLoopWorker = new BackgroundWorker();
    private string m_musicLoopName = "";
    private uint m_musicLoopPosition = 0;

    private float m_MasterMusicVolume = 1;
    private float m_MasterSFXVolume = 1;
    private float m_MasterSFXVolumeScenario = 1;

    private Mutex mu_queuedMusic = new Mutex();
    private Queue<SoundInfo> m_queuedMusic = new Queue<SoundInfo>();
    private Mutex mu_queuedSounds = new Mutex();
    private Queue<SoundInfo> m_queuedSounds = new Queue<SoundInfo>();

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


    public void Initialize(int handle)
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
      //m_musicLoopWorker.RunWorkerAsync();
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
        fmodsystem.createStream(sdfl, FMOD.MODE.CREATESAMPLE, out sd); // accurate time not needed 
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
      mu_queuedMusic.WaitOne();
      m_queuedMusic.Enqueue(new SoundInfo { Name = name, Loop = loop, Interrupt = true, Position_ms = position_ms });
      mu_queuedMusic.ReleaseMutex();

      if (loop)
        m_musicLoopName = name;

      //Update();
      return true;
    }

    public void SetMusicLoop(string name, uint position_ms = 0)
    {
      m_musicLoopName = name;
      m_musicLoopPosition = position_ms;
    }

    public bool ProcessMusic() //, uint position = 0)
    {
      SoundInfo sound = null;
      mu_queuedMusic.WaitOne();
      if (m_queuedMusic.Count > 0)
      {
        sound = m_queuedMusic.Dequeue();
        //if (m_queuedMusic.Count == 0)
        //  m_queuedMusic.Enqueue(name);
      }
      mu_queuedMusic.ReleaseMutex();

      if (sound != null && sound.Name == "stop")
      {
        StopMusic();
      }
      else if (sound != null && sound.Name == "pause")
      {
        PauseMusic();
      }
      else if (sound != null && sound.Name == "resume")
      {
        ResumeMusic();
      }
      else if (sound != null && music.ContainsKey(sound.Name))
      {
        StopMusic();

        Channel fmodchannel;
        musicgrp.getChannel(0, out fmodchannel);
        musicgrp.setVolume(MasterMusicVolume);

        fmodsystem.playSound(music[sound.Name], musicgrp, false, out fmodchannel);

        if (sound.Position_ms > 0)
        {
          fmodchannel.setPosition(sound.Position_ms, TIMEUNIT.MS);
        }
        fmodchannel.setCallback(m_cb);

        return true;
      }
      return false;
    }

    public bool SetMusicStop()
    {
      mu_queuedMusic.WaitOne();
      m_queuedMusic.Enqueue(new SoundInfo { Name = "stop" });
      mu_queuedMusic.ReleaseMutex();

      //Update();
      return true;
    }

    private bool StopMusic()
    {
      Channel fmodchannel;
      musicgrp.getChannel(0, out fmodchannel);
      fmodchannel.setCallback(null);
      fmodchannel.stop();
      return true;
    }

    public bool SetMusicPause()
    {
      mu_queuedMusic.WaitOne();
      m_queuedMusic.Enqueue(new SoundInfo { Name = "pause" });
      mu_queuedMusic.ReleaseMutex();

      //Update();
      return true;
    }

    private bool PauseMusic()
    {
      Channel fmodchannel;
      musicgrp.getChannel(0, out fmodchannel);
      fmodchannel.setPaused(true);
      return true;
    }

    public bool SetMusicResume()
    {
      mu_queuedMusic.WaitOne();
      m_queuedMusic.Enqueue(new SoundInfo { Name = "resume" });
      mu_queuedMusic.ReleaseMutex();

      //Update();
      return true;
    }

    private bool ResumeMusic()
    {
      Channel fmodchannel;
      musicgrp.getChannel(0, out fmodchannel);
      fmodchannel.setPaused(false);
      return true;
    }

    /*
    public void MusicLoop(object sender, DoWorkEventArgs e)
    {
      Channel fmodchannel;
      musicgrp.getChannel(0, out fmodchannel);

      while (Game.Instance().IsLoading || Game.Instance().IsRunning)
      {
        bool bp;
        fmodchannel.isPlaying(out bp);
        if (!bp && m_musicLoopName.Length > 0)
          SetMusic(m_musicLoopName); //, false, m_musicLoopPosition);

        Thread.Sleep(100);
      }
    }
    */

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
      mu_queuedSounds.WaitOne();
      m_queuedSounds.Enqueue(new SoundInfo { Name = name, Loop = loop, Interrupt = interrupt, Volume = volume });
      mu_queuedSounds.ReleaseMutex();

      //Update();
      return true;
    }

    private void ProcessSounds()
    {
      mu_queuedSounds.WaitOne();
      //Queue<SoundInfo> nextsi = new Queue<SoundInfo>();
      while (m_queuedSounds.Count > 0)
      {
        SoundInfo sound = m_queuedSounds.Dequeue();

        if (sound != null && sound.Name.StartsWith("stop_") && sounds.ContainsKey(sound.Name.Substring(4)))
        {
            ChannelGroup soundgrp = soundgrps[sound.Name.Substring(4)];

            Channel fmodchannel;
            soundgrp.getChannel(0, out fmodchannel);
            fmodchannel.stop();
        }
        else if (sound != null && sound.Name == "stopall")
        {
          foreach (string name in sounds.Keys)
          {
            ChannelGroup soundgrp = soundgrps[name];

            Channel fmodchannel;
            soundgrp.getChannel(0, out fmodchannel);
            fmodchannel.stop();
          }
        }
        else if (sound != null && sounds.ContainsKey(sound.Name))
        {
          ChannelGroup soundgrp = soundgrps[sound.Name];

          Channel fmodchannel;
          soundgrp.getChannel(0, out fmodchannel);
          soundgrp.setVolume(sound.Volume * MasterSFXVolume);
          fmodchannel.setLoopCount((sound.Loop) ? -1 : 0);
          //fmodchannel.setMode((sound.Loop) ? MODE.LOOP_NORMAL : MODE.LOOP_OFF);

          bool bp;
          soundgrp.isPlaying(out bp);
          if (!bp || sound.Interrupt)
          {
            fmodchannel.stop();
            fmodsystem.playSound(sounds[sound.Name], soundgrp, false, out fmodchannel);
          }
        }
      }

      //while (nextsi.Count > 0)
      //{
      //  m_queuedMusic.Enqueue(nextsi.Dequeue());
      //}
      mu_queuedSounds.ReleaseMutex();
    }

    public void SetSoundStop(string name)
    {
      mu_queuedSounds.WaitOne();
      m_queuedSounds.Enqueue(new SoundInfo { Name = "stop_" + name });
      mu_queuedSounds.ReleaseMutex();

      //Update();
    }


    public void SetSoundStopAll()
    {
      mu_queuedSounds.WaitOne();
      m_queuedSounds.Enqueue(new SoundInfo { Name = "stopall" });
      mu_queuedSounds.ReleaseMutex();

      //Update();
    }


    public void Update()
    {
      ProcessMusic();
      ProcessSounds();
    }

    public bool PendingUpdate
    {
      get
      {
        fmodsystem.update();
        return (m_queuedMusic.Count > 0 || m_queuedSounds.Count > 0);
      }
    }
  }
}
