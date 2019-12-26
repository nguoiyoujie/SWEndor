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
    public static Val GetMood(Context context)
    {
      return new Val(context.Engine.SoundManager.GetMood());
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
    public static Val SetMood(Context context, int state)
    {
      context.Engine.SoundManager.SetMood(state);
      return Val.NULL;
    }

    public static Val SetMusic(Context context, string piece_name)
    {
      return new Val(context.Engine.SoundManager.SetMusic(piece_name));
    }

    public static Val SetMusic(Context context, string piece_name, bool loop)
    {
      return new Val(context.Engine.SoundManager.SetMusic(piece_name, loop));
    }

    public static Val SetMusic(Context context, string piece_name, bool loop, int position_ms)
    {
      return new Val(context.Engine.SoundManager.SetMusic(piece_name, loop, (uint)position_ms));
    }

    public static Val SetMusic(Context context, string piece_name, bool loop, int position_ms, int end_ms)
    {
      return new Val(context.Engine.SoundManager.SetMusic(piece_name, loop, (uint)position_ms, (uint)end_ms));
    }

    public static Val SetMusicLoop(Context context, string piece_name)
    {
      context.Engine.SoundManager.SetMusicLoop(piece_name);
      return Val.NULL;
    }

    public static Val SetMusicLoop(Context context, string piece_name, int position_ms)
    {
      context.Engine.SoundManager.SetMusicLoop(piece_name, (uint)position_ms);
      return Val.NULL;
    }

    public static Val SetMusicDyn(Context context, string piece_name)
    {
      context.Engine.SoundManager.SetMusicDyn(piece_name);
      return Val.NULL;
    }

    public static Val StopMusic(Context context)
    {
      context.Engine.SoundManager.StopMusic();
      return Val.NULL;
    }

    public static Val PauseMusic(Context context)
    {
      context.Engine.SoundManager.PauseMusic();
      return Val.NULL;
    }

    public static Val ResumeMusic(Context context)
    {
      context.Engine.SoundManager.ResumeMusic();
      return Val.NULL;
    }

    public static Val SetSound(Context context, string sound_name)
    {
      context.Engine.SoundManager.SetSound(sound_name);
      return Val.NULL;
    }

    public static Val SetSound(Context context, string sound_name, float volume)
    {
      context.Engine.SoundManager.SetSound(sound_name, volume);
      return Val.NULL;
    }

    public static Val SetSound(Context context, string sound_name, float volume, bool loop)
    {
      context.Engine.SoundManager.SetSound(sound_name, volume, loop);
      return Val.NULL;
    }

    public static Val SetSoundSingle(Context context, string sound_name)
    {
      context.Engine.SoundManager.SetSoundSingle(sound_name);
      return Val.NULL;
    }

    public static Val SetSoundSingle(Context context, string sound_name, bool interrupt)
    {
      context.Engine.SoundManager.SetSoundSingle(sound_name, interrupt);
      return Val.NULL;
    }

    public static Val SetSoundSingle(Context context, string sound_name, bool interrupt, float volume)
    {
      context.Engine.SoundManager.SetSoundSingle(sound_name, interrupt, volume);
      return Val.NULL;
    }

    public static Val SetSoundSingle(Context context, string sound_name, bool interrupt, float volume, bool loop)
    {
      context.Engine.SoundManager.SetSoundSingle(sound_name, interrupt, volume, loop);
      return Val.NULL;
    }

    public static Val StopSound(Context context, string sound_name)
    {
      context.Engine.SoundManager.StopSound(sound_name);
      return Val.NULL;
    }

    public static Val StopAllSounds(Context context)
    {
      context.Engine.SoundManager.StopAllSounds();
      return Val.NULL;
    }
  }
}
