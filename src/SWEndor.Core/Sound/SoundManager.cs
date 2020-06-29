using System;
using System.IO;
using FMOD;
using System.Collections.Concurrent;
using System.Text;
using System.Collections.Generic;
using Primrose.Primitives.Extensions;
using Primrose.Primitives.Factories;

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

    internal SoundManager() { }
    public void Dispose() { }

    private FMOD.System fmodsystem = null;
    private int channels = 64; // 0 = music. 1-63s = sounds. ?
    private ChannelGroup musicgrp;
    private Registry<ChannelGroup> soundgrps = new Registry<ChannelGroup>();

    private Registry<DoubleBufferedSound> music = new Registry<DoubleBufferedSound>();
    private Registry<DoubleBufferedSound> sounds = new Registry<DoubleBufferedSound>();
    private Channel current_channel;

    // keep callback references alive
    private CHANNEL_CALLBACK m_cb;
    private CHANNEL_CALLBACK m_icb;

    private SoundStartInfo m_musicLoop = new SoundStartInfo();
    private ConcurrentQueue<SoundStartInfo> m_musicQueue = new ConcurrentQueue<SoundStartInfo>();

    internal string CurrMusic { get; private set; }
    internal string IntrMusic { get; private set; }
    internal string PrevDynMusic { get; private set; }
    internal bool EndSyncPointReached;

    private float m_MasterMusicVolume = 1;
    private float m_MasterSFXVolume = 1;
    private float m_MasterSFXVolumeScenario = 1;

    internal Random Random = new Random(); // sound engine uses its own randomizer

    public string Version 
    { 
      get
      { 
        if (fmodsystem == null) { return null; }
        uint v = 0;
        fmodsystem.getVersion(out v);
        return  "fmod.{0:x8}".F(VERSION.dll, v); 
      }
    }

    private ConcurrentQueue<InstBase> m_queuedInstructions = new ConcurrentQueue<InstBase>();

    public float MasterMusicVolume
    {
      get { return m_MasterMusicVolume; }
      set
      {
        musicgrp?.setVolume(value);
        m_MasterMusicVolume = value;
      }
    }
    public float SFXVolume
    {
      get { return m_MasterSFXVolume * m_MasterSFXVolumeScenario; }
    }

    public float MasterSFXVolume
    {
      get { return m_MasterSFXVolume; }
      set { m_MasterSFXVolume = value; }
    }
    public float MasterSFXVolumeScenario
    {
      get { return m_MasterSFXVolumeScenario; }
      set { m_MasterSFXVolumeScenario = value; }
    }

    private int m_mood = 0;
    public int GetMood() { return m_mood; }
    public void SetMood(MoodState value)
    {
      SetMood((int)value);
    }

    public void SetMood(int value)
    {
      if (m_mood != value)
      {
        if (value < 0)
          TriggerInterruptMood(value);
        else
          m_mood = value;
      }
    }

    public struct SingleBufferedSound
    {
      private FMOD.Sound _s1;
      public SingleBufferedSound(FMOD.System fmodsystem, string soundfile)
      {
        fmodsystem.createStream(soundfile, FMOD.MODE.LOWMEM | FMOD.MODE.CREATESTREAM | FMOD.MODE.ACCURATETIME, out _s1);
      }

      public FMOD.Sound GetSound(bool toggleSwitch)
      {
        return _s1;
      }

      public IEnumerable<FMOD.Sound> GetSounds()
      {
        yield return _s1;
      }
    }

    public struct DoubleBufferedSound
    {
      private FMOD.Sound _s1;
      private FMOD.Sound _s2;
      private bool _switch;
      public DoubleBufferedSound(FMOD.System fmodsystem, string soundfile)
      {
        fmodsystem.createStream(soundfile, FMOD.MODE.LOWMEM | FMOD.MODE.CREATESTREAM | FMOD.MODE.ACCURATETIME, out _s1);
        fmodsystem.createStream(soundfile, FMOD.MODE.LOWMEM | FMOD.MODE.CREATESTREAM | FMOD.MODE.ACCURATETIME, out _s2);
        _switch = false;
      }

      public FMOD.Sound GetSound(bool toggleSwitch)
      {
        FMOD.Sound s = _switch ? _s2 : _s1;
        if (toggleSwitch)
          _switch = !_switch;
        return s;
      }

      public IEnumerable<FMOD.Sound> GetSounds()
      {
        yield return _s1;
        yield return _s2;
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

      for (int i = 0; i < channels; i++)
      {
        Channel ch;
        if (fmodsystem.getChannel(i, out ch) == RESULT.OK)
        {
          if (i <= 1)
          {
            ch.setChannelGroup(musicgrp);
          }
        }
      }

      m_cb = new CHANNEL_CALLBACK(MusicCallback);
      m_icb = new CHANNEL_CALLBACK(InterruptMusiCallback);
    }

    public void Load()
    {
      string[] soundfiles = Directory.GetFiles(Globals.SoundPath, Globals.SoundExt, SearchOption.AllDirectories);
      int i = 0;
      foreach (string sdfl in soundfiles)
      {
        string sdname = Path.Combine(Path.GetDirectoryName(sdfl), Path.GetFileNameWithoutExtension(sdfl)).Replace(Globals.SoundPath, "");
        Logger.Log(Logger.DEBUG, LogType.ASSET_LOADING, "Sound", sdname);

        sounds.Add(sdname, new DoubleBufferedSound(fmodsystem, sdfl));

        Channel ch;
        fmodsystem.getChannel(i, out ch);
        ChannelGroup grp;
        fmodsystem.createChannelGroup(i.ToString(), out grp);
        ch.setChannelGroup(grp);
        soundgrps.Add(sdname, grp);

        i++;
        i %= soundgrps.Count;
      }

      string[] musicfiles = Directory.GetFiles(Globals.MusicPath, Globals.MusicExt, SearchOption.AllDirectories);
      foreach (string mufl in musicfiles)
      {
        string muname = Path.Combine(Path.GetDirectoryName(mufl), Path.GetFileNameWithoutExtension(mufl)).Replace(Globals.MusicPath, "");
        Logger.Log(Logger.DEBUG, LogType.ASSET_LOADING, "Music", muname);
        music.Add(muname, new DoubleBufferedSound(fmodsystem, mufl));
      }
    }
    
    public void Update()
    {
      InstBase instr;
      while (m_queuedInstructions.TryDequeue(out instr))
        instr.Process(this);
    }

    public void Clear()
    {
      InstBase i;
      while (m_queuedInstructions.TryDequeue(out i)) { }

      SoundStartInfo s;
      while (m_musicQueue.TryDequeue(out s)) { }

      StopAllSounds();
      StopMusic();
      IntrMusic = null;
      //interruptActive = false;
      //m_currMusic = null;
      PrevDynMusic = null;
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
        Piece p = plist.Random(Random);
        QueueInterruptMusic(p);
      }
    }

    private RESULT MusicCallback(IntPtr channelraw, CHANNELCONTROL_TYPE controltype, CHANNELCONTROL_CALLBACK_TYPE type, IntPtr commanddata1, IntPtr commanddata2)
    {
      switch (type)
      {
        case FMOD.CHANNELCONTROL_CALLBACK_TYPE.END:
          if (IntrMusic == null)
          {
            if (m_musicLoop.Name != null && m_musicLoop.Name.Length > 0)
              SetMusic(m_musicLoop.Name, false, m_musicLoop.Position);
            else
            {
              if (!EndSyncPointReached)
                PopDynamicQueue(true);
              else
                PopDynamicQueue(false);
            }
          }
          break;
        case FMOD.CHANNELCONTROL_CALLBACK_TYPE.SYNCPOINT:
          if (IntrMusic == null)
          {
            IntPtr syncp;
            music[CurrMusic].GetSound(false).getSyncPoint((int)commanddata1, out syncp);
            StringBuilder name = new StringBuilder(5);
            uint offset;
            music[CurrMusic].GetSound(false).getSyncPointInfo(syncp, name, 5, out offset, TIMEUNIT.MS);

            switch (name.ToString())
            {
              case "exit":
                PopDynamicQueue(false);
                break;
              case "end":
                PopDynamicQueue(true);
                EndSyncPointReached = true;
                break;
            }
          }
          break;
      }
      return FMOD.RESULT.OK;
    }

    private RESULT InterruptMusiCallback(IntPtr channelraw, CHANNELCONTROL_TYPE controltype, CHANNELCONTROL_CALLBACK_TYPE type, IntPtr commanddata1, IntPtr commanddata2)
    {
      switch (type)
      {
        case FMOD.CHANNELCONTROL_CALLBACK_TYPE.END:
          if (!EndSyncPointReached)
            PopDynamicQueue(true);
          break;
        case FMOD.CHANNELCONTROL_CALLBACK_TYPE.SYNCPOINT:
          PopDynamicQueue(true);
          EndSyncPointReached = true;
          break;
      }

      IntrMusic = null;
      //lastInterrupt = 0;
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

        Dynamic.PrepDynNext(this, ssi2.Name);
      }
      else if (autogenerate)
      {
        // find next piece dynamically using Mood
        Piece next = Dynamic.GetDynNext(this, IntrMusic);
        if (next == null)
        {
          next = Dynamic.GetDynNext(this, CurrMusic);
          if (next != null)
            PrevDynMusic = CurrMusic;
          else
            next = Dynamic.GetDynNext(this, PrevDynMusic);
        }

        if (next != null)
        {
          if (next.IsInterrupt) // this is an intermission
            SetInterruptMusic(next.SoundName, next?.EntryPosition ?? 0);
          else
          {
            SetMusic(next.SoundName, false, next?.EntryPosition ?? 0);
            //CurrMusic = next.SoundName;
          }
          Dynamic.PrepDynNext(this, next.SoundName);
        }
      }
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
