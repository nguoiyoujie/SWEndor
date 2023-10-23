using MTV3D65;
using SWEndor.Game.Core;
using SWEndor.Game.Models;
using Primrose.Primitives.Factories;
using System.IO;
using SWEndor.FileFormat.INI;
using Primrose;
using Primrose.Primitives.Extensions;
using Primrose.FileFormat.INI;
using SWEndor.Game.Primitives.Extensions;
using System.Collections.Generic;

namespace SWEndor.Game.Shaders
{

  public partial class ShaderInfo
  {
    public string Name;
    public Engine Engine;
    public TVShader TVShader;
    public TVScene TVScene;
    internal ShaderData Data;
    internal string ShaderText;
    private Registry<string, int> ConstTex = new Registry<string, int>();
    private Registry<string, int[]> RandTex = new Registry<string, int[]>();
    private ObjectPool<TVShader> _pool;
    private int _count;

    internal ObjectPool<TVShader> Pool { get { return _pool; } }

    private ShaderInfo(Engine engine, string name)
    {
      Name = name;
      string dataFile = Path.Combine(Globals.DataShadersPath, Name + ".ini");
      if (File.Exists(dataFile))
      {
        INIFile f = new INIFile(dataFile);
        f.LoadByAttribute(ref Data);
        Load(engine);
      }
      Engine = engine;
      TVScene = engine.TrueVision.TVScene;
      if (Data.DynamicValues == null || Data.DynamicValues.Count > 0)
      {
        _pool = new ObjectPool<TVShader>(true, GenerateShader, null);
        int temp = Data.InitialCount;
        _count = 0;
        List<TVShader> templist = new List<TVShader>();
        for (int i = 0; i < temp; i++)
        {
          templist.Add(_pool.GetNew());
        }

        foreach (TVShader s in templist)
        {
          _pool.Return(s);
        }
      }
      else
      {
        TVShader = GenerateShader();
        _count = 1;
      }
    }

    private void Load(Engine engine)
    {
      if (string.IsNullOrWhiteSpace(Data.ShaderFile))
      {
        Data.ShaderFile = Name + ".fx";
      }

      // load textures and convert to index
      ConstTex.Clear();
      if (Data.TexConstants != null)
        foreach (var kvp in Data.TexConstants)
        {
          string stex = kvp.Value;
          int t = engine.TrueVision.TVTextureFactory.LoadTexture(Path.Combine(Globals.ImagePath, stex), stex);
          ConstTex.Add(kvp.Key, t);
        }

      RandTex.Clear();
      if (Data.TexRndConstants != null)
        foreach (var kvp in Data.TexRndConstants)
        {
          string[] stexs = kvp.Value;
          if (stexs.Length >= 0)
          {
            int[] ts = new int[stexs.Length];
            for (int i = 0; i < stexs.Length; i++)
            {
              string stex = stexs[i];
              ts[i] = engine.TrueVision.TVTextureFactory.LoadTexture(Path.Combine(Globals.ImagePath, stex), stex);
            }
            RandTex.Add(kvp.Key, ts);
          }
        }
    }

    public TVShader GetOrCreate()
    {
      return _pool?.GetNew() ?? TVShader;
    }

    private TVShader GenerateShader()
    {
      if (string.IsNullOrEmpty(ShaderText))
      {
        ShaderText = File.ReadAllText(Path.Combine(Globals.DataShadersPath, Data.ShaderFile));
      }
      TVShader shader = TVScene.CreateShader(Name + (++_count).ToString());
      shader.CreateFromEffectString(ShaderText);
      string err = shader.GetLastError();
      if (err != null && err.Length != 0)
      {
        // log error
        Log.Warn(Globals.LogChannel, LogDecorator.GetFormat(LogType.SHADER_LOAD_ERROR).F(Name, err));
      }
      else
      {
        foreach (var kvp in Data.BoolConstants)
          shader.SetEffectParamBoolean(kvp.Key, kvp.Value);

        foreach (var kvp in Data.FloatConstants)
          shader.SetEffectParamFloat(kvp.Key, kvp.Value);

        foreach (var kvp in Data.Vec2Constants)
          shader.SetEffectParamVector2(kvp.Key, kvp.Value.ToVec2());

        foreach (var kvp in Data.Vec3Constants)
          shader.SetEffectParamVector3(kvp.Key, kvp.Value.ToVec3());

        foreach (var kvp in ConstTex)
          shader.SetEffectParamTexture(kvp.Key, kvp.Value);

        foreach (var kvp in RandTex)
          shader.SetEffectParamTexture(kvp.Key, RandTex[kvp.Key][Engine.Random.Next(0, RandTex[kvp.Key].Length)]);

        Log.Info(Globals.LogChannel, LogDecorator.GetFormat(LogType.SHADER_LOADED).F(Name));
      }
      return shader;
    }

    public void ReturnShader(TVShader shader)
    {
      _pool?.Return(shader);
    }

    public void SetShaderParam<T, TType, TCreate>(T obj, TVShader shader)
      where T :
      IEngineObject,
      ITyped<TType>,
      IActorCreateable<TCreate>,
      IDyingTime
      where TType :
      ITypeInfo<T>
    {
      if (shader == null || Data.DynamicValues == null)
        return;

      foreach (string key in Data.DynamicValues.EnumerateKeys())
      {
        DynamicShaderDataSetters<T, TType, TCreate>.Set(shader, key, Data.DynamicValues[key], obj);
      }
    }
  }
}
