using SWEndor.Actors;
using SWEndor.Core;
using Primitives.FileFormat.INI;
using System;
using Primrose.Primitives.ValueTypes;
using Primrose.Primitives.Factories;

namespace SWEndor.ActorTypes.Components
{
  internal static class DyingMoveMethod
  {
    internal delegate void DyingMoveInitDelegate(Engine e, ActorInfo a, ref float3 f);

    internal static DyingMoveInitDelegate _killInit = delegate (Engine e, ActorInfo a, ref float3 d) { a.SetState_Dead(); };
    internal static DyingMoveInitDelegate _spinInit = delegate (Engine e, ActorInfo a, ref float3 d)
    {
      a.ApplyZBalance = false;
      a.MoveData.ResetTurn();
      a.MoveData.MaxTurnRate = d.x + (float)e.Random.NextDouble() * (d.y - d.x);
      if (e.Random.NextDouble() > 0.5)
        a.MoveData.MaxTurnRate = -a.MoveData.MaxTurnRate;
    };
    internal static Action<ActorInfo, float3, float> _spinUpdt = (a, d, t) =>
    {
      a.Rotate(0, 0, a.MoveData.MaxTurnRate * t);
      a.MoveData.ResetTurn();
    };
    internal static Action<ActorInfo, float3, float> _sinkUpdt = (a, d, t) =>
    {
      a.XTurnAngle += d.x * t;
      a.MoveAbsolute(d.y * t, -d.z * t, 0);
    };
  }

  internal struct DyingMoveData
  {
    private const string sDyingMove = "DyingMove";
    private const string sSpin = "spin";
    private const string sSink = "sink";
    private const string sKill = "kill";

    private static Registry<string, DyingMoveMethod.DyingMoveInitDelegate> r_init;
    private static Registry<string, Action<ActorInfo, float3, float>> r_updt;

    static DyingMoveData()
    {
      r_init = new Registry<string, DyingMoveMethod.DyingMoveInitDelegate>();
      r_init.Add(sSpin, DyingMoveMethod._spinInit);
      r_init.Add(sKill, DyingMoveMethod._killInit);
      r_init.Default = null;

      r_updt = new Registry<string, Action<ActorInfo, float3, float>>();
      r_updt.Add(sSpin, DyingMoveMethod._spinUpdt);
      r_updt.Add(sSink, DyingMoveMethod._sinkUpdt);
      r_updt.Default = null;
    }

#pragma warning disable 0649 // values are filled by the attribute
    [INIValue(sDyingMove, "Data")]
    internal float3 _data;

    [INIValue(sDyingMove, "Type")]
    internal string _type;
#pragma warning restore 0649

    public void Initialize(Engine engine, ActorInfo actor) { r_init.Get(_type ?? "")?.Invoke(engine, actor, ref _data); }

    public void Update(ActorInfo actor, float time) { r_updt.Get(_type ?? "")?.Invoke(actor, _data, time); }
  }
}
