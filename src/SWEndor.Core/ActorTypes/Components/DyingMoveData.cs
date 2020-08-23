using SWEndor.Actors;
using SWEndor.Core;
using Primrose.FileFormat.INI;
using Primrose.Primitives.ValueTypes;
using Primrose.Primitives.Factories;

namespace SWEndor.ActorTypes.Components
{
  internal static class DyingMoveMethod
  {
    internal delegate void DyingMoveInitDelegate(Engine e, ActorInfo a, ref float3 f);
    internal delegate void DyingMoveUpdateDelegate(ActorInfo a, float3 f, float t);

    internal static void _killInit(Engine e, ActorInfo a, ref float3 d)
    { 
      a.SetState_Dead(); 
    }

    internal static void _spinInit(Engine e, ActorInfo a, ref float3 d)
    {
      a.ApplyZBalance = false;
      a.MoveData.ResetTurn();
      a.MoveData.MaxTurnRate = d.x + (float)e.Random.NextDouble() * (d.y - d.x);
      if (e.Random.NextDouble() > 0.5)
        a.MoveData.MaxTurnRate = -a.MoveData.MaxTurnRate;
    }

    internal static void _spinUpdt(ActorInfo a, float3 d, float t)
    {
      a.Rotate(0, 0, a.MoveData.MaxTurnRate * t);
      a.MoveData.ResetTurn();
    }

    internal static void _sinkUpdt(ActorInfo a, float3 d, float t)
    {
      a.XTurnAngle += d.x * t;
      a.MoveAbsolute(d.y * t, -d.z * t, 0);
    }
  }

  internal struct DyingMoveData
  {
    private const string sSpin = "spin";
    private const string sSink = "sink";
    private const string sKill = "kill";

    private static readonly Registry<string, DyingMoveMethod.DyingMoveInitDelegate> r_init;
    private static readonly Registry<string, DyingMoveMethod.DyingMoveUpdateDelegate> r_updt;

    static DyingMoveData()
    {
      r_init = new Registry<string, DyingMoveMethod.DyingMoveInitDelegate>();
      r_init.Add(sSpin, DyingMoveMethod._spinInit);
      r_init.Add(sKill, DyingMoveMethod._killInit);
      r_init.Default = null;

      r_updt = new Registry<string, DyingMoveMethod.DyingMoveUpdateDelegate>();
      r_updt.Add(sSpin, DyingMoveMethod._spinUpdt);
      r_updt.Add(sSink, DyingMoveMethod._sinkUpdt);
      r_updt.Default = null;
    }

#pragma warning disable 0649 // values are filled by the attribute
    [INIValue]
    internal float3 Data;

    [INIValue]
    internal string Type;
#pragma warning restore 0649

    public void Initialize(Engine engine, ActorInfo actor) { r_init.Get(Type ?? "")?.Invoke(engine, actor, ref Data); }

    public void Update(ActorInfo actor, float time) { r_updt.Get(Type ?? "")?.Invoke(actor, Data, time); }
  }
}
