using Primrose.Expressions;
using System;

namespace SWEndor.Scenarios.Scripting.Functions
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
  }
}
