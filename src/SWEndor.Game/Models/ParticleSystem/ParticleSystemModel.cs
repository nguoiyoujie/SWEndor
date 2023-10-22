using MTV3D65;
using Primrose.Primitives;
using SWEndor.Game.Particles;
using SWEndor.Game.Primitives.Extensions;
using SWEndor.Game.Shaders;

namespace SWEndor.Game.Models
{
  internal struct ParticleSystemModel
  {
    // cache
    private TVParticleSystem ParticleSystem;
    private int ParticleID;
    private bool IsEmitterActive;
    private bool IsEmitterVisible;
    //private ShaderInfo ShaderInfo;
    //private TVShader Shader;




    private ScopeCounters.ScopeCounter emitterScope;
    private ScopeCounters.ScopeCounter disposeScope;


    public void Init(ShaderInfo.Factory shaderFactory, TV_3DVECTOR position, ref ParticleSystemData data)
    {
      if (emitterScope == null)
        emitterScope = new ScopeCounters.ScopeCounter();

      if (disposeScope == null)
        disposeScope = new ScopeCounters.ScopeCounter();

      GenerateEmitter(shaderFactory, position, ref data);

      ScopeCounters.Reset(disposeScope);
    }

    private void GenerateEmitter(ShaderInfo.Factory shaderFactory, TV_3DVECTOR position, ref ParticleSystemData data)
    {
      using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
      {
        ParticleSystem = data.ParticleSystem;
        if (data.EmitteParticleLimit <= 0) { data.EmitteParticleLimit = 1; }
        if (data.EmitteParticlePerSecond <= 0) { data.EmitteParticlePerSecond = 1; }
        ParticleID = ParticleSystem.CreateEmitter(CONST_TV_EMITTERTYPE.TV_EMITTER_BILLBOARD, data.EmitteParticleLimit);
        ParticleSystem.SetEmitterPosition(ParticleID, position);
        ParticleSystem.SetEmitterPower(ParticleID, data.EmitterPower, data.ParticleLifeTime);
        ParticleSystem.SetEmitterShape(ParticleID, data.Shape);
        ParticleSystem.SetBillboard(ParticleID, data.ParticleTexture, data.ParticleWidth, data.ParticleWidth);
        ParticleSystem.SetEmitterDirection(ParticleID, true, data.Direction.ToVec3(), data.DirectionRandom.ToVec3());
        ParticleSystem.SetEmitterSpeed(ParticleID, data.EmitteParticleLimit / data.EmitteParticlePerSecond);
        ParticleSystem.SetParticleDefaultColor(ParticleID, data.Color.ToVecColor());
        ParticleSystem.SetEmitterGravity(ParticleID, true, data.GravityDirection.ToVec3());
        IsEmitterActive = true;
        IsEmitterVisible = true;
        //ParticleSystem.SetBillboardShader()
      }
    }

    public void StopEmitting()
    {
      if (IsEmitterActive)
      {
        using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
        {
          ParticleSystem.SetEmitterPower(ParticleID, 0, 0);
          IsEmitterActive = false;
        }
      }
    }

    public void Update(ParticleInfo particle)
    {
      bool render = particle.Active && !particle.IsAggregateMode;
      if (particle.AttachedActorID != -1 && particle.ParentForCoords == null)
      {
        if (IsEmitterActive)
          StopEmitting();

        if (particle.DyingTimeRemaining > particle.TypeInfo.ParticleSystemData.ParticleLifeTime)
        {
          particle.DyingTimerSet(particle.TypeInfo.ParticleSystemData.ParticleLifeTime, true);
        }
      }
      else
      {
        using (ScopeCounters.Acquire(emitterScope))
          if (ScopeCounters.IsZero(disposeScope))
          {
            using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
            {
              ParticleSystem.SetEmitterPosition(ParticleID, particle.GetGlobalPosition());
            }

            if (render != IsEmitterVisible)
            {
              IsEmitterVisible = render;
              using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
                ParticleSystem.SetEmitterEnable(ParticleID, IsEmitterVisible);
            }
          }
      }
    }

    public void Dispose()
    {
      if (disposeScope != null && ScopeCounters.AcquireIfZero(disposeScope))
      {
        using (ScopeCounters.AcquireWhenZero(emitterScope))
        using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
        {
          ParticleSystem.DestroyEmitter(ParticleID);
        }
      }
    }
  }
}
