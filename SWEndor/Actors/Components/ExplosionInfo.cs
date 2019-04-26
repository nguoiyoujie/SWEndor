using MTV3D65;
using SWEndor.ActorTypes;

namespace SWEndor.Actors.Components
{
  public struct ExplosionInfo
  {
    private readonly ActorInfo Actor;
    public bool Active;
    public float ExplosionCooldown;
    public float ExplosionRate;
    public float ExplosionSize;
    public string ExplosionType;
    private ActorTypeInfo _cache;
    public bool EnableDeathExplosion;
    public string DeathExplosionType;
    public float DeathExplosionSize;

    public ExplosionInfo(ActorInfo actor)
    {
      Actor = actor;

      Active = false;
      ExplosionCooldown = Actor.GetEngine().Game.GameTime;
      ExplosionRate = 0.5f;
      ExplosionSize = 1;
      ExplosionType = "Explosion";
      _cache = null;
      EnableDeathExplosion = false;
      DeathExplosionType = "ExplosionSm";
      DeathExplosionSize = 1;
    }

    public void Reset()
    {
      Active = false;
      ExplosionCooldown = Actor.GetEngine().Game.GameTime;
      ExplosionRate = 0.5f;
      ExplosionSize = 1;
      ExplosionType = "Explosion";
      _cache = null;
      EnableDeathExplosion = false;
      DeathExplosionType = "ExplosionSm";
      DeathExplosionSize = 1;
    }

    public void ProcessDying()
    {
      if (Actor.TypeInfo.IsExplosion) // don't let explosions create explosions.
        return;

      if (Actor.ActorState == ActorState.DYING)
        Active = true;

      // Explosion
      if (Active && !Actor.GetEngine().Game.IsLowFPS())
      {
        if (ExplosionCooldown < Actor.GetEngine().Game.GameTime - 5f) // skip explosion effects that are delayed after more than 5 secs
          ExplosionCooldown = Actor.GetEngine().Game.GameTime;

        while (ExplosionCooldown < Actor.GetEngine().Game.GameTime && Actor.GetVertexCount() > 0)
        {
          ExplosionCooldown += (float)Actor.GetEngine().Random.NextDouble() * ExplosionRate;
          MakeExplosion(Actor.GetRandomVertex());
        }
      }
      else
      {
        ExplosionCooldown = Actor.GetEngine().Game.GameTime;
      }
    }

    public void OnDeath()
    {
      if (Actor.TypeInfo.IsExplosion) // don't let explosions create explosions.
        return;

      if (EnableDeathExplosion)
        MakeDeathExplosion();
    }

    private void MakeExplosion(TV_3DVECTOR vert)
    {
      if (_cache == null)
        _cache = Actor.GetEngine().ActorTypeFactory.Get(ExplosionType);
      MakeExplosion(_cache, Actor.GetRelativePositionXYZ(vert.x * Actor.Scale.x, vert.y * Actor.Scale.y, vert.z * Actor.Scale.z), ExplosionSize);
    }

    private void MakeDeathExplosion()
    {
      // Death explosion is one count, no cache needed
      MakeExplosion(Actor.GetEngine().ActorTypeFactory.Get(DeathExplosionType), Actor.GetPosition(), DeathExplosionSize);
    }

    private void MakeExplosion(ActorTypeInfo type, TV_3DVECTOR globalPosition, float explSize)
    {
      ActorCreationInfo acinfo = new ActorCreationInfo(type);
      acinfo.Position = globalPosition;
      acinfo.InitialScale = new TV_3DVECTOR(explSize * (Actor.Scale.x + Actor.Scale.y + Actor.Scale.z) / 3, explSize * (Actor.Scale.x + Actor.Scale.y + Actor.Scale.z) / 3, 1);
      ActorInfo.Create(type.GetEngine().ActorFactory, acinfo);
    }
  }
}
