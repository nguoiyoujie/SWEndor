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
      ExplosionCooldown = Globals.Engine.Game.GameTime;
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
      ExplosionCooldown = Globals.Engine.Game.GameTime;
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
      if (Active && !Globals.Engine.Game.IsLowFPS())
      {
        if (ExplosionCooldown < Globals.Engine.Game.GameTime - 5f) // skip explosion effects that are delayed after more than 5 secs
          ExplosionCooldown = Globals.Engine.Game.GameTime;

        while (ExplosionCooldown < Globals.Engine.Game.GameTime && Actor.GetVertexCount() > 0)
        {
          ExplosionCooldown += (float)Globals.Engine.Random.NextDouble() * ExplosionRate;
          MakeExplosion(Actor.GetRandomVertex());
        }
      }
      else
      {
        ExplosionCooldown = Globals.Engine.Game.GameTime;
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
        _cache = Globals.Engine.ActorTypeFactory.Get(ExplosionType);
      MakeExplosion(_cache, Actor.GetRelativePositionXYZ(vert.x * Actor.Scale.x, vert.y * Actor.Scale.y, vert.z * Actor.Scale.z), ExplosionSize);
    }

    private void MakeDeathExplosion()
    {
      // Death explosion is one count, no cache needed
      MakeExplosion(Globals.Engine.ActorTypeFactory.Get(DeathExplosionType), Actor.GetPosition(), DeathExplosionSize);
    }

    private void MakeExplosion(ActorTypeInfo type, TV_3DVECTOR globalPosition, float explSize)
    {
      ActorCreationInfo acinfo = new ActorCreationInfo(type);
      acinfo.Position = globalPosition;
      acinfo.InitialScale = new TV_3DVECTOR(explSize * (Actor.Scale.x + Actor.Scale.y + Actor.Scale.z) / 3, explSize * (Actor.Scale.x + Actor.Scale.y + Actor.Scale.z) / 3, 1);
      ActorInfo.Create(type.Owner.Engine.ActorFactory, acinfo);
    }
  }
}
