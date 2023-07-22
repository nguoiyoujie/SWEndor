using Primrose.FileFormat.INI;
using Primrose.Primitives.Factories;
using Primrose.Primitives.ValueTypes;
using SWEndor.Game.Shaders;

namespace SWEndor.FileFormat.INI
{
  public struct ShaderData
  {
    [INIValue("General", "InitialCount")]
    public int InitialCount;

    [INIValue("General", "ShaderFile")]
    public string ShaderFile;

    [INIRegistry("BOOLEAN")]
    public Registry<string, bool> BoolConstants;

    [INIRegistry("FLOAT")]
    public Registry<string, float> FloatConstants;

    [INIRegistry("VEC2")]
    public Registry<string, float2> Vec2Constants;

    [INIRegistry("VEC3")]
    public Registry<string, float3> Vec3Constants;

    [INIRegistry("TEXTURE")]
    public Registry<string, string> TexConstants;

    [INIRegistry("TEXTURE_RANDOM")]
    public Registry<string, string[]> TexRndConstants;

    [INIRegistry("DYNAMIC")]
    public Registry<string, DynamicShaderDataSource> DynamicValues;
  }
}
