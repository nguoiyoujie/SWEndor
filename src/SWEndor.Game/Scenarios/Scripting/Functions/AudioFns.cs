using Primrose.Expressions;

namespace SWEndor.Game.Scenarios.Scripting.Functions
{
  public static class AudioFns
  {
    /// <summary>
    /// Gets the audio Mood
    /// </summary>
    /// <param name="context">The game context</param>
    /// <returns>The current mood</returns>
    public static Val GetMood(IContext context)
    {
      return new Val(((Context)context).Engine.SoundManager.GetMood());
    }

    /// <summary>
    /// Triggers the audio Mood
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     INT mood
    /// </param>
    /// <returns>NULL</returns>
    public static Val SetMood(IContext context, int state)
    {
      ((Context)context).Engine.SoundManager.SetMood(state);
      return Val.NULL;
    }

    public static Val SetMusic(IContext context, string piece_name)
    {
      return new Val(((Context)context).Engine.SoundManager.SetMusic(piece_name));
    }

    public static Val SetMusic(IContext context, string piece_name, bool loop)
    {
      return new Val(((Context)context).Engine.SoundManager.SetMusic(piece_name, loop));
    }

    public static Val SetMusic(IContext context, string piece_name, bool loop, int position_ms)
    {
      return new Val(((Context)context).Engine.SoundManager.SetMusic(piece_name, loop, (uint)position_ms));
    }

    public static Val SetMusic(IContext context, string piece_name, bool loop, int position_ms, int end_ms)
    {
      return new Val(((Context)context).Engine.SoundManager.SetMusic(piece_name, loop, (uint)position_ms, (uint)end_ms));
    }

    public static Val SetMusicLoop(IContext context, string piece_name)
    {
      ((Context)context).Engine.SoundManager.SetMusicLoop(piece_name);
      return Val.NULL;
    }

    public static Val SetMusicLoop(IContext context, string piece_name, int position_ms)
    {
      ((Context)context).Engine.SoundManager.SetMusicLoop(piece_name, (uint)position_ms);
      return Val.NULL;
    }

    public static Val SetMusicDyn(IContext context, string piece_name)
    {
      ((Context)context).Engine.SoundManager.SetMusicDyn(piece_name);
      return Val.NULL;
    }

    public static Val StopMusic(IContext context)
    {
      ((Context)context).Engine.SoundManager.StopMusic();
      return Val.NULL;
    }

    public static Val PauseMusic(IContext context)
    {
      ((Context)context).Engine.SoundManager.PauseMusic();
      return Val.NULL;
    }

    public static Val ResumeMusic(IContext context)
    {
      ((Context)context).Engine.SoundManager.ResumeMusic();
      return Val.NULL;
    }

    public static Val SetSound(IContext context, string sound_name)
    {
      ((Context)context).Engine.SoundManager.SetSound(sound_name);
      return Val.NULL;
    }

    public static Val SetSound(IContext context, string sound_name, float volume)
    {
      ((Context)context).Engine.SoundManager.SetSound(sound_name, volume);
      return Val.NULL;
    }

    public static Val SetSound(IContext context, string sound_name, float volume, bool loop)
    {
      ((Context)context).Engine.SoundManager.SetSound(sound_name, volume, loop);
      return Val.NULL;
    }

    public static Val SetSoundSingle(IContext context, string sound_name)
    {
      ((Context)context).Engine.SoundManager.SetSoundSingle(sound_name);
      return Val.NULL;
    }

    public static Val SetSoundSingle(IContext context, string sound_name, bool interrupt)
    {
      ((Context)context).Engine.SoundManager.SetSoundSingle(sound_name, interrupt);
      return Val.NULL;
    }

    public static Val SetSoundSingle(IContext context, string sound_name, bool interrupt, float volume)
    {
      ((Context)context).Engine.SoundManager.SetSoundSingle(sound_name, interrupt, volume);
      return Val.NULL;
    }

    public static Val SetSoundSingle(IContext context, string sound_name, bool interrupt, float volume, bool loop)
    {
      ((Context)context).Engine.SoundManager.SetSoundSingle(sound_name, interrupt, volume, loop);
      return Val.NULL;
    }

    public static Val StopSound(IContext context, string sound_name)
    {
      ((Context)context).Engine.SoundManager.StopSound(sound_name);
      return Val.NULL;
    }

    public static Val StopAllSounds(IContext context)
    {
      ((Context)context).Engine.SoundManager.StopAllSounds();
      return Val.NULL;
    }

    public static Val QueueSpeech(IContext context, string speech_name)
    {
      ((Context)context).Engine.SoundManager.QueueSpeech(speech_name);
      return Val.NULL;
    }

    public static Val QueueSpeech(IContext context, string sp1, string sp2)
    {
      var sndmgr = ((Context)context).Engine.SoundManager;
      sndmgr.QueueSpeech(sp1);
      sndmgr.QueueSpeech(sp2);
      return Val.NULL;
    }

    public static Val QueueSpeech(IContext context, string sp1, string sp2, string sp3)
    {
      var sndmgr = ((Context)context).Engine.SoundManager;
      sndmgr.QueueSpeech(sp1);
      sndmgr.QueueSpeech(sp2);
      sndmgr.QueueSpeech(sp3);
      return Val.NULL;
    }

    public static Val QueueSpeech(IContext context, string sp1, string sp2, string sp3, string sp4)
    {
      var sndmgr = ((Context)context).Engine.SoundManager;
      sndmgr.QueueSpeech(sp1);
      sndmgr.QueueSpeech(sp2);
      sndmgr.QueueSpeech(sp3);
      sndmgr.QueueSpeech(sp4);
      return Val.NULL;
    }

    public static Val QueueSpeech(IContext context, string sp1, string sp2, string sp3, string sp4, string sp5)
    {
      var sndmgr = ((Context)context).Engine.SoundManager;
      sndmgr.QueueSpeech(sp1);
      sndmgr.QueueSpeech(sp2);
      sndmgr.QueueSpeech(sp3);
      sndmgr.QueueSpeech(sp4);
      sndmgr.QueueSpeech(sp5);
      return Val.NULL;
    }

    public static Val QueueSpeech(IContext context, string sp1, string sp2, string sp3, string sp4, string sp5, string sp6)
    {
      var sndmgr = ((Context)context).Engine.SoundManager;
      sndmgr.QueueSpeech(sp1);
      sndmgr.QueueSpeech(sp2);
      sndmgr.QueueSpeech(sp3);
      sndmgr.QueueSpeech(sp4);
      sndmgr.QueueSpeech(sp5);
      sndmgr.QueueSpeech(sp6);
      return Val.NULL;
    }

    public static Val StopSpeech(IContext context)
    {
      ((Context)context).Engine.SoundManager.StopSpeech();
      return Val.NULL;
    }
  }
}
