using MTV3D65;
using Primrose.FileFormat.INI;
using Primrose.Primitives.ValueTypes;
using SWEndor.Game.Core;
using System.IO;

namespace SWEndor.Game.Models
{
  internal struct ParticleSystemData
  {
    [INIValue]
    public string TexturePath;

    [INIValue]
    public float EmitterPower;

    [INIValue]
    public int EmitteParticleLimit; 

    [INIValue]
    public float EmitteParticlePerSecond; // number of particles in a second: spawn time = EmitteParticleLimit / EmitterParticleRate

    [INIValue]
    public float ParticleLifeTime;

    [INIValue]
    public float ParticleWidth;

    [INIValue]
    public float ParticleHeight;

    //[INIValue]
    //public CONST_TV_EMITTERTYPE EmitterType;

    [INIValue]
    public CONST_TV_EMITTERSHAPE Shape;

    [INIValue]
    public float3 Direction;

    [INIValue]
    public float3 DirectionRandom;

    [INIValue]
    public float4 Color;

    [INIValue]
    public float3 GravityDirection;

    // Derived
    public int ParticleTexture;
    public TVParticleSystem ParticleSystem;

    public static void Init(Engine engine, ref ParticleSystemData data)
    {
      data.ParticleSystem = engine.TrueVision.TVScene.CreateParticleSystem();
      string texpath = Path.Combine(Globals.ImagePath, data.TexturePath);
      data.ParticleTexture = engine.TrueVision.TVTextureFactory.LoadTexture(texpath, Path.GetFileNameWithoutExtension(texpath), -1, -1, CONST_TV_COLORKEY.TV_COLORKEY_USE_ALPHA_CHANNEL, true);
    }

    public void Process()
    {
      ParticleSystem.Update();
    }
  }
}
