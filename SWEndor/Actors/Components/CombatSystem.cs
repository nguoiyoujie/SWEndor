using MTV3D65;
using SWEndor.Actors.Data;
using SWEndor.Actors.Traits;
using System;

namespace SWEndor.Actors.Components
{
  public class CombatSystem
  {
    internal static void Deactivate(Engine engine, int id)
    {
      ActorInfo actor = engine.ActorFactory.Get(id);
      actor.CombatData.IsCombatObject = false;
    }
  }
}
