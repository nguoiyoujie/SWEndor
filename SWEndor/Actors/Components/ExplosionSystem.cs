using MTV3D65;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes;

namespace SWEndor.Actors.Components
{
  public static class ExplosionSystem
  {
    internal static void ProcessDying(Engine engine, ActorInfo actor)
    {
      ProcessDying(engine, actor, ref engine.ActorDataSet.ExplodeData[actor.dataID], ref engine.MeshDataSet.list[actor.dataID]);
    }

    private static void ProcessDying(Engine engine, ActorInfo actor, ref ExplodeData data, ref MeshData mdata)
    {
      if (actor.TypeInfo.IsExplosion) // don't let explosions create explosions.
        return;

      // Explosion
      if (!engine.Game.IsLowFPS())
      {
        if (data.ExplosionCooldown < engine.Game.GameTime - 5f) // skip explosion effects that are delayed after more than 5 secs
          data.ExplosionCooldown = engine.Game.GameTime;

        while (data.ExplosionCooldown < engine.Game.GameTime && mdata.GetVertexCount() > 0)
        {
          int vertID = engine.Random.Next(0, mdata.GetVertexCount());
          data.ExplosionCooldown += (float)engine.Random.NextDouble() * data.ExplosionRate;
          MakeExplosion(engine, actor, mdata.GetVertex(vertID), ref data);
        }
      }
    }

    internal static void OnDeath(Engine engine, ActorInfo actor)
    {
      OnDeath(engine, actor, ref engine.ActorDataSet.ExplodeData[actor.dataID]);//, ref engine.TimedLifeDataSet.list[actor.dataID]);
    }

    private static void OnDeath(Engine engine, ActorInfo actor, ref ExplodeData data)//, ref TimedLifeData tdata)
    {
      if (actor.TypeInfo.IsExplosion) // don't let explosions create explosions.
        return;

      if (data.DeathExplosionTrigger == DeathExplosionTrigger.ALWAYS
        || (data.DeathExplosionTrigger == DeathExplosionTrigger.TIMENOTEXPIRED_ONLY 
          && actor.DyingTimer.TimeRemaining > 0)
        )
        MakeDeathExplosion(engine, actor, ref data);
    }

    private static void MakeExplosion(Engine engine, ActorInfo actor, TV_3DVECTOR vert, ref ExplodeData data)
    {
      float scale = actor.Scale;  

      if (data._cache == null)
        data._cache = actor.ActorTypeFactory.Get(data.ExplosionType);

      MakeExplosion(actor
        , data._cache
        , actor.GetRelativePositionXYZ(vert.x * scale, vert.y * scale, vert.z * scale)
        , data.ExplosionSize
        , ref data);
    }

    private static void MakeDeathExplosion(Engine engine, ActorInfo actor, ref ExplodeData data)
    {
      // Death explosion is one count, no cache needed
      TV_3DVECTOR pos = (actor.TypeInfo is ActorTypes.Groups.Projectile) ? actor.GetPrevGlobalPosition() : actor.GetGlobalPosition();
      MakeExplosion(actor, actor.ActorTypeFactory.Get(data.DeathExplosionType), pos, data.DeathExplosionSize, ref data);
      //MakeExplosion(actor, actor.ActorTypeFactory.Get(data.DeathExplosionType), actor.GetPosition(), data.DeathExplosionSize, ref data);
    }

    private static void MakeExplosion(ActorInfo actor, ActorTypeInfo type, TV_3DVECTOR globalPosition, float explSize, ref ExplodeData data)
    {
      ActorCreationInfo acinfo = new ActorCreationInfo(type);
      acinfo.Position = globalPosition;
      acinfo.InitialScale = explSize * actor.Scale;
      actor.ActorFactory.Create(acinfo);
    }
  }
}
