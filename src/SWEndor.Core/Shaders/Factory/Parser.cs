using MTV3D65;
using SWEndor.Core;
using Primitives.FileFormat.INI;
using SWEndor.Primitives.Extensions;
using System.IO;
using Primrose.Primitives.ValueTypes;

namespace SWEndor.Shaders
{
  public partial class ShaderInfo
  {
    public static class Parser
    {
      public static void LoadFromINI(Engine engine, ShaderInfo s, INIFile f)
      {
        string head = "BOOLEAN";
        if (f.HasSection(head))
          foreach (INIFile.INISection.INILine ln in f.GetSection(head).Lines)
          {
            if (ln.HasKey)
            {
              string key = ln.Key;
              if (!s.ConstBool.ContainsKey(key))
              {
                bool val = f.GetValue<bool>(head, ln.Key);
                s.ConstBool.Add(key, val);
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
              if (!s.ConstFloat.ContainsKey(key))
              {
                float val = f.GetValue<float>(head, ln.Key);
                s.ConstFloat.Add(key, val);
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
              if (!s.ConstVec2.ContainsKey(key))
              {
                TV_2DVECTOR val = f.GetValue<float2>(head, ln.Key).ToVec2();
                s.ConstVec2.Add(key, val);
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
              if (!s.ConstVec3.ContainsKey(key))
              {
                TV_3DVECTOR val = f.GetValue<float3>(head, ln.Key).ToVec3();
                s.ConstVec3.Add(key, val);
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
              if (!s.ConstTex.ContainsKey(key))
              {
                string stex = f.GetString(head, ln.Key);
                int t = engine.TrueVision.TVTextureFactory.LoadTexture(Path.Combine(Globals.ImagePath, stex.Trim()), stex);
                s.ConstTex.Add(key, t);
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
              if (!s.RandTex.ContainsKey(key))
              {
                string[] stexs = f.GetValue(head, ln.Key, new string[0]);
                if (stexs.Length >= 0)
                {
                  int[] ts = new int[stexs.Length];
                  for (int i = 0; i < stexs.Length; i++)
                  {
                    string stex = stexs[i];
                    ts[i] = engine.TrueVision.TVTextureFactory.LoadTexture(Path.Combine(Globals.ImagePath, stex.Trim()), stex);
                  }
                  s.RandTex.Add(key, ts);
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
              if (!s.DynamicParam.ContainsKey(key))
              {
                DynamicShaderDataSource val = f.GetValue(head, ln.Key, DynamicShaderDataSource.GAME_TIME);
                s.DynamicParam.Add(key, val);
              }
            }
          }

        s._count = f.GetValue("General", "InitialCount", 0);
      }

    }
  }
}
