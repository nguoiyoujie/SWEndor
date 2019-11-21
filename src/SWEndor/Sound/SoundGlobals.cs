namespace SWEndor.Sound
{
  public static class SoundGlobals
  {
    // placeholder class, eventually this will be moved to a data file
    public static string Button1 = @"button\button_1";
    public static string Button2 = @"button\button_2";
    public static string Button3 = @"button\button_3";
    public static string Button4 = @"button\button_4";
    public static string Ready = @"button\ready";
    public static string Exit = @"button\exit";

    public static string ExpHit = @"exp\hit";
    public static string LowHP = @"dmg\low";
    public static string[] DmgSounds = new string[] { @"dmg\d1", @"dmg\d2", @"dmg\d3", @"dmg\d4" };
    public static string MissileAlert = @"warn\mislalert";
    public static string LostShip = @"warn\shiplost";

    // tied to units only, can be eliminated after units are moved to placeholder
    public static string[] ExpSm = new string[] { @"exp\sm1", @"exp\sm2", @"exp\sm3", @"exp\sm4" };
    public static string[] ExpMd = new string[] { @"exp\med1", @"exp\med2", @"exp\med3" };
    public static string ExpLg = @"exp\lg";

    public static string EngineTie = @"eng\tie";
    public static string EngineMissile = @"eng\misl";
    public static string EngineXWing = @"eng\xw";
    public static string EngineYWing = @"eng\yw";
    public static string EngineAWing = @"eng\aw";
    public static string EngineFalcon = @"eng\falcon";
    public static string EngineShip = @"eng\ship";




  }
}
