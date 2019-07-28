using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.Primitives.Traits;

namespace SWEndor.Actors.Traits
{
  /*
  public interface ISpawner : ITrait { void Spawn<A>(A actor) where A : ITraitOwner; }
  public interface IExplodeOnDying : ISpawner { }
  public interface IExplodeOnDeath : ISpawner { }

  public class SpawnExplosion : IExplodeOnDying
  {
    public const string ExplosionTypeDefault = "Explosion";

    private float Cooldown;
    public readonly float Rate;
    public readonly float Size;
    public readonly string Type;
    internal ActorTypeInfo _cache;

    public SpawnExplosion(float rate, float size, string type)
    {
      Cooldown = 0;
      Rate = rate;
      Size = size;
      Type = type;
    }

    public SpawnExplosion(SpawnExplosion copy)
    {
      Cooldown = 0;
      Rate = copy.Rate;
      Size = copy.Size;
      Type = copy.Type;
    }

    public void Spawn<A>(A actor)
      where A : ITraitOwner
    {
      if (actor.Engine.Game.IsLowFPS())
        return;

      IMeshModel m = actor.TraitOrDefault<IMeshModel>();
      if (m == null)
        return;

      ITransform t = actor.TraitOrDefault<ITransform>();
      if (t == null)
        return;

      if (Cooldown < actor.Engine.Game.GameTime)
        return;

      if (m.GetVertexCount() > 0)
        return;

      if (Cooldown < actor.Engine.Game.GameTime - 5f) // skip explosion effects that are delayed after more than 5 secs
        Cooldown = actor.Engine.Game.GameTime;

      int i = actor.Engine.Random.Next(0, m.GetVertexCount());
      TV_3DVECTOR v = m.GetVertex(i);
      float scale = t.Scale;

      Cooldown += (float)actor.Engine.Random.NextDouble() * Rate;

      if (_cache == null)
        _cache = actor.Engine.ActorTypeFactory.Get(Type);

      ActorCreationInfo acinfo = new ActorCreationInfo(_cache);
      acinfo.Position = t.GetRelativePositionFUR(actor, v.z * scale, v.y * scale, v.x * scale);
      acinfo.InitialScale = Size; //* actor.CoordData.Scale;
      actor.Engine.ActorFactory.Create(acinfo);
    }
  }

  public class SpawnDeathExplosion : IExplodeOnDeath
  {
    public enum DeathExplosionTrigger { NONE, TIMENOTEXPIRED_ONLY, ALWAYS }
    public const string DeathExplosionTypeDefault = "ExplosionSm";

    public readonly DeathExplosionTrigger Trigger;
    public readonly float Size;
    public readonly string Type;

    private ActorTypeInfo _cache;

    public SpawnDeathExplosion(DeathExplosionTrigger trigger, float size, string type)
    {
      Trigger = trigger;
      Size = size;
      Type = type;
    }

    public SpawnDeathExplosion(SpawnDeathExplosion copy)
    {
      Trigger = copy.Trigger;
      Size = copy.Size;
      Type = copy.Type;
    }

    public void Spawn<A>(A actor)
      where A : ITraitOwner
    {
      if (_cache == null)
        _cache = actor.Engine.ActorTypeFactory.Get(Type);

      ITransform t = actor.TraitOrDefault<ITransform>();
      if (t == null)
        return;

      ActorCreationInfo acinfo = new ActorCreationInfo(_cache);
      acinfo.Position = t.GetGlobalPosition(actor, actor.Engine.Game.GameTime);
      acinfo.InitialScale = Size * t.Scale;
      actor.Engine.ActorFactory.Create(acinfo);
    }
  }
  */
}
