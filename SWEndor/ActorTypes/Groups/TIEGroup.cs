using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes
{
  public class TIEGroup : FighterGroup
  {
    internal TIEGroup(string name): base(name)
    {
      CullDistance = 7200;
      SoundSources = new SoundSourceInfo[]{ new SoundSourceInfo("engine_tie", new TV_3DVECTOR(0, 0, -30), 200, true) };
    }
  }
}

