using SWEndor.Game.Actors.Models;
using Primrose.FileFormat.INI;

namespace SWEndor.Game.ActorTypes.Components
{
  internal struct SystemInstrumentData
  {
    [INIValue]
    public SystemPart PartType;

    [INIValue]
    public int Endurance; // amount of hits the instrument can take

    [INIValue]
    public float DamageChance;

    [INIValue]
    public float RecoveryTime;

    [INIValue]
    public float RecoveryTimeRandom;

    [INIValue]
    public int RecoveryEndurance;
  }
}
