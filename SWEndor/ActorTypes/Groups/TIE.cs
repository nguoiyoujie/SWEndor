using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Groups
{
  public class TIE : Fighter
  {
    internal TIE(string name): base(name)
    {
      CullDistance = 7200;
      SoundSources = new SoundSourceInfo[]{ new SoundSourceInfo("engine_tie", 200f, new TV_3DVECTOR(0, 0, -30), true) };
    }
  }
}

