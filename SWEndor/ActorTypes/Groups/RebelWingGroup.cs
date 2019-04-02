using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes
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

