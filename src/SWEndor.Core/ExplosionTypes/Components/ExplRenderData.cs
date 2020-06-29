using SWEndor.Core;
using SWEndor.Explosions;
using Primrose.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  internal struct ExplRenderData
  {
#pragma warning disable 0649 // values are filled by the attribute
    [INIValue]
    public readonly int AtlasX;

    [INIValue]
    public readonly int AtlasY;

    [INIValue]
    public readonly float AnimDuration;

    [INIValue]
    public readonly float ExpandSize;
#pragma warning restore 0649 

    public void Process(Engine engine, ExplosionInfo ainfo)
    {
      int frames = AtlasX * AtlasY;
      if (frames != 0 && ainfo.AnimInfo.CyclePeriod != 0)
      {
        int k = frames - 1 - (int)(ainfo.AnimInfo.CycleTime / ainfo.AnimInfo.CyclePeriod * frames);
        float su = 1f / AtlasX;
        float sv = 1f / AtlasY;
        float u = (k % AtlasX) * su;
        float v = (k / AtlasX) * sv;
        ainfo.SetTexMod(u, v, su, sv);
      }
      ainfo.Scale += ExpandSize * engine.Game.TimeSinceRender;
    }
  }
}
