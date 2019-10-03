using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.Core;
using SWEndor.Explosions;
using SWEndor.ExplosionTypes;

namespace SWEndor.Models
{
  public struct ExplodeModel<T> 
    where T : 
    IEngineObject, 
    IActorState, 
    IDyingTime,
    IMeshObject, 
    IActorDisposable,
    ITransformable
  {
    private ExplodeData[] _data;
    private ExplosionTypeInfo[] _typecache;
    private float[] _time;

    public void Init(ExplodeData[] data, ActorCreationInfo acinfo)
    {
      _data = data;
      _typecache = new ExplosionTypeInfo[data.Length];
      _time = new float[data.Length];

      //fill time
      for (int i = 0; i < data.Length; i++)
        _time[i] = acinfo.CreationTime;
    }

    public void Tick(T self, float time) { Process(self); }

    private bool Check(Engine engine, T a, ExplodeData exp)
    {
      int comparand = 0x10 << (-(int)a.ActorState); // hack to map ActorState to ExplodeTrigger, should probably revert

      return
        (!engine.Game.IsLowFPS() || !exp.Trigger.Has(ExplodeTrigger.DONT_CREATE_ON_LOWFPS))
        && (((ExplodeTrigger)comparand & exp.Trigger) > 0)
        && ((a.DyingTimeRemaining > 0) || !exp.Trigger.Has(ExplodeTrigger.ONLY_WHEN_DYINGTIME_NOT_EXPIRED));

      /*
    return
      (!engine.Game.IsLowFPS() || !exp.Trigger.Has(ExplodeTrigger.DONT_CREATE_ON_LOWFPS))
      && ((!a.IsDyingOrDead && exp.Trigger.Has(ExplodeTrigger.ON_NORMAL))
          || (a.IsDying && exp.Trigger.Has(ExplodeTrigger.ON_DYING))
          || (a.IsDead && exp.Trigger.Has(ExplodeTrigger.ON_DEATH)))
      && (!exp.Trigger.Has(ExplodeTrigger.ONLY_WHEN_DYINGTIME_NOT_EXPIRED) || (a.DyingTimer.TimeRemaining > 0));
      */
    }

    private void Process(T a)
    {
      if (a.DisposingOrDisposed)
        return;

      Engine engine = a.Engine;

      for (int i = 0; i < _data.Length; i++)
      {
        ExplodeData exp = _data[i];
        if (Check(engine, a, exp))
        {
          if (_time[i] < engine.Game.GameTime - 5f) // skip explosion effects that are delayed after more than 5 secs
            _time[i] = engine.Game.GameTime;

          if (_time[i] <= engine.Game.GameTime)
          {
            float rate = (exp.Rate <= 0) ? 1 : exp.Rate;
            float size = exp.Size;
            if (size == 0)
              size = 1;
            if (_typecache[i] == null)
              _typecache[i] = engine.ExplosionTypeFactory.Get(exp.Type);

            if (exp.Trigger.Has(ExplodeTrigger.CREATE_ON_MESHVERTICES))
              CreateOnMeshVertices(engine, a, i, rate, size, exp.Trigger.Has(ExplodeTrigger.ATTACH_TO_ACTOR));
            else
              CreateOnMeshCenter(engine, a, i, rate, size, exp.Trigger.Has(ExplodeTrigger.ATTACH_TO_ACTOR));
          }
        }
      }
    }

    private void CreateOnMeshVertices(Engine engine, T a, int i, float rate, float size, bool attach)
    {
      int cnt = a.GetVertexCount();
      while (_time[i] < engine.Game.GameTime && cnt > 0)
      {
        _time[i] += (float)engine.Random.NextDouble() * rate;
        int vertID = engine.Random.Next(0, cnt);
        TV_3DVECTOR vert = a.GetVertex(vertID);

        if (attach)
        {
          ExplosionInfo e = MakeExplosion(engine, _typecache[i], vert * a.Scale, size);
          e.AttachedActorID = a.ID;
        }
        else
        {
          TV_3DVECTOR v = a.GetRelativePositionXYZ(vert.x * a.Scale, vert.y * a.Scale, vert.z * a.Scale);
          ExplosionInfo e = MakeExplosion(engine, _typecache[i], v, size);
        }
      }
    }

    private void CreateOnMeshCenter(Engine engine, T a, int i, float rate, float size, bool attach)
    {
      _time[i] = engine.Game.GameTime + rate;
      if (attach)
      {
        ExplosionInfo e = MakeExplosion(engine, _typecache[i], default(TV_3DVECTOR), size);
        e.AttachedActorID = a.ID;
      }
      else
        MakeExplosion(engine, _typecache[i], a.GetPrevGlobalPosition(), size);
    }

    private static ExplosionInfo MakeExplosion(Engine engine, ExplosionTypeInfo type, TV_3DVECTOR globalPosition, float explSize)
    {
      ExplosionCreationInfo acinfo = new ExplosionCreationInfo(type);
      acinfo.Position = globalPosition;
      acinfo.InitialScale = explSize;
      return engine.ExplosionFactory.Create(acinfo);
    }
  }
}
