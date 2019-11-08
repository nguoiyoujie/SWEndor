using Primrose.Expressions;
using SWEndor.Scenarios.Scripting.Expressions;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class AudioFns
  {
    public static Val SetMusic(Context context, Val[] ps)
    {
      if (ps.Length == 1)
        return new Val(Globals.Engine.SoundManager.SetMusic((string)ps[0]));
      else if (ps.Length == 2)
        return new Val(Globals.Engine.SoundManager.SetMusic((string)ps[0], (bool)ps[1]));
      else
        return new Val(Globals.Engine.SoundManager.SetMusic((string)ps[0], (bool)ps[1], (uint)(int)ps[2]));
    }

    public static Val SetMusicLoop(Context context, Val[] ps)
    {
      if (ps.Length == 1)
        Globals.Engine.SoundManager.SetMusicLoop((string)ps[0]);
      else
        Globals.Engine.SoundManager.SetMusicLoop((string)ps[0], (uint)(int)ps[1]);
      return Val.TRUE;
    }

    public static Val SetMusicDyn(Context context, Val[] ps)
    {
      Globals.Engine.SoundManager.SetMusicDyn((string)ps[0]);
      return Val.TRUE;
    }

    public static Val SetMusicStop(Context context, Val[] ps)
    {
      Globals.Engine.SoundManager.SetMusicStop();
      return Val.NULL;
    }

    public static Val SetMusicPause(Context context, Val[] ps)
    {
      Globals.Engine.SoundManager.SetMusicPause();
      return Val.NULL;
    }

    public static Val SetMusicResume(Context context, Val[] ps)
    {
      Globals.Engine.SoundManager.SetMusicResume();
      return Val.NULL;
    }

  }
}
