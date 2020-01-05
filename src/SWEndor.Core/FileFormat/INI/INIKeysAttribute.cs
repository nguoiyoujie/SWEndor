using Primrose.Primitives.Extensions;
using Primrose.Primitives.Factories;
using Primrose.Primitives.ValueTypes;
using System;
using System.Collections.Generic;

namespace SWEndor.FileFormat.INI
{
  [AttributeUsage(AttributeTargets.Field)]
  public class INIKeyListAttribute : Attribute
  {
    public string Section;
    public INIKeyListAttribute(string section)
    {
      Section = section;
    }

    public string[] Read(INIFile f)
    {
      List<string> vals = new List<string>();
      if (f.HasSection(Section))
        foreach (INIFile.INISection.INILine ln in f.GetSection(Section).Lines)
          if (ln.HasKey)
            vals.Add(ln.Key);

      return vals.ToArray();
    }

    public void Write(INIFile f, string[] value)
    {
      foreach (string v in value)
        f.SetEmptyKey(Section, v);
    }
  }
}
