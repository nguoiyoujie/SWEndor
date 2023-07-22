using Primrose.FileFormat.INI;

namespace SWEndor.Game.Models
{
  internal struct ExplodeSystemData
  {
    private static ExplodeData[] NullExpl = new ExplodeData[0];
    private static ParticleData[] NullPart = new ParticleData[0];

    [INISubSectionList(SubsectionPrefix = "EXP")]
    internal ExplodeData[] Explodes;

    [INISubSectionList(SubsectionPrefix = "PRT")]
    internal ParticleData[] Particles;

    public static ExplodeSystemData Default
    { 
      get 
      { 
        return new ExplodeSystemData(NullExpl, NullPart); 
      } 
    }

    public ExplodeSystemData(ExplodeData[] expl, ParticleData[] part)
    {
      Explodes = expl;
      Particles = part;
    }
  }
}
