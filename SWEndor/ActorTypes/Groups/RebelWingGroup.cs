using MTV3D65;

namespace SWEndor.Actors.Types
{
  public class RebelWingGroup : FighterGroup
  {
    internal RebelWingGroup(string name): base(name)
    {
      CullDistance = 7500;
      SoundSources = new SoundSourceInfo[] { new SoundSourceInfo("xwing_engine", new TV_3DVECTOR(0, 0, -30), 200, true) };
    }
  }
}

