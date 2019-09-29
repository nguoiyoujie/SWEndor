using SWEndor.Scenarios.Scripting.Expressions;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class AudioManagement
  {
    public static Val SetMusic(Context context, Val[] ps)
    {
      if (ps.Length == 1)
        return new Val(Globals.Engine.SoundManager.SetMusic(ps[0].ValueS));
      else if (ps.Length == 2)
        return new Val(Globals.Engine.SoundManager.SetMusic(ps[0].ValueS, ps[1].ValueB));
      else
        return new Val(Globals.Engine.SoundManager.SetMusic(ps[0].ValueS, ps[1].ValueB, (uint)ps[2].ValueI));
    }

    public static Val SetMusicLoop(Context context, Val[] ps)
    {
      if (ps.Length == 1)
        Globals.Engine.SoundManager.SetMusicLoop(ps[0].ValueS);
      else
        Globals.Engine.SoundManager.SetMusicLoop(ps[0].ValueS, (uint)ps[1].ValueI);
      return Val.TRUE;
    }

    public static Val SetMusicDyn(Context context, Val[] ps)
    {
      Globals.Engine.SoundManager.SetMusicDyn(ps[0].ValueS);
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
