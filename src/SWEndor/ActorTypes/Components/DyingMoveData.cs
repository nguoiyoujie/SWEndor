using MTV3D65;
using SWEndor.Actors;
using SWEndor.FileFormat.INI;
using System;

namespace SWEndor.ActorTypes.Components
{
  internal static class DyingMoveMethod
  {
    internal static Action<ActorInfo> _killInit = (a) => a.SetState_Dead();
    internal static Action<ActorInfo> _spinInit = (a) => a.ApplyZBalance = false;
    internal static Action<ActorInfo, TV_3DVECTOR, float> _spinUpdt = (a, d, t) =>
    {
      a.Rotate(0, 0, d.x * t);
      a.MoveData.ResetTurn();
    };
    internal static Action<ActorInfo, TV_3DVECTOR, float> _sinkUpdt = (a, d, t) =>
    {
      a.XTurnAngle += d.x * t;
      a.MoveAbsolute(d.y * t, -d.z * t, 0);
    };
  }

  public struct DyingMoveData
  {
    Action<ActorInfo> _init;
    Action<ActorInfo, TV_3DVECTOR, float> _update;
    TV_3DVECTOR _data;

    public void Kill()
    {
      _init = DyingMoveMethod._killInit;
      _update = null;
    }

    public void Spin(float minRate, float maxRate)
    {
      _data.y = minRate;
      _data.z = maxRate;
      _data.x = minRate + (float)Globals.Engine.Random.NextDouble() * (maxRate - minRate);
      if (Globals.Engine.Random.NextDouble() > 0.5)
        _data.x = -_data.x;

      _init = DyingMoveMethod._spinInit;
      _update = DyingMoveMethod._spinUpdt;
    }

    public void Sink(float pitchRate, float forwardRate, float sinkRate)
    {
      _data.x = pitchRate;
      _data.y = forwardRate;
      _data.z = sinkRate;

      _init = null;
      _update = DyingMoveMethod._sinkUpdt;
    }

    public void Initialize(ActorInfo actor) { _init?.Invoke(actor); }
    public void Update(ActorInfo actor, float time) { _update?.Invoke(actor, _data, time); }

    public void LoadFromINI(INIFile f, string sectionname)
    {
      TV_3DVECTOR d = f.GetTV_3DVECTOR(sectionname, "Data", _data);
      string t = f.GetStringValue(sectionname, "Type", "");
      if (t == "spin")
        Spin(d.y, d.z);
      else if (t == "sink")
        Sink(d.x, d.y, d.z);
      else
        Kill();
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      string t = "";
      if (_update == DyingMoveMethod._spinUpdt)
        t = "spin";
      else if (_update == DyingMoveMethod._sinkUpdt)
        t = "sink";

      f.SetStringValue(sectionname, "Type", t);
      f.SetTV_3DVECTOR(sectionname, "Data", _data);
    }
  }
}
