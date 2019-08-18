using MTV3D65;
using SWEndor.Actors.Data;
using SWEndor.Actors.Traits;
using System;

namespace SWEndor.Actors.Components
{
  public class CombatSystem
  {
    internal static void Deactivate(Engine engine, ActorInfo actor)
    {
      actor.CombatData.IsCombatObject = false;
    }
  }
}
