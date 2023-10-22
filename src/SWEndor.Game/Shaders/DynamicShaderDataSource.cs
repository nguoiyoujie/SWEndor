using MTV3D65;
using Primrose;
using Primrose.Primitives.Extensions;
using Primrose.Primitives.Factories;
using SWEndor.Game.Actors;
using SWEndor.Game.Core;
using SWEndor.Game.Models;
using System;

namespace SWEndor.Game.Shaders
{
  public enum DynamicShaderDataSource
  {
    // global
    GAME_TIME,

    // generic
    CREATION_TIME,
    DYING_INTERVAL,
    DYING_TIME_REMAINING,

    // Actor
    HP_FRAC,
    SHD_FRAC,
    HULL_FRAC,
    SPEED,
    SPEED_FRAC,
    HYPERSPACE_FACTOR,
    PLAYER_HYPERSPACE_FACTOR
  }

  internal static class DynamicShaderDataSetters<T, TType, TCreate>
          where T :
      IEngineObject,
      ITyped<TType>,
      IActorCreateable<TCreate>,
      IDyingTime
      where TType :
      ITypeInfo<T>
  {
    private static Registry<DynamicShaderDataSource, Action<TVShader, string, T>> _functions = new Registry<DynamicShaderDataSource, Action<TVShader, string, T>>()
    {
      { DynamicShaderDataSource.GAME_TIME, (tv, s, t) => tv.SetEffectParamFloat(s, t.Engine.Game.GameTime) },
      { DynamicShaderDataSource.CREATION_TIME, (tv, s, t) => tv.SetEffectParamFloat(s, t.CreationTime) },
      { DynamicShaderDataSource.DYING_INTERVAL, (tv, s, t) => tv.SetEffectParamFloat(s, t.DyingDuration) },
      { DynamicShaderDataSource.DYING_TIME_REMAINING, (tv, s, t) => tv.SetEffectParamFloat(s, t.DyingTimeRemaining) },
      { DynamicShaderDataSource.HP_FRAC, (tv, s, t) => {if (t is ActorInfo a) {tv.SetEffectParamFloat(s, a.HP_Frac); } } },
      { DynamicShaderDataSource.SHD_FRAC, (tv, s, t) => {if (t is ActorInfo a) {tv.SetEffectParamFloat(s, a.Shd_Frac); } } },
      { DynamicShaderDataSource.HULL_FRAC, (tv, s, t) => {if (t is ActorInfo a) {tv.SetEffectParamFloat(s, a.Hull_Frac); } } },
      { DynamicShaderDataSource.SPEED, (tv, s, t) => {if (t is ActorInfo a) {tv.SetEffectParamFloat(s, a.MoveData.Speed); } } },
      { DynamicShaderDataSource.SPEED_FRAC, (tv, s, t) => {if (t is ActorInfo a) {tv.SetEffectParamFloat(s, a.MoveData.MaxSpeed == 0 ? 1 : a.MoveData.Speed / a.MoveData.MaxSpeed); } } },
      { DynamicShaderDataSource.HYPERSPACE_FACTOR, (tv, s, t) => {if (t is ActorInfo a) {tv.SetEffectParamFloat(s, a.HyperspaceFactor); } } },
      { DynamicShaderDataSource.PLAYER_HYPERSPACE_FACTOR, (tv, s, t) => { if (t.Engine.PlayerInfo.TempActor is ActorInfo a) {tv.SetEffectParamFloat(s, a.HyperspaceFactor); } else { tv.SetEffectParamFloat(s, 0); } } },
    };

    public static void Set(TVShader shader, string varname, DynamicShaderDataSource valuesource, T obj)
    {
      Action<TVShader, string, T> action = _functions.Get(valuesource);
      if (action == null)
      {
        Log.Info(Globals.LogChannel, LogDecorator.GetFormat(LogType.MISSING_SHADER_VALUE_SOURCE).F(valuesource));
        return;
      }

      action.Invoke(shader, varname, obj);
    }
  }
}
