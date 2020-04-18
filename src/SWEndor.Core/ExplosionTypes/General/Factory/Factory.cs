using SWEndor.Core;
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

      public void Register(ExplosionTypeInfo atype)
      {
        Put(atype.ID, atype);
        Engine.Screen2D.LoadingTextLines.Add(string.Format("{0} loaded!", atype.Name));
      }

      public void Load()
      {
        foreach (string fp in Directory.GetFiles(Globals.ExplosionTypeINIDirectory, Globals.INIExt, SearchOption.AllDirectories))
        {
          string f = Path.GetFileNameWithoutExtension(fp);
          Logger.Log(Logger.DEBUG, LogType.ASSET_LOADING, "ExplosionType", f);
          if (Contains(f))
            throw new InvalidOperationException(TextLocalization.Get(TextLocalKeys.EXPLTYPE_INITWICE_ERROR).F(f));
          ExplosionTypeInfo t = new ExplosionTypeInfo(this, f, f);
          t.LoadFromINI(f, fp);
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
    }
  }
}
