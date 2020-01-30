using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;
using Primitives.FileFormat.INI;
using SWEndor.Models;
using Primrose.Primitives.Factories;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.Shaders
{
  public enum DynamicShaderDataSource
  {
    GAME_TIME,
    CREATION_TIME,
    DYING_INTERVAL,
    DYING_TIME_REMAINING,
    HP_FRAC,
    SPEED
  }

  public partial class ShaderInfo
  {
    public string Name;
    public Engine Engine;
    public TVShader TVShader;
    public TVScene TVScene;
    private Dictionary<string, bool> ConstBool = new Dictionary<string, bool>();
    private Dictionary<string, float> ConstFloat = new Dictionary<string, float>();
    private Dictionary<string, TV_2DVECTOR> ConstVec2 = new Dictionary<string, TV_2DVECTOR>();
    private Dictionary<string, TV_3DVECTOR> ConstVec3 = new Dictionary<string, TV_3DVECTOR>();
    private Dictionary<string, int> ConstTex = new Dictionary<string, int>();
    private Dictionary<string, int[]> RandTex = new Dictionary<string, int[]>();
    private Dictionary<string, DynamicShaderDataSource> DynamicParam = new Dictionary<string, DynamicShaderDataSource>();
    private ObjectPool<TVShader> _pool;
    private int _count;

    private ShaderInfo(Engine engine, string name)
    {
      Name = name;
      string dataFile = Path.Combine(Globals.DataShadersPath, Name + ".ini");
      if (File.Exists(dataFile))
      {
        INIFile f = new INIFile(dataFile);
        Parser.LoadFromINI(engine, this, f);
      }
      Engine = engine;
      TVScene = engine.TrueVision.TVScene;
      if (DynamicParam.Count > 0)
      {
        _pool = new ObjectPool<TVShader>(GenerateShader, null);
        int temp = _count;
        _count = 0;
        for (int i = 0; i < temp; i++)
          _pool.Return(GenerateShader());
      }
      else
      {
        TVShader = GenerateShader();
        _count = 1;
      }
    }

    public TVShader GetOrCreate()
    {
      return _pool?.GetNew() ?? TVShader;
    }

    private TVShader GenerateShader()
    {
      string shaderText = File.ReadAllText(Path.Combine(Globals.DataShadersPath, Name + ".fx"));
      TVShader shader = TVScene.CreateShader(Name + (++_count).ToString());
      shader.CreateFromEffectString(shaderText);
      string err = shader.GetLastError();
      if (err != null && err.Length != 0)
      {
        // log error
      }
      else
      {
        foreach (string s in ConstBool.Keys)
          shader.SetEffectParamBoolean(s, ConstBool[s]);

        foreach (string s in ConstFloat.Keys)
          shader.SetEffectParamFloat(s, ConstFloat[s]);

        foreach (string s in ConstVec2.Keys)
          shader.SetEffectParamVector2(s, ConstVec2[s]);

        foreach (string s in ConstVec3.Keys)
          shader.SetEffectParamVector3(s, ConstVec3[s]);

        foreach (string s in ConstTex.Keys)
          shader.SetEffectParamTexture(s, ConstTex[s]);

        foreach (string s in RandTex.Keys)
          shader.SetEffectParamTexture(s, RandTex[s][Engine.Random.Next(0, RandTex[s].Length)]);
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
      if (shader == null)
        return;

      foreach (string s in DynamicParam.Keys)
      {
        switch (DynamicParam[s])
        {
          case DynamicShaderDataSource.GAME_TIME:
            shader.SetEffectParamFloat(s, obj.Engine.Game.GameTime);
            break;

          case DynamicShaderDataSource.CREATION_TIME:
            shader.SetEffectParamFloat(s, obj.CreationTime);
            break;

          case DynamicShaderDataSource.DYING_INTERVAL:
            shader.SetEffectParamFloat(s, obj.DyingDuration);
            break;

          case DynamicShaderDataSource.DYING_TIME_REMAINING:
            shader.SetEffectParamFloat(s, obj.DyingTimeRemaining);
            break;

          case DynamicShaderDataSource.HP_FRAC:
            {
              ActorInfo a = obj as ActorInfo;
              if (a != null)
                shader.SetEffectParamFloat(s, a.HP_Frac);
              break;
            }
          case DynamicShaderDataSource.SPEED:
            {
              ActorInfo a = obj as ActorInfo;
              if (a != null)
                shader.SetEffectParamFloat(s, a.MoveData.Speed);
              break;
            }
        }
      }
    }
  }
}
