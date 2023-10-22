using SWEndor.Game.Core;
using Primrose.Primitives.Extensions;
using Primrose.Primitives.Factories;
using System;
using System.IO;
using Primrose;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SWEndor.Game.ParticleTypes
{
  public partial class ParticleTypeInfo
  {
    internal class Factory : Registry<ParticleTypeInfo>
    {
      public readonly Engine Engine;
      internal Factory(Engine engine)
      { Engine = engine; }

      public void Register(ParticleTypeInfo atype)
      {
        Put(atype.ID, atype);
        Engine.Screen2D.AppendLoadingText(string.Format("Particle: {0} loaded!", atype.Name));
        Log.Info(Globals.LogChannel, LogDecorator.GetFormat(LogType.ASSET_LOADED), "ParticleType", atype.Name);
      }

      public void Load()
      {
        Registry<string> paths = new Registry<string>();
        foreach (string fp in Directory.GetFiles(Globals.ParticleTypeINIDirectory, Globals.INIExt, SearchOption.AllDirectories))
        {
          string f = Path.GetFileNameWithoutExtension(fp);
          if (Contains(f))
            throw new InvalidOperationException(TextLocalization.Get(TextLocalKeys.PROJTYPE_INITWICE_ERROR).F(f));
          paths.Add(f, fp);
        }

        Parallel.ForEach(paths, new ParallelOptions { MaxDegreeOfParallelism = 8 }, LoadOne);
      }

      private void LoadOne(KeyValuePair<string, string> kvp)
      {
        string id = kvp.Key;
        string source = kvp.Value;
        Log.Info(Globals.LogChannel, LogDecorator.GetFormat(LogType.ASSET_LOADING), "ParticleType", id);
        ParticleTypeInfo t = new ParticleTypeInfo(this, id, id);
        t.LoadFromINI(id, source);
        Register(t);
      }

      public void Initialise()
      {
        foreach (ParticleTypeInfo atype in list.Values)
          atype.Init();
      }

      public new ParticleTypeInfo Get(string id)
      {
        ParticleTypeInfo ret = base.Get(id);
        if (ret == null)
          throw new Exception(TextLocalization.Get(TextLocalKeys.ACTORTYPE_INVALID_ERROR).F(id));

        return ret;
      }
    }
  }
}

