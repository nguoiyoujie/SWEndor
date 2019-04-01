using MTV3D65;
using SWEndor.ActorTypes;

namespace SWEndor.Actors.Components
{
  public class ExplosionInfo
  {
    private ActorInfo Actor;
    public bool Active = false;
    public float ExplosionCooldown = Game.Instance().GameTime;
    public float ExplosionRate = 0.5f;
    public float ExplosionSize = 1;
    public string ExplosionType = "Explosion";
    private ActorTypeInfo _cache;
    public bool EnableDeathExplosion = false;
    public string DeathExplosionType = "ExplosionSm";
    public float DeathExplosionSize = 1;

    public ExplosionInfo(ActorInfo actor)
    {
      Actor = actor;
    }

    public void ProcessDying()
    {
      if (Actor.TypeInfo.IsExplosion) // don't let explosions create explosions.
        return;

      if (Actor.ActorState == ActorState.DYING)
        Active = true;

      // Explosion
      if (Active && !Game.Instance().IsLowFPS())
      {
        if (ExplosionCooldown < Game.Instance().GameTime - 5f) // skip explosion effects that are delayed after more than 5 secs
          ExplosionCooldown = Game.Instance().GameTime;

        while (ExplosionCooldown < Game.Instance().GameTime && Actor.GetVertexCount() > 0)
        {
          ExplosionCooldown += (float)Engine.Instance().Random.NextDouble() * ExplosionRate;
          MakeExplosion(Actor.GetRandomVertex());
        }
      }
      else
      {
        ExplosionCooldown = Game.Instance().GameTime;
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
        _cache = ActorTypeInfo.Factory.Get(ExplosionType);
      MakeExplosion(_cache, Actor.GetRelativePositionXYZ(vert.x * Actor.Scale.x, vert.y * Actor.Scale.y, vert.z * Actor.Scale.z), ExplosionSize);
    }

    private void MakeDeathExplosion()
    {
      // Death explosion is one count, no cache needed
      MakeExplosion(ActorTypeInfo.Factory.Get(DeathExplosionType), Actor.GetPosition(), DeathExplosionSize);
    }

    private void MakeExplosion(ActorTypeInfo type, TV_3DVECTOR globalPosition, float explSize)
    {
      ActorCreationInfo acinfo = new ActorCreationInfo(type);
      acinfo.Position = globalPosition;
      acinfo.InitialScale = new TV_3DVECTOR(explSize * (Actor.Scale.x + Actor.Scale.y + Actor.Scale.z) / 3, explSize * (Actor.Scale.x + Actor.Scale.y + Actor.Scale.z) / 3, 1);
      ActorInfo.Create(acinfo);
    }
  }
}
