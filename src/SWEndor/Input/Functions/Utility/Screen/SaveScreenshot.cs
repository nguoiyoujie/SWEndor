using MTV3D65;
using SWEndor.Core;
using System;
using System.IO;

namespace SWEndor.Input.Functions.Utility.Screen
{
  public class SaveScreenshot : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_SYSRQ;
    public static string InternalName = "u_printscr";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process(Engine engine)
    {
      string filename = string.Concat("screenshot_", DateTime.Now.ToString("yyyyMMdd_hhmmss_fff"), ".png");
      engine.TrueVision.TVEngine.Screenshot(Path.Combine(Globals.ScreenshotsPath, filename), CONST_TV_IMAGEFORMAT.TV_IMAGE_PNG);
    }
  }
}
