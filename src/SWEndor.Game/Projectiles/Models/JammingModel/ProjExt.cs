using FMOD;
using SWEndor.Game.Models;

namespace SWEndor.Game.Projectiles
{
  public partial class ProjectileInfo
  {
    public bool IsStunned
    {
      get { return Jamming.IsStunned(Engine); }
    }

    public void InflictStun(float stunduration)
    {
      Jamming.InflictStun(Engine, stunduration);
    }
  }
}