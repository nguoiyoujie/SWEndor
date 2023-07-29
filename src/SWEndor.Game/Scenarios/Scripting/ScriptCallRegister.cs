using Primrose.Expressions;
using Primrose.Primitives.Factories;
using SWEndor.Game.Actors;
using SWEndor.Game.Scenarios.Scripting.Functions;
using System.Collections.Generic;

namespace SWEndor.Game.Scenarios.Scripting
{
  internal class ScriptCallRegister
  {
    private readonly List<string> _functions = new List<string>();

    public void Add(string function)
    {
      _functions.Add(function);
    }

    public void Clear() { _functions.Clear(); }

    public void Call(ActorInfo actor)
    {
      if (_functions.Count > 0)
      {
        List<Val> v = ObjectPool<List<Val>>.GetStaticPool().GetNew();
        v.Add(Val.NULL);
        v.Add(new Val(actor.ID));
        foreach (string script in _functions)
        {
          v[0] = new Val(script);
          ScriptingFns.Call(actor.Engine.ScriptContext, v);
        }
        ObjectPool<List<Val>>.GetStaticPool().Return(v);
      }
    }

    public void Call(ActorInfo actor, ActorInfo target)
    {
      if (_functions.Count > 0)
      {
        List<Val> v = ObjectPool<List<Val>>.GetStaticPool().GetNew();
        v.Add(Val.NULL);
        v.Add(new Val(actor.ID));
        v.Add(new Val(target?.ID ?? -1));
        foreach (string script in _functions)
        {
          v[0] = new Val(script);
          ScriptingFns.Call(actor.Engine.ScriptContext, v);
        }
        ObjectPool<List<Val>>.GetStaticPool().Return(v);
      }
    }
  }
}
