﻿using SWEndor.ActorTypes.Instances;
using SWEndor.Core;
using SWEndor.ExplosionTypes.Instances;
using SWEndor.FileFormat.INI;
using SWEndor.Primitives.Factories;
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
        if (Contains(atype.Name))
        {
          atype = GetX(atype.Name);
        }
        else
        {
          Add(atype.Name, atype);
        }
        //atype.LoadFromINI();
        Engine.Screen2D.LoadingTextLines.Add(string.Format("{0} loaded!", atype.Name));
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
          throw new Exception(string.Format("ExplosionTypeInfo '{0}' does not exist", id));

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