using System;
using System.IO;
using FMOD;
using System.Collections.Concurrent;
using SWEndor.Primitives;
using System.Text;
using System.Collections.Generic;
using SWEndor.Core;
using SWEndor.Primitives.Extensions;

namespace SWEndor.Sound
{
  public partial class SoundManager
  {
    private struct SoundStartInfo
    {
      public string Name;
      public uint Position;
      public bool IsInterrupt;
    }

    public readonly Engine Engine;
    internal SoundManager(Engine engine)
    {
      Engine = engine;
    }

    public void Dispose() { }


    private FMOD.System fmodsystem = null;
    private int channels = 32; // 0 = music. 1-31 = sounds. ?
    private ChannelGroup musicgrp;
    private ChannelGroup interruptmusicgrp;
    private bool interruptActive;
    private ThreadSafeDictionary<string, ChannelGroup> soundgrps = new ThreadSafeDictionary<string, ChannelGroup>();

    private ThreadSafeDictionary<string, FMOD.Sound> music = new ThreadSafeDictionary<string, FMOD.Sound>();
    private ThreadSafeDictionary<string, FMOD.Sound> sounds = new ThreadSafeDictionary<string, FMOD.Sound>();
    private Channel current_channel;

    // keep callback references alive
    private CHANNEL_CALLBACK m_cb;
    private CHANNEL_CALLBACK m_icb;

    private SoundStartInfo m_musicLoop = new SoundStartInfo();
    private ConcurrentQueue<SoundStartInfo> m_musicQueue = new ConcurrentQueue<SoundStartInfo>();

    private string m_currMusic;
    private string m_prevDynMusic;

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
        if (interruptmusicgrp != null)
          interruptmusicgrp.setVolume(value);
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
      RESULT res = FMOD.Factory.System_Create(out fmodsystem);

      if (res != RESULT.OK)
        throw new Exception(TextLocalization.Get(TextLocalKeys.SOUND_INIT_ERROR).F(res));

      //Initialise FMOD
      fmodsystem.init(channels, INITFLAGS.NORMAL, (IntPtr)null);
      fmodsystem.createChannelGroup("music", out musicgrp);
      fmodsystem.createChannelGroup("interruptmusic", out interruptmusicgrp);

      for (int i = 0; i < channels; i++)
      {
        Channel ch;
        if (fmodsystem.getChannel(i, out ch) == RESULT.OK)
        {
          if (i == 0)
          {
            ch.setChannelGroup(musicgrp);
          }
          else if (i == 1)
          {
            ch.setChannelGroup(interruptmusicgrp);
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

      m_cb = new CHANNEL_CALLBACK(MusicCallback);
      m_icb = new CHANNEL_CALLBACK(InterruptMusiCallback);
    }

    public void Load()
    {
      string[] soundfiles = Directory.GetFiles(Globals.SoundPath, "*.wav", SearchOption.AllDirectories);
      int i = 0;
      //bool rpt = false;
      foreach (string sdfl in soundfiles)
      {
        string sdname = Path.Combine(Path.GetDirectoryName(sdfl), Path.GetFileNameWithoutExtension(sdfl)).Replace(Globals.SoundPath, "");
        Log.Write(Log.DEBUG, LogType.ASSET_SOUND_LOAD, sdname);

        if (i >= soundgrps.Count)
        {
          i = 0;
          //rpt = true;
        }
        FMOD.Sound sd = null;
        fmodsystem.createSound(sdfl, FMOD.MODE.LOWMEM | FMOD.MODE.CREATESTREAM | FMOD.MODE.ACCURATETIME, out sd);
        //fmodsystem.createSound(sdfl, FMOD.MODE.LOWMEM | FMOD.MODE.CREATECOMPRESSEDSAMPLE | FMOD.MODE.ACCURATETIME, out sd);
        sounds.Add(sdname, sd);

        Channel ch;
        fmodsystem.getChannel(i, out ch);
        ChannelGroup grp;
        fmodsystem.createChannelGroup(i.ToString(), out grp);
        ch.setChannelGroup(grp);
        soundgrps.Add(sdname, grp);

        i++;
        Log.Write(Log.DEBUG, LogType.ASSET_SOUND_LOADED, sdname);
      }

      string[] musicfiles = Directory.GetFiles(Globals.MusicPath, "*.mp3", SearchOption.AllDirectories);
      foreach (string mufl in musicfiles)
      {
        string muname = Path.Combine(Path.GetDirectoryName(mufl), Path.GetFileNameWithoutExtension(mufl)).Replace(Globals.MusicPath, "");
        Log.Write(Log.DEBUG, LogType.ASSET_SOUND_LOAD, muname);

        FMOD.Sound mu = null;
        fmodsystem.createStream(mufl, FMOD.MODE.LOWMEM | FMOD.MODE.CREATESTREAM | FMOD.MODE.ACCURATETIME, out mu);
        //fmodsystem.createSound(mufl, FMOD.MODE.LOWMEM | FMOD.MODE.CREATECOMPRESSEDSAMPLE | FMOD.MODE.ACCURATETIME, out mu);
        music.Add(muname, mu);

        // second one for fadepoint to self
        FMOD.Sound mu2 = null;
        fmodsystem.createStream(mufl, FMOD.MODE.LOWMEM | FMOD.MODE.CREATESTREAM | FMOD.MODE.ACCURATETIME, out mu2);
        music.Add(muname + "%", mu2);

        Log.Write(Log.DEBUG, LogType.ASSET_SOUND_LOADED, muname);
      }
    }

    public IEnumerable<string> GetMusicNames()
    {
      return music.Keys;
    }

    public bool PreloadMusic(string name)
    {
      m_queuedInstructions.Enqueue(new InstPrepMusic { Name = name });
      return true;
    }

    public bool SetMusic(string name, bool loop = false, uint position_ms = 0, uint end_ms = 0)
    {
      interruptActive = false;
      m_queuedInstructions.Enqueue(new InstPlayMusic { Name = name, Loop = loop, Position_ms = position_ms, End_ms = end_ms });
      if (loop)
      {
        m_musicLoop.Name = name;
        m_musicLoop.Position = position_ms;
      }
      return true;
    }

    public void SetMusicLoop(string name, uint position_ms = 0)
    {
      m_musicLoop.Name = name;
      m_musicLoop.Position = position_ms;
    }

    public void SetMusicDyn(string name)
    {
      interruptActive = false;
      Piece p = Piece.Factory.Get(name);
      SetMusic(p.SoundName, false, p.EntryPosition);
      PrepDynNext(p.SoundName);
    }

    public void Update()
    {
      InstBase instr;
      while (m_queuedInstructions.TryDequeue(out instr))
        instr.Process(this);
    }

    public void SetMusicStop()
    {
      interruptActive = false;
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

    public void AddMusicSyncPoint(string name, string label, uint position_ms)
    {
      m_queuedInstructions.Enqueue(new InstAddSyncPoint { Name = name, Label = label, Position_ms = position_ms });
    }

    public void SetInterruptMusic(string name, uint position_ms = 0, uint end_ms = 0)
    {
      m_queuedInstructions.Enqueue(new InstPlayMusic { Name = name, isInterruptMusic = true, Position_ms = position_ms, End_ms = end_ms });
    }

    public void QueueInterruptMusic(string name, uint position_ms)
    {
      //if (!interruptActive)
        m_musicQueue.Enqueue(new SoundStartInfo { Name = name, Position = position_ms, IsInterrupt = true });
    }

    public void Clear()
    {
      InstBase i;
      while (m_queuedInstructions.TryDequeue(out i)) { }

      SoundStartInfo s;
      while (m_musicQueue.TryDequeue(out s)) { }

      SetSoundStopAll();
      SetMusicStop();
      interruptActive = false;
      //m_currMusic = null;
      m_prevDynMusic = null;
      m_musicLoop = new SoundStartInfo();
    }

    private int lastInterrupt = 0;
    public void TriggerInterruptMood(int mood)
    {
      if (lastInterrupt == mood) // prevent spam
        return;

      lastInterrupt = mood;

      List<Piece> plist = new List<Piece>(Piece.Factory.GetPieces(mood));
      if (plist.Count > 0)
      {
        Piece p = plist[Engine.Random.Next(0, plist.Count)];
        Engine.SoundManager.QueueInterruptMusic(p.SoundName, p.EntryPosition);
      }
    }

    private RESULT MusicCallback(IntPtr channelraw, CHANNELCONTROL_TYPE controltype, CHANNELCONTROL_CALLBACK_TYPE type, IntPtr commanddata1, IntPtr commanddata2)
    {
      switch (type)
      {
        case FMOD.CHANNELCONTROL_CALLBACK_TYPE.END:
          if (!interruptActive)
          {
            if (m_musicLoop.Name != null && m_musicLoop.Name.Length > 0)
              SetMusic(m_musicLoop.Name, false, m_musicLoop.Position);
            else
            {
              PopDynamicQueue(false);
            }
          }
          break;
        case FMOD.CHANNELCONTROL_CALLBACK_TYPE.SYNCPOINT:
          if (!interruptActive)
          {
            IntPtr syncp;
            music[m_currMusic].getSyncPoint((int)commanddata1, out syncp);
            StringBuilder name = new StringBuilder(5);
            uint offset;
            music[m_currMusic].getSyncPointInfo(syncp, name, 5, out offset, TIMEUNIT.MS);

            switch (name.ToString().ToLower())
            {
              case "exit":
                PopDynamicQueue(false);
                break;
              case "end":
                PopDynamicQueue(true);
                break;
            }
          }
          break;
      }

      //lastInterrupt = 0;
      return FMOD.RESULT.OK;
    }

    private RESULT InterruptMusiCallback(IntPtr channelraw, CHANNELCONTROL_TYPE controltype, CHANNELCONTROL_CALLBACK_TYPE type, IntPtr commanddata1, IntPtr commanddata2)
    {
      interruptActive = false;
      switch (type)
      {
        case FMOD.CHANNELCONTROL_CALLBACK_TYPE.END:
          break;
        case FMOD.CHANNELCONTROL_CALLBACK_TYPE.SYNCPOINT:
          PopDynamicQueue(true);
          break;
      }

      lastInterrupt = 0;
      return FMOD.RESULT.OK;
    }

    private void PopDynamicQueue(bool autogenerate)
    {
      SoundStartInfo ssi2;
      if (m_musicQueue.TryDequeue(out ssi2))
      {
        if (ssi2.IsInterrupt)
          SetInterruptMusic(ssi2.Name, ssi2.Position);
        else
          SetMusic(ssi2.Name, false, ssi2.Position);

        PrepDynNext(ssi2.Name);
      }
      else if (autogenerate)
      {
        // find next piece dynamically using Mood
        string next = GetDynNext(m_currMusic);
        if (next != null)
          m_prevDynMusic = m_currMusic;
        else
          next = GetDynNext(m_prevDynMusic);

        if (next != null)
        {
          Piece pnext = Piece.Factory.Get(next);
          SetMusic(next, false, pnext?.EntryPosition ?? 0);
          PrepDynNext(next);
        }
      }
    }


    private void PrepDynNext(string currentSound)
    {
      if (currentSound == null)
        return;

      Piece p = Piece.Factory.Get(currentSound);
      if (p == null)
        return;

      if (p.MoodTransitions != null)
        foreach (string[] m in p.MoodTransitions)
          foreach (string s in m)
            PreloadMusic(s);
    }

    private string GetDynNext(string currentSound)
    {
      if (currentSound == null)
        return null;

      Piece p = Piece.Factory.Get(currentSound);
      if (p == null)
        return null;

      int mood = (int)(Engine.GameScenarioManager.Scenario?.Mood); // get mood from somewhere...

      string next = null;
      if (p.MoodTransitions != null
        && p.MoodTransitions.Length > mood
        && p.MoodTransitions[mood] != null
        && p.MoodTransitions[mood].Length > 0)
      {
        next = p.MoodTransitions[mood][Engine.Random.Next(0, p.MoodTransitions[mood].Length)];
      }
      return next;
    }

    public bool SetSoundSingle(string name, bool interrupt = true, float volume = 1.0f, bool loop = true)//, uint position = 0)
    {
      m_queuedInstructions.Enqueue(new InstPlaySoundSingle { Name = name, Loop = loop, Interrupt = interrupt, Volume = volume });
      return true;
    }

    public bool SetSound(string name, bool interrupt = true, float volume = 1.0f, bool loop = true)//, uint position = 0)
    {
      m_queuedInstructions.Enqueue(new InstPlaySound { Name = name, Loop = loop, Interrupt = interrupt, Volume = volume });
      return true;
    }

    public void SetSoundStop(string name)
    {
      m_queuedInstructions.Enqueue(new InstStopOneSound { Name = name });
    }

    public void SetSoundStopAll()
    {
      m_queuedInstructions.Enqueue(new InstStopAllSound());
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
