using MTV3D65;
using Primrose.Primitives;
using Primrose.Primitives.ValueTypes;
using SWEndor.Game.ActorTypes.Components;
using SWEndor.Game.Core;
using SWEndor.Game.Explosions;
using SWEndor.Game.ExplosionTypes;
using SWEndor.Game.Particles;
using SWEndor.Game.ParticleTypes;
using SWEndor.Game.Primitives.Extensions;
using SWEndor.Game.ProjectileTypes;

namespace SWEndor.Game.Models
{
  internal struct ExplodeModel<T, U> 
    where T : 
    IEngineObject, 
    IIdentity<int>,
    IParent<U>,
    IActorState, 
    IDyingTime,
    IMeshObject, 
    IActorDisposable,
    ITransformable,
    IStunnable

   where U :
    IEngineObject,
    IIdentity<int>,
    IActorState,
    IDyingTime,
    IMeshObject,
    IActorDisposable,
    ITransformable,
    IStunnable
  {
    private ExplodeData[] _data;
    private ParticleData[] _pdata;
    private ExplosionTypeInfo[] _typecache;
    private ParticleTypeInfo[] _typepcache;
    private float[] _time;
    private int[] _pinstance;

    public void Init(ExplodeData[] data, ParticleData[] pdata, float creationTime)
    {
      _data = data;
      _pdata = pdata;
      _typecache = new ExplosionTypeInfo[data.Length];
      _typepcache = new ParticleTypeInfo[pdata.Length];
      _time = new float[data.Length];
      _pinstance = new int[pdata.Length];

      //fill time
      for (int i = 0; i < _time.Length; i++)
        _time[i] = creationTime;

      for (int i = 0; i < _pinstance.Length; i++)
        _pinstance[i] = -1;
    }

    public void Tick(T self, float time) { Process(self); }

    private bool Check(Engine engine, T a, ExplodeData exp)
    {
      if (engine.Game.IsLowFPS() && exp.Trigger.Has(ExplodeTrigger.DONT_CREATE_ON_LOWFPS))
        return false;

      if (a.DyingTimeRemaining < 0 && exp.Trigger.Has(ExplodeTrigger.ONLY_WHEN_DYINGTIME_NOT_EXPIRED)) // used in combination with ON_DEATH for impact deaths, ignoring decay. Like lasers
        return false;
      
      if (a.IsStunned && exp.Trigger.Has(ExplodeTrigger.WHILE_STUNNED)) // typically used to show stunned animation on ion'ed turbolaser hardpoints
        return true;

      ExplodeTrigger comparand;
      switch (a.ActorState)
      {
        default:
        case ActorState.NORMAL:
          comparand = ExplodeTrigger.WHILE_NORMAL;
          break;
        case ActorState.DYING:
          comparand = ExplodeTrigger.WHILE_DYING;
          break;
        case ActorState.DEAD:
          comparand = ExplodeTrigger.ON_DEATH;
          break;
      }
      return (comparand & exp.Trigger) > 0;
    }

    private bool Check(Engine engine, T a, ParticleData exp)
    {
      if (engine.Game.IsLowFPS() && exp.Trigger.Has(ExplodeTrigger.DONT_CREATE_ON_LOWFPS))
        return false;

      if (a.DyingTimeRemaining < 0 && exp.Trigger.Has(ExplodeTrigger.ONLY_WHEN_DYINGTIME_NOT_EXPIRED)) // used in combination with ON_DEATH for impact deaths, ignoring decay. Like lasers
        return false;

      if (a.IsStunned && exp.Trigger.Has(ExplodeTrigger.WHILE_STUNNED)) // typically used to show stunned animation on ion'ed turbolaser hardpoints
        return true;

      ExplodeTrigger comparand;
      switch (a.ActorState)
      {
        default:
        case ActorState.NORMAL:
          comparand = ExplodeTrigger.WHILE_NORMAL;
          break;
        case ActorState.DYING:
          comparand = ExplodeTrigger.WHILE_DYING;
          break;
        case ActorState.DEAD:
          comparand = ExplodeTrigger.ON_DEATH;
          break;
      }
      return (comparand & exp.Trigger) > 0;
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
            float3 offset = exp.PositionOffset;
            if (size == 0)
              size = 1;
            if (_typecache[i] == null)
              _typecache[i] = engine.ExplosionTypeFactory.Get(exp.Type);

            if (exp.Trigger.Has(ExplodeTrigger.ATTACH_TO_PARENT) && a.ParentForCoords != null)
            {
              U parent = a.ParentForCoords;
              if (exp.Trigger.Has(ExplodeTrigger.CREATE_ON_MESHVERTICES))
                CreateExplosionOnMeshVertices(engine, parent, i, rate, size, exp.Trigger.Has(ExplodeTrigger.ATTACH_TO_ACTOR));
              else
                CreateExplosionOnMesh(engine, parent, a.GetRelativePositionXYZ(offset.x, offset.y, offset.z, true), i, rate, size, exp.Trigger.Has(ExplodeTrigger.ATTACH_TO_ACTOR));
            }
            else
            {
              if (exp.Trigger.Has(ExplodeTrigger.CREATE_ON_MESHVERTICES))
                CreateExplosionOnMeshVertices(engine, a, i, rate, size, exp.Trigger.Has(ExplodeTrigger.ATTACH_TO_ACTOR));
              else
                CreateExplosionOnMesh(engine, a, offset.ToVec3(), i, rate, size, exp.Trigger.Has(ExplodeTrigger.ATTACH_TO_ACTOR));
            }
          }
        }
      }

      for (int i = 0; i < _pdata.Length; i++)
      {
        ParticleData prt = _pdata[i];
        if (_pinstance[i] == -1 || engine.ParticleFactory.Get(_pinstance[i]) == null) // check only if the current particle effect is not already active
        {
          if (Check(engine, a, prt))
          {
            //float duration = (prt.Duration <= 0) ? -1 : prt.Duration;
            float size = prt.Size;
            float3 offset = prt.PositionOffset;
            if (size == 0)
              size = 1;
            if (_typepcache[i] == null)
              _typepcache[i] = engine.ParticleTypeFactory.Get(prt.Type);

            if (prt.Trigger.Has(ExplodeTrigger.ATTACH_TO_PARENT) && a.ParentForCoords != null)
            {
              U parent = a.ParentForCoords;
              if (prt.Trigger.Has(ExplodeTrigger.CREATE_ON_MESHVERTICES))
                CreateParticleOnMeshVertices(engine, parent, i, size, prt.Trigger.Has(ExplodeTrigger.ATTACH_TO_ACTOR));
              else
                CreateParticleOnMesh(engine, parent, a.GetRelativePositionXYZ(offset.x, offset.y, offset.z, true), i, size, prt.Trigger.Has(ExplodeTrigger.ATTACH_TO_ACTOR));
            }
            else
            {
              if (prt.Trigger.Has(ExplodeTrigger.CREATE_ON_MESHVERTICES))
                CreateParticleOnMeshVertices(engine, a, i, size, prt.Trigger.Has(ExplodeTrigger.ATTACH_TO_ACTOR));
              else
                CreateParticleOnMesh(engine, a, offset.ToVec3(), i, size, prt.Trigger.Has(ExplodeTrigger.ATTACH_TO_ACTOR));
            }
          }
        }
      }
    }

    private void CreateExplosionOnMeshVertices<N>(Engine engine, N a, int i, float rate, float size, bool attach)
      where N :
        IEngineObject,
        IIdentity<int>,
        IMeshObject,
        ITransformable
    {
      int cnt = a.GetVertexCount();
      while (_time[i] < engine.Game.GameTime && cnt > 0)
      {
        _time[i] += (float)engine.Random.NextDouble() * rate;
        int vertID = engine.Random.Next(0, cnt);
        float3 vert = a.GetVertex(vertID).ToFloat3();
        TV_3DVECTOR final = (vert * a.Scale).ToVec3();

        if (attach)
        {
          ExplosionInfo e = MakeExplosion(engine, _typecache[i], final, size);
          e.AttachedActorID = a.ID;
        }
        else
        {
          TV_3DVECTOR v = a.GetRelativePositionXYZ(final.x, final.y, final.z);
          MakeExplosion(engine, _typecache[i], v, size);
        }
      }
    }

    private void CreateExplosionOnMesh<N>(Engine engine, N a, TV_3DVECTOR relativepos, int i, float rate, float size, bool attach)
      where N :
        IEngineObject,
        IIdentity<int>,
        IMeshObject,
        ITransformable
    {
      _time[i] = engine.Game.GameTime + rate;
      if (attach)
      {
        ExplosionInfo e = MakeExplosion(engine, _typecache[i], relativepos, size);
        e.AttachedActorID = a.ID;
      }
      else
      {
        TV_3DVECTOR offset = a.GetRelativePositionXYZ(relativepos.x, relativepos.y, relativepos.z) - a.GetGlobalPosition();
        MakeExplosion(engine, _typecache[i], a.GetPrevGlobalPosition() + offset, size);
      }
    }

    private void CreateParticleOnMeshVertices<N>(Engine engine, N a, int i, float size, bool attach)
      where N :
        IEngineObject,
        IIdentity<int>,
        IMeshObject,
        ITransformable
    {
      int cnt = a.GetVertexCount();
      // note: only one particle emitted is spawned
      int vertID = engine.Random.Next(0, cnt);
      float3 vert = a.GetVertex(vertID).ToFloat3();
      TV_3DVECTOR final = (vert * a.Scale).ToVec3();

      ParticleInfo p;
      if (attach)
      {
        p = MakeParticleEffect(engine, _typepcache[i], final, size);
        p.AttachedActorID = a.ID;
      }
      else
      {
        TV_3DVECTOR v = a.GetRelativePositionXYZ(final.x, final.y, final.z);
        p = MakeParticleEffect(engine, _typepcache[i], v, size);
      }

      _pinstance[i] = p.ID;
    }

    private void CreateParticleOnMesh<N>(Engine engine, N a, TV_3DVECTOR relativepos, int i, float size, bool attach)
      where N :
        IEngineObject,
        IIdentity<int>,
        IMeshObject,
        ITransformable
    {
      //_ptime[i] = engine.Game.GameTime + rate;
      ParticleInfo p;
      if (attach)
      {
        p = MakeParticleEffect(engine, _typepcache[i], relativepos, size);
        p.AttachedActorID = a.ID;
      }
      else
      {
        TV_3DVECTOR offset = a.GetRelativePositionXYZ(relativepos.x, relativepos.y, relativepos.z) - a.GetGlobalPosition();
        p = MakeParticleEffect(engine, _typepcache[i], a.GetPrevGlobalPosition() + offset, size);
      }
      _pinstance[i] = p.ID;
    }

    private static ExplosionInfo MakeExplosion(Engine engine, ExplosionTypeInfo type, TV_3DVECTOR globalPosition, float explSize)
    {
      ExplosionCreationInfo acinfo = new ExplosionCreationInfo(type)
      {
        Position = globalPosition,
        InitialScale = explSize
      };
      return engine.ExplosionFactory.Create(acinfo);
    }

    private static ParticleInfo MakeParticleEffect(Engine engine, ParticleTypeInfo type, TV_3DVECTOR globalPosition, float partSize)
    {
      ParticleCreationInfo acinfo = new ParticleCreationInfo(type)
      {
        Position = globalPosition,
        InitialScale = partSize
      };
      return engine.ParticleFactory.Create(acinfo);
    }
  }
}
