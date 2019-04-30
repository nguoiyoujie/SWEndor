using MTV3D65;
using SWEndor.ActorTypes;

namespace SWEndor.Actors.Components
{
  public enum ExplosionTrigger { NONE, DYING, ALWAYS }
  public enum DeathExplosionTrigger { NONE, TIMENOTEXPIRED_ONLY, ALWAYS }

  public class ExplosionInfo
  {
    private readonly ActorInfo Actor;
    public float ExplosionCooldown;
    public float ExplosionRate;
    public float ExplosionSize;
    public string ExplosionType;
    private ActorTypeInfo _cache;
    public DeathExplosionTrigger DeathExplosionTrigger;
    public string DeathExplosionType;
    public float DeathExplosionSize;

    public ExplosionInfo(ActorInfo actor)
    {
      Actor = actor;

      ExplosionCooldown = Actor.Game.GameTime;
      ExplosionRate = 0.5f;
      ExplosionSize = 1;
      ExplosionType = "Explosion";
      _cache = null;
      DeathExplosionTrigger = DeathExplosionTrigger.NONE;
      DeathExplosionType = "ExplosionSm";
      DeathExplosionSize = 1;
    }

    public void Reset()
    {
      ExplosionCooldown = Actor.Game.GameTime;
      ExplosionRate = 0.5f;
      ExplosionSize = 1;
      ExplosionType = "Explosion";
      _cache = null;
      DeathExplosionTrigger = DeathExplosionTrigger.NONE;
      DeathExplosionType = "ExplosionSm";
      DeathExplosionSize = 1;
    }

    public void ProcessDying()
    {
      if (Actor.TypeInfo.IsExplosion) // don't let explosions create explosions.
        return;

      // Explosion
      if (!Actor.Game.IsLowFPS())
      {
        if (ExplosionCooldown < Actor.Game.GameTime - 5f) // skip explosion effects that are delayed after more than 5 secs
          ExplosionCooldown = Actor.Game.GameTime;

        while (ExplosionCooldown < Actor.Game.GameTime && Actor.GetVertexCount() > 0)
        {
          ExplosionCooldown += (float)Actor.Engine.Random.NextDouble() * ExplosionRate;
          MakeExplosion(Actor.GetRandomVertex());
        }
      }
    }

    public void OnDeath()
    {
      if (Actor.TypeInfo.IsExplosion) // don't let explosions create explosions.
        return;

      if (DeathExplosionTrigger == DeathExplosionTrigger.ALWAYS
        || (DeathExplosionTrigger == DeathExplosionTrigger.TIMENOTEXPIRED_ONLY && Actor.CombatInfo.TimedLife > 0)
        )
        MakeDeathExplosion();
    }

    private void MakeExplosion(TV_3DVECTOR vert)
    {
      if (_cache == null)
        _cache = Actor.ActorTypeFactory.Get(ExplosionType);
      MakeExplosion(_cache, Actor.GetRelativePositionXYZ(vert.x * Actor.Scale.x, vert.y * Actor.Scale.y, vert.z * Actor.Scale.z), ExplosionSize);
    }

    private void MakeDeathExplosion()
    {
      // Death explosion is one count, no cache needed
      MakeExplosion(Actor.ActorTypeFactory.Get(DeathExplosionType), Actor.GetPosition(), DeathExplosionSize);
    }

    private void MakeExplosion(ActorTypeInfo type, TV_3DVECTOR globalPosition, float explSize)
    {
      ActorCreationInfo acinfo = new ActorCreationInfo(type);
      acinfo.Position = globalPosition;
      acinfo.InitialScale = new TV_3DVECTOR(explSize * (Actor.Scale.x + Actor.Scale.y + Actor.Scale.z) / 3, explSize * (Actor.Scale.x + Actor.Scale.y + Actor.Scale.z) / 3, 1);
      ActorInfo.Create(Actor.ActorFactory, acinfo);
    }
  }
}
