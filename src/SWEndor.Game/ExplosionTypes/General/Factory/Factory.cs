using SWEndor.Game.Core;
using Primrose.Primitives.Extensions;
using Primrose.Primitives.Factories;
using System;
using System.IO;
using Primrose;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SWEndor.Game.ExplosionTypes
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
        Engine.Screen2D.AppendLoadingText(string.Format("Explosion: {0} loaded!", atype.Name));
        Log.Info(Globals.LogChannel, LogDecorator.GetFormat(LogType.ASSET_LOADED), "ExplosionType", atype.Name);
      }

      public void Load()
      {
        Registry<string> paths = new Registry<string>();
        foreach (string fp in Directory.GetFiles(Globals.ExplosionTypeINIDirectory, Globals.INIExt, SearchOption.AllDirectories))
        {
          string f = Path.GetFileNameWithoutExtension(fp);
          if (Contains(f))
            throw new InvalidOperationException(TextLocalization.Get(TextLocalKeys.EXPLTYPE_INITWICE_ERROR).F(f));
          paths.Add(f, fp);
        }

        Parallel.ForEach(paths, new ParallelOptions { MaxDegreeOfParallelism = 32 }, LoadOne);
      }

      private void LoadOne(KeyValuePair<string, string> kvp)
      {
        string id = kvp.Key;
        string source = kvp.Value;
        Log.Info(Globals.LogChannel, LogDecorator.GetFormat(LogType.ASSET_LOADING), "ExplosionType", id);
        ExplosionTypeInfo t = new ExplosionTypeInfo(this, id, id);
        t.LoadFromINI(id, source);
        Register(t);
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
