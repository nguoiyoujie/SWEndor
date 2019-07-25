using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.Primitives.Traits;
using System;
using System.Collections.Generic;

namespace SWEndor.Actors.Traits
{
  [Flags]
  public enum ExplodeTrigger
  {
    NONE = 0,
    ON_NORMAL = 0x1,
    ON_DYING = 0x2,
    ON_DEATH = 0x4,
    ONLY_WHEN_DYINGTIME_NOT_EXPIRED = 0x8,
    CREATE_ON_MESHVERTICES = 0x10,
    DONT_CREATE_ON_LOWFPS = 0x20
  }

  public struct ExplodeInfo
  {
    public string ActorType;
    public float Rate;
    public float Size;
    public ExplodeTrigger Trigger;

    public ExplodeInfo(string type, float rate, float size, ExplodeTrigger trigger)
    {
      ActorType = type;
      Rate = rate;
      Size = size;
      Trigger = trigger;
    }
  }

  public class Explodes : ITick, INotifyDying, INotifyDead
  {
    ExplodeInfo[] _data;
    ActorTypeInfo[] _types;
    float[] _time;

    public void Init(ActorTypeInfo type, ActorCreationInfo acinfo)
    {
      _data = new ExplodeInfo[type.Explodes.Length];
      _types = new ActorTypeInfo[type.Explodes.Length];
      _time = new float[type.Explodes.Length];

      //fill time
      for (int i = 0; i < type.Explodes.Length; i++)
      {
        _data[i] = type.Explodes[i];
        _time[i] = acinfo.CreationTime;
      }
    }

    void ITick.Tick<A>(A self, float time)
    {
      Process(self);
    }

    private bool Check(Engine engine, ExplodeInfo exp, IStateModel s, DyingTimer d)
    {
      return
        (!engine.Game.IsLowFPS() || !exp.Trigger.HasFlag(ExplodeTrigger.DONT_CREATE_ON_LOWFPS))
        && ((s.ActorState == ActorState.NORMAL && exp.Trigger.HasFlag(ExplodeTrigger.ON_NORMAL))
            || (s.ActorState == ActorState.DYING && exp.Trigger.HasFlag(ExplodeTrigger.ON_DYING))
            || (s.ActorState == ActorState.DEAD && exp.Trigger.HasFlag(ExplodeTrigger.ON_DEATH)))
        && (!exp.Trigger.HasFlag(ExplodeTrigger.ONLY_WHEN_DYINGTIME_NOT_EXPIRED) || (d != null && d.TimeRemaining > 0));
    }

    private void Process<A>(A self) where A : ITraitOwner
    {
      IStateModel s = self.TraitOrDefault<IStateModel>();
      DyingTimer d = self.TraitOrDefault<DyingTimer>();
      IMeshModel m = self.TraitOrDefault<IMeshModel>();
      ITransform t = self.TraitOrDefault<ITransform>();
      Engine engine = Globals.Engine;

      if (s == null 
        || s.CreationState != CreationState.ACTIVE
        || t == null)
        return;

      for (int i = 0; i < _data.Length; i++)
      {
        ExplodeInfo exp = _data[i];
        if (Check(engine, exp, s, d))
        {
          if (_time[i] < engine.Game.GameTime - 5f) // skip explosion effects that are delayed after more than 5 secs
            _time[i] = engine.Game.GameTime;

          if (_time[i] <= engine.Game.GameTime)
          {
            float rate = (exp.Rate <= 0) ? 1 : exp.Rate;
            float size = exp.Size * ((t != null) ? t.Scale : 1);
            if (size == 0)
              size = 1;
            if (_types[i] == null)
              _types[i] = engine.ActorTypeFactory.Get(exp.ActorType);

            if (m != null && exp.Trigger.HasFlag(ExplodeTrigger.CREATE_ON_MESHVERTICES))
              CreateOnMeshVertices(engine, self, i, rate, size, m, t);
            else
              CreateOnMeshCenter(engine, self, i, rate, size, t);
          }
        }
      }
    }

    private void CreateOnMeshVertices<A>(Engine engine, A self, int i, float rate, float size, IMeshModel m, ITransform t) where A : ITraitOwner
    {
      while (_time[i] < engine.Game.GameTime && m.GetVertexCount() > 0)
      {
        _time[i] += (float)engine.Random.NextDouble() * rate;
        int vertID = engine.Random.Next(0, m.GetVertexCount());
        TV_3DVECTOR vert = m.GetVertex(vertID);

        TV_3DVECTOR v = t.GetRelativePositionXYZ(self, engine.Game.GameTime, vert.x * t.Scale, vert.y * t.Scale, vert.z * t.Scale);
        MakeExplosion(_types[i], v, size);
      }
    }

    private void CreateOnMeshCenter<A>(Engine engine, A self, int i, float rate, float size, ITransform t) where A : ITraitOwner
    {
      _time[i] = engine.Game.GameTime + rate;
      MakeExplosion(_types[i], t.GetPrevGlobalPosition(self, engine.Game.GameTime), size);
    }

    private static void MakeExplosion(ActorTypeInfo type, TV_3DVECTOR globalPosition, float explSize)
    {
      ActorCreationInfo acinfo = new ActorCreationInfo(type);
      acinfo.Position = globalPosition;
      acinfo.InitialScale = explSize;
      Globals.Engine.ActorFactory.Create(acinfo);
    }

    void INotifyDying.Dying<A1>(A1 self)
    {
      Process(self);
    }

    void INotifyDead.Dead<A1>(A1 self)
    {
      Process(self);
    }
  }
}
