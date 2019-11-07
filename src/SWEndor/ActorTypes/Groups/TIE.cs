using MTV3D65;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;

namespace SWEndor.ActorTypes.Groups
{
  public class TIE : Fighter
  {
    internal TIE(Factory owner, string id, string name) : base(owner, id, name)
    {
      RenderData.CullDistance = 7200;
      SoundSources = new SoundSourceData[]{ new SoundSourceData(SoundGlobals.EngineTie, 200f, new TV_3DVECTOR(0, 0, -30), true, isEngineSound: true) };
    }
  }
}

