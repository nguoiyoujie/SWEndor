using MTV3D65;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes;

namespace SWEndor.Actors.Components
{
  public static class ExplosionSystem
  {
    internal static void ProcessDying(Engine engine, int id)
    {
      ProcessDying(engine, id, ref engine.ActorDataSet.ExplodeData[engine.ActorFactory.GetIndex(id)]);
    }

    private static void ProcessDying(Engine engine, int id, ref ExplodeData data)
    {
      ActorInfo actor = engine.ActorFactory.Get(id);

      if (actor.TypeInfo.IsExplosion) // don't let explosions create explosions.
        return;

      // Explosion
      if (!engine.Game.IsLowFPS())
      {
        if (data.ExplosionCooldown < engine.Game.GameTime - 5f) // skip explosion effects that are delayed after more than 5 secs
          data.ExplosionCooldown = engine.Game.GameTime;

        while (data.ExplosionCooldown < engine.Game.GameTime && actor.GetVertexCount() > 0)
        {
          data.ExplosionCooldown += (float)engine.Random.NextDouble() * data.ExplosionRate;
          MakeExplosion(engine, id, actor.GetRandomVertex(), ref data);
        }
      }
    }

    internal static void OnDeath(Engine engine, int id)
    {
      int index = engine.ActorFactory.GetIndex(id);
      OnDeath(engine, id, ref engine.ActorDataSet.ExplodeData[index], ref engine.TimedLifeDataSet.list[index]);
    }

    private static void OnDeath(Engine engine, int id, ref ExplodeData data, ref TimedLifeData tdata)
    {
      ActorInfo actor = engine.ActorFactory.Get(id);

      if (actor.TypeInfo.IsExplosion) // don't let explosions create explosions.
        return;

      if (data.DeathExplosionTrigger == DeathExplosionTrigger.ALWAYS
        || (data.DeathExplosionTrigger == DeathExplosionTrigger.TIMENOTEXPIRED_ONLY 
          && tdata.TimedLife > 0)
        )
        MakeDeathExplosion(engine, id, ref data);
    }

    private static void MakeExplosion(Engine engine, int id, TV_3DVECTOR vert, ref ExplodeData data)
    {
      ActorInfo actor = engine.ActorFactory.Get(id);
      float scale = engine.MeshDataSet.Scale_get(id);  

      if (data._cache == null)
        data._cache = actor.ActorTypeFactory.Get(data.ExplosionType);

      MakeExplosion(actor
        , data._cache
        , actor.GetRelativePositionXYZ(vert.x * scale, vert.y * scale, vert.z * scale)
        , data.ExplosionSize
        , ref data);
    }

    private static void MakeDeathExplosion(Engine engine, int id, ref ExplodeData data)
    {
      // Death explosion is one count, no cache needed
      ActorInfo actor = engine.ActorFactory.Get(id);
      MakeExplosion(actor, actor.ActorTypeFactory.Get(data.DeathExplosionType), actor.GetPosition(), data.DeathExplosionSize, ref data);
    }

    private static void MakeExplosion(ActorInfo actor, ActorTypeInfo type, TV_3DVECTOR globalPosition, float explSize, ref ExplodeData data)
    {
      ActorCreationInfo acinfo = new ActorCreationInfo(type);
      acinfo.Position = globalPosition;
      acinfo.InitialScale = explSize * actor.Engine.MeshDataSet.Scale_get(actor.ID);
      ActorInfo.Create(actor.ActorFactory, acinfo);
    }
  }
}
