using Primrose.Expressions;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class AudioFns
  {
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

    public static Val SetMusicStop(Context context)
    {
      context.Engine.SoundManager.SetMusicStop();
      return Val.NULL;
    }

    public static Val SetMusicPause(Context context)
    {
      context.Engine.SoundManager.SetMusicPause();
      return Val.NULL;
    }

    public static Val SetMusicResume(Context context)
    {
      context.Engine.SoundManager.SetMusicResume();
      return Val.NULL;
    }

  }
}
