using SWEndor.Core;
using SWEndor.Explosions;
using Primitives.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  internal struct ExplRenderData
  {
    public readonly int AtlasX;
    public readonly int AtlasY;
    public readonly float AnimDuration;
    public readonly float ExpandSize;

    public ExplRenderData(int atlasX, int atlasY, float animDuration, float expandSize)
    {
      AtlasX = atlasX;
      AtlasY = atlasY;
      AnimDuration = animDuration;
      ExpandSize = expandSize;
    }

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

    public void LoadFromINI(INIFile f, string sectionname)
    {
      int atlasX = f.GetInt(sectionname, "AtlasX", AtlasX);
      int atlasY = f.GetInt(sectionname, "AtlasY", AtlasY);
      float animDuration = f.GetFloat(sectionname, "AnimDuration", AnimDuration);
      float expandSize = f.GetFloat(sectionname, "ExpandSize", ExpandSize);
      this = new ExplRenderData(atlasX, atlasY, animDuration, expandSize);
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      f.SetInt(sectionname, "AtlasX", AtlasX);
      f.SetInt(sectionname, "AtlasY", AtlasY);
      f.SetFloat(sectionname, "AnimDuration", AnimDuration);
      f.SetFloat(sectionname, "ExpandSize", ExpandSize);
    }
  }
}
