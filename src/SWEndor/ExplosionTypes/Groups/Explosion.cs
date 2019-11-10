using SWEndor.Models;

namespace SWEndor.ExplosionTypes.Groups
{
  internal class Explosion : ExplosionTypeInfo
  {
    internal Explosion(Factory owner, string id, string name) : base(owner, id, name)
    {
      // Combat
      RenderData.CullDistance = 12500f;
      RenderData.RadarSize = 1;
      RenderData.RadarType = RadarType.FILLED_CIRCLE_M;
    }
  }
}

