using SWEndor.Core;
using SWEndor.Explosions;
using Primitives.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  internal struct ExplRenderData
  {
    private const string sExplRender = "ExplRender";

    [INIValue(sExplRender, "AtlasX")]
    public readonly int AtlasX;

    [INIValue(sExplRender, "AtlasY")]
    public readonly int AtlasY;

    [INIValue(sExplRender, "AnimDuration")]
    public readonly float AnimDuration;

    [INIValue(sExplRender, "ExpandSize")]
    public readonly float ExpandSize;

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
