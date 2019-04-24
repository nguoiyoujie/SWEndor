using SWEndor.Scenarios.Scripting.Expressions;
using SWEndor.Sound;
using System;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class AudioManagement
  {
    public static object SetMusic(Context context, object[] ps)
    {
      if (ps.Length == 1)
        return Globals.Engine.SoundManager.SetMusic(ps[0].ToString());
      else if (ps.Length == 2)
        return Globals.Engine.SoundManager.SetMusic(ps[0].ToString(), Convert.ToBoolean(ps[1].ToString()));
      else
        return Globals.Engine.SoundManager.SetMusic(ps[0].ToString(), Convert.ToBoolean(ps[1].ToString()), Convert.ToUInt32(ps[2].ToString()));
    }

    public static object SetMusicLoop(Context context, object[] ps)
    {
      if (ps.Length == 1)
        Globals.Engine.SoundManager.SetMusicLoop(ps[0].ToString());
      else
        Globals.Engine.SoundManager.SetMusicLoop(ps[0].ToString(), Convert.ToUInt32(ps[1].ToString()));
      return true;
    }

    public static object SetMusicStop(Context context, object[] ps)
    {
      Globals.Engine.SoundManager.SetMusicStop();
      return null;
    }

    public static object SetMusicPause(Context context, object[] ps)
    {
      Globals.Engine.SoundManager.SetMusicPause();
      return null;
    }

    public static object SetMusicResume(Context context, object[] ps)
    {
      Globals.Engine.SoundManager.SetMusicResume();
      return null;
    }

  }
}
