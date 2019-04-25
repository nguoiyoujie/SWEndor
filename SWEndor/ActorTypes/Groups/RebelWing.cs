using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Groups
{
  public class RebelWing : Fighter
  {
    internal RebelWing(Factory owner, string name): base(owner, name)
    {
      CullDistance = 7500;
      SoundSources = new SoundSourceInfo[] { new SoundSourceInfo("xwing_engine", 200f, new TV_3DVECTOR(0, 0, -30), true) };
    }
  }
}

