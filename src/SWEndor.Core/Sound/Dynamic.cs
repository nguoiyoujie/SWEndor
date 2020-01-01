using Primrose.Primitives.Extensions;
using static SWEndor.Sound.SoundManager;

namespace SWEndor.Sound
{
  public static class Dynamic
  {
    internal static void PrepDynNext(SoundManager mgr, string currentSound)
    {
      if (currentSound == null)
        return;

      Piece p = Piece.Factory.Get(currentSound);
      if (p == null)
        return;

      foreach (string[] m in p.MoodTransitions.GetAll())
        foreach (string s in m)
          mgr.PreloadMusic(s);
    }

    internal static Piece GetDynNext(SoundManager mgr, string currentSound)
    {
      if (currentSound == null)
        return null;

      Piece p = Piece.Factory.Get(currentSound);
      if (p == null)
        return null;

      int mood = mgr.GetMood();

      if (p.ChangeMood.Contains(mood))
        mgr.SetMood(p.ChangeMood[mood]);
      else if (p.ChangeMood.Contains(-1))
        mgr.SetMood(p.ChangeMood[-1]);

      if (p.MoodTransitions.Contains(mood))
      {
        string nextSound = p.MoodTransitions[mood].Random(mgr.Engine.Random);
        Piece pn = Piece.Factory.Get(nextSound);
        return pn;
      }
      else if (p.MoodTransitions.Contains(-1))
      {
        string nextSound = p.MoodTransitions[-1].Random(mgr.Engine.Random);
        Piece pn = Piece.Factory.Get(nextSound);
        return pn;
      }
      return null;
    }
  }
}
