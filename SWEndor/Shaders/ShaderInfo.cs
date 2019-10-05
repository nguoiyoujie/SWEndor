using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.FileFormat.INI;
using SWEndor.Models;
using SWEndor.Primitives.Factories;
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
    public TVShader TVShader;
    public TVScene TVScene;
    private Dictionary<string, bool> ConstBool = new Dictionary<string, bool>();
    private Dictionary<string, float> ConstFloat = new Dictionary<string, float>();
    private Dictionary<string, TV_2DVECTOR> ConstVec2 = new Dictionary<string, TV_2DVECTOR>();
    private Dictionary<string, TV_3DVECTOR> ConstVec3 = new Dictionary<string, TV_3DVECTOR>();
    private Dictionary<string, int> ConstTex = new Dictionary<string, int>();
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
        LoadFromINI(engine, f);
      }
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
      }
      return shader;
    }

    public void ReturnShader(TVShader shader)
    {
      _pool?.Return(shader);
    }

    public void LoadFromINI(Engine engine, INIFile f)
    {
      string head = "BOOLEAN";
      if (f.HasSection(head))
        foreach (INIFile.INISection.INILine ln in f.GetSection(head).Lines)
        {
          if (ln.HasKey)
          {
            string key = ln.Key;
            if (!ConstBool.ContainsKey(key))
            {
              bool val = f.GetBoolValue(head, ln.Key);
              ConstBool.Add(key, val);
            }
          }
        }

      head = "FLOAT";
      if (f.HasSection(head))
        foreach (INIFile.INISection.INILine ln in f.GetSection(head).Lines)
        {
          if (ln.HasKey)
          {
            string key = ln.Key;
            if (!ConstFloat.ContainsKey(key))
            {
              float val = f.GetFloatValue(head, ln.Key);
              ConstFloat.Add(key, val);
            }
          }
        }

      head = "VEC2";
      if (f.HasSection(head))
        foreach (INIFile.INISection.INILine ln in f.GetSection(head).Lines)
        {
          if (ln.HasKey)
          {
            string key = ln.Key;
            if (!ConstVec2.ContainsKey(key))
            {
              TV_2DVECTOR val = f.GetTV_2DVECTOR(head, ln.Key);
              ConstVec2.Add(key, val);
            }
          }
        }

      head = "VEC3";
      if (f.HasSection(head))
        foreach (INIFile.INISection.INILine ln in f.GetSection(head).Lines)
        {
          if (ln.HasKey)
          {
            string key = ln.Key;
            if (!ConstVec3.ContainsKey(key))
            {
              TV_3DVECTOR val = f.GetTV_3DVECTOR(head, ln.Key);
              ConstVec3.Add(key, val);
            }
          }
        }

      head = "TEXTURE";
      if (f.HasSection(head))
        foreach (INIFile.INISection.INILine ln in f.GetSection(head).Lines)
            {
          if (ln.HasKey)
          {
            string key = ln.Key;
            if (!ConstTex.ContainsKey(key))
            {
              string stex = f.GetStringValue(head, ln.Key);
              int t = engine.TrueVision.TVTextureFactory.LoadTexture(Path.Combine(Globals.ImagePath, stex.Trim()));
              ConstTex.Add(key, t);
            }
          }
        }

      head = "TEXTURE_RANDOM";
      if (f.HasSection(head))
        foreach (INIFile.INISection.INILine ln in f.GetSection(head).Lines)
        {
          if (ln.HasKey)
          {
            string key = ln.Key;
            if (!ConstTex.ContainsKey(key))
            {
              string[] stex = f.GetStringList(head, ln.Key, new string[0]);
              if (stex.Length >= 0)
              {
                int t = engine.TrueVision.TVTextureFactory.LoadTexture(Path.Combine(Globals.ImagePath, stex[engine.Random.Next(0, stex.Length)].Trim()));
                ConstTex.Add(key, t);
              }
            }
          }
        }

      head = "DYNAMIC";
      if (f.HasSection(head))
        foreach (INIFile.INISection.INILine ln in f.GetSection(head).Lines)
        {
          if (ln.HasKey)
          {
            string key = ln.Key;
            if (!DynamicParam.ContainsKey(key))
            {
              DynamicShaderDataSource val = f.GetEnumValue(head, ln.Key, DynamicShaderDataSource.GAME_TIME);
              DynamicParam.Add(key, val);
            }
          }
        }

      _count = f.GetIntValue("General", "InitialCount", 0);
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
