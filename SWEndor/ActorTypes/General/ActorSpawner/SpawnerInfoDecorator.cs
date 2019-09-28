using MTV3D65;

namespace SWEndor
{
  /// <summary>
  /// Static place-holder for data
  /// </summary>
  public static class SpawnerInfoDecorator
  {
    public static SpawnerInfo SD_Default
    {
      get
      {
        SpawnerInfo d = SpawnerInfo.Default;
        d.SpawnTypes = new string[] { "TIE" };
        d.SpawnMoveDelay = 4;
        d.SpawnInterval = 5;
        d.SpawnsRemaining = 60;
        d.SpawnLocations = new TV_3DVECTOR[]{ new TV_3DVECTOR(50, 0, -50)
                                               , new TV_3DVECTOR(50, 0, 50)
                                               , new TV_3DVECTOR(-50, 0, -50)
                                               , new TV_3DVECTOR(-50, 0, 50)
                                               };

        d.SpawnSpeed = -1;
        d.SpawnManualPositioningMult = new TV_3DVECTOR(0, -30, 0);
        return d;
      }
    }

    public static SpawnerInfo SDII_Default
    {
      get
      {
        SpawnerInfo d = SpawnerInfo.Default;
        d.SpawnTypes = new string[] { "TIE Interceptor" };
        d.SpawnMoveDelay = 4;
        d.SpawnInterval = 5;
        d.SpawnsRemaining = 60;
        d.SpawnLocations = new TV_3DVECTOR[]{ new TV_3DVECTOR(50, 0, -50)
                                               , new TV_3DVECTOR(50, 0, 50)
                                               , new TV_3DVECTOR(-50, 0, -50)
                                               , new TV_3DVECTOR(-50, 0, 50)
                                               };

        d.SpawnSpeed = -1;
        d.SpawnManualPositioningMult = new TV_3DVECTOR(0, -30, 0);
        return d;
      }
    }

    public static SpawnerInfo PlayerSpawn_Default
    {
      get
      {
        SpawnerInfo d = SpawnerInfo.Default;
        d.SpawnMoveDelay = 3;
        d.SpawnInterval = 5;
        d.SpawnsRemaining = 0;
        d.SpawnLocations = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 0, 0) };
        return d;
      }
    }

    public static SpawnerInfo NebB_Default
    {
      get
      {
        SpawnerInfo d = SpawnerInfo.Default;
        d.SpawnTypes = new string[] { "Z-95", "Y-Wing" };
        d.SpawnMoveDelay = 2;
        d.SpawnInterval = 10;
        d.SpawnsRemaining = 99;
        d.SpawnLocations = new TV_3DVECTOR[] { new TV_3DVECTOR(-20, 0, 0) };
        d.SpawnSpeed = 25;
        d.SpawnRotation = new TV_3DVECTOR(0, 90, 0);
        return d;
      }
    }

    public static SpawnerInfo MC90_Default
    {
      get
      {
        SpawnerInfo d = SpawnerInfo.Default;
        d.SpawnTypes = new string[] { "X-Wing", "A-Wing", "Y-Wing", "B-Wing" };
        d.SpawnMoveDelay = 2.5f;
        d.SpawnInterval = 10;
        d.SpawnsRemaining = 99;
        d.SpawnLocations = new TV_3DVECTOR[] { new TV_3DVECTOR(-20, 0, 0) };
        d.SpawnSpeed = 125;
        d.SpawnRotation = new TV_3DVECTOR(0, 90, 0);
        return d;
      }
    }

    public static SpawnerInfo MCL_Default
    {
      get
      {
        SpawnerInfo d = SpawnerInfo.Default;
        d.SpawnTypes = new string[] { "Z-95", "Y-Wing" };
        d.SpawnMoveDelay = 2.5f;
        d.SpawnInterval = 15;
        d.SpawnsRemaining = 99;
        d.SpawnLocations = new TV_3DVECTOR[] { new TV_3DVECTOR(-20, 0, 0) };
        d.SpawnSpeed = 125;
        d.SpawnRotation = new TV_3DVECTOR(0, 90, 0);
        return d;
      }
    }
  }
}
