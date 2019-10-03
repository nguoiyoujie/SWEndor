using MTV3D65;
using SWEndor.Core;
using System.IO;

namespace SWEndor.Shaders
{
  public partial class ShaderInfo
  {
    public string Name;
    public TVShader Shader;

    private ShaderInfo(Engine engine, string name)
    {
      Name = name;
      Shader = engine.TrueVision.TVScene.CreateShader(name);
      string shaderText = File.ReadAllText(Path.Combine(Globals.DataShadersPath, name + ".fx"));
      Shader.CreateFromEffectString(shaderText);
    }
  }
}
