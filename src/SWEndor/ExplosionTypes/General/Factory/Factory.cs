using SWEndor.Core;
using SWEndor.ExplosionTypes.Instances;
using Primrose.Primitives.Extensions;
using Primrose.Primitives.Factories;
using System;
using System.IO;

namespace SWEndor.ExplosionTypes
{
  public partial class ExplosionTypeInfo
  {
    public class Factory : Registry<ExplosionTypeInfo>
    {
      public readonly Engine Engine;
      internal Factory(Engine engine)
      { Engine = engine; }

      public void RegisterBase()
      {
        // explosions
        Register(new ExpS00(this));
        Register(new ExpL00(this));
        Register(new ExpL01(this));
        Register(new ExpL02(this));
        Register(new ExpW01(this));
        Register(new ExpW02(this));
        Register(new ElectroATI(this));
      }

      public void Register(ExplosionTypeInfo atype)
      {
        if (Contains(atype.ID))
        {
          atype = GetX(atype.ID);
        }
        else
        {
          Add(atype.ID, atype);
        }
        atype.SaveToINI(atype.ID);
        Engine.Screen2D.LoadingTextLines.Add(string.Format("{0} loaded!", atype.Name));
      }

      public void Load()
      {
        foreach (string fp in Directory.GetFiles(Globals.ExplosionTypeINIDirectory, "*.ini", SearchOption.AllDirectories))
        {
          string f = Path.GetFileNameWithoutExtension(fp);
          if (Contains(f))
            throw new InvalidOperationException(TextLocalization.Get(TextLocalKeys.EXPLTYPE_INITWICE_ERROR).F(f));
          ExplosionTypeInfo t = new ExplosionTypeInfo(this, f, f);
          t.LoadFromINI(f);
          Register(t);
        }
      }

      public void Initialise()
      {
        foreach (ExplosionTypeInfo atype in list.Values)
          atype.Init();
      }

      public new ExplosionTypeInfo Get(string id)
      {
        ExplosionTypeInfo ret = base.Get(id);
        if (ret == null)
          throw new Exception(TextLocalization.Get(TextLocalKeys.EXPLTYPE_INVALID_ERROR).F(id));

        return ret;
      }

      /*
      public void LoadFromINI(string filepath)
      {
        if (File.Exists(filepath))
        {
          INIFile f = new INIFile(filepath);
          foreach (string s in f.Sections)
          {
            if (s != INIFile.PreHeaderSectionName)
              Register(Parser.Parse(this, f, s));
          }
        }
      }
      */
    }
  }
}
