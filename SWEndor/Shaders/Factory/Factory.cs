using SWEndor.Core;
using Primrose.Primitives.Factories;
using System.IO;

namespace SWEndor.Shaders
{
  public partial class ShaderInfo
  {
    public class Factory : Registry<ShaderInfo>
    {
      public readonly Engine Engine;
      internal Factory(Engine engine)
      { Engine = engine; }

      public void Load()
      {
        foreach (string fp in Directory.GetFiles(Globals.DataShadersPath, "*.fx", SearchOption.AllDirectories))
        {
          string f = Path.GetFileNameWithoutExtension(fp);
          ShaderInfo t = new ShaderInfo(Engine, f);
          Add(f, t);
        }
      }
    }
  }
}
