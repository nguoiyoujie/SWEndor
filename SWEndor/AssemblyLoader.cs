using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace SWEndor
{
  public static class AssemblyLoader
  {
    public static bool Load(string path)
    {
      FileInfo info = new FileInfo(path);
      if (info.Exists)
      {
        Assembly ass = Assembly.LoadFrom(info.FullName);
        ass.EntryPoint.Invoke(null, null);
        return true;
      }
      return false;
    }
  }
}
