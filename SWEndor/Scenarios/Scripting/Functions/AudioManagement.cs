using SWEndor.Scenarios.Scripting.Expressions;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class AudioManagement
  {
    public static Val SetMusic(Context context, Val[] ps)
    {
      if (ps.Length == 1)
        return new Val(Globals.Engine.SoundManager.SetMusic(ps[0].vS));
      else if (ps.Length == 2)
        return new Val(Globals.Engine.SoundManager.SetMusic(ps[0].vS, ps[1].vB));
      else
        return new Val(Globals.Engine.SoundManager.SetMusic(ps[0].vS, ps[1].vB, (uint)ps[2].vI));
    }

    public static Val SetMusicLoop(Context context, Val[] ps)
    {
      if (ps.Length == 1)
        Globals.Engine.SoundManager.SetMusicLoop(ps[0].vS);
      else
        Globals.Engine.SoundManager.SetMusicLoop(ps[0].vS, (uint)ps[1].vI);
      return Val.TRUE;
    }

    public static Val SetMusicDyn(Context context, Val[] ps)
    {
      Globals.Engine.SoundManager.SetMusicDyn(ps[0].vS);
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
