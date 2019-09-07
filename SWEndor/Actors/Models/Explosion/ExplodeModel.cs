
using MTV3D65;
using SWEndor.ActorTypes;
using System;

namespace SWEndor.Actors
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

  public static class ExplodeTriggerExt
  {
    public static bool Has(this ExplodeTrigger mask, ExplodeTrigger subset)
    {
      return (mask & subset) == subset;
    }
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

  public partial class ActorInfo
  {
    private struct ExplodeModel
    {
      private ExplodeInfo[] _data;
      private ActorTypeInfo[] _types;
      private float[] _time;

      public void Init(ActorTypeInfo type, ActorCreationInfo acinfo)
      {
        _data = type.Explodes;
        _types = new ActorTypeInfo[type.Explodes.Length];
        _time = new float[type.Explodes.Length];

        //fill time
        for (int i = 0; i < type.Explodes.Length; i++)
          _time[i] = acinfo.CreationTime;
      }

      public void Tick(ActorInfo self, float time)
      {
        Process(self);
      }

      private bool Check(Engine engine, ActorInfo a, ExplodeInfo exp)
      {
        return
          (!engine.Game.IsLowFPS() || !exp.Trigger.Has(ExplodeTrigger.DONT_CREATE_ON_LOWFPS))
          && ((!a.IsDyingOrDead && exp.Trigger.Has(ExplodeTrigger.ON_NORMAL))
              || (a.IsDying && exp.Trigger.Has(ExplodeTrigger.ON_DYING))
              || (a.IsDead && exp.Trigger.Has(ExplodeTrigger.ON_DEATH)))
          && (!exp.Trigger.Has(ExplodeTrigger.ONLY_WHEN_DYINGTIME_NOT_EXPIRED) || (a.DyingTimer.TimeRemaining > 0));
      }

      private void Process(ActorInfo a)
      {
        if (a.DisposingOrDisposed)
          return;

        Engine engine = a.Engine;

        for (int i = 0; i < _data.Length; i++)
        {
          ExplodeInfo exp = _data[i];
          if (Check(engine, a, exp))
          {
            if (_time[i] < engine.Game.GameTime - 5f) // skip explosion effects that are delayed after more than 5 secs
              _time[i] = engine.Game.GameTime;

            if (_time[i] <= engine.Game.GameTime)
            {
              float rate = (exp.Rate <= 0) ? 1 : exp.Rate;
              float size = exp.Size * a.Scale;
              if (size == 0)
                size = 1;
              if (_types[i] == null)
                _types[i] = engine.ActorTypeFactory.Get(exp.ActorType);

              if (exp.Trigger.Has(ExplodeTrigger.CREATE_ON_MESHVERTICES))
                CreateOnMeshVertices(engine, a, i, rate, size);
              else
                CreateOnMeshCenter(engine, a, i, rate, size);
            }
          }
        }
      }

      private void CreateOnMeshVertices(Engine engine, ActorInfo a, int i, float rate, float size)
      {
        while (_time[i] < engine.Game.GameTime && a.GetVertexCount() > 0)
        {
          _time[i] += (float)engine.Random.NextDouble() * rate;
          int vertID = engine.Random.Next(0, a.GetVertexCount());
          TV_3DVECTOR vert = a.GetVertex(vertID);

          TV_3DVECTOR v = a.GetRelativePositionXYZ(vert.x * a.Scale, vert.y * a.Scale, vert.z * a.Scale);
          MakeExplosion(_types[i], v, size);
        }
      }

      private void CreateOnMeshCenter(Engine engine, ActorInfo a, int i, float rate, float size)
      {
        _time[i] = engine.Game.GameTime + rate;
        MakeExplosion(_types[i], a.GetPrevGlobalPosition(), size);
      }

      private static void MakeExplosion(ActorTypeInfo type, TV_3DVECTOR globalPosition, float explSize)
      {
        ActorCreationInfo acinfo = new ActorCreationInfo(type);
        acinfo.Position = globalPosition;
        acinfo.InitialScale = explSize;
        Globals.Engine.ActorFactory.Create(acinfo);
      }
    }

    public void TickExplosions() { Explosions.Tick(this, Game.GameTime); }
  }
}
