using MTV3D65;
using Primrose.Primitives.ValueTypes;
using Primitives.FileFormat.INI;
using SWEndor.Primitives.Extensions;

namespace SWEndor.Weapons
{
  internal struct WeapPortInfo
  {
    private const string sNone = "";

    [INIValue(sNone, "FirePositions")]
    public float3[] FirePos;

    [INIValue(sNone, "CooldownRate")]
    public float2 CooldownRate; // rate, random

    public float2[] UIPos;
    public int Index;
    public float Cooldown;

    public static WeapPortInfo Default = new WeapPortInfo
    {
      FirePos = new float3[0],
      UIPos = null,
      Index = 0,
      CooldownRate = new float2(1, 0),
      Cooldown = 0
    };

    public void Init()
    {
      UIPos = new float2[FirePos.Length];

      for (int i = 0; i < UIPos.Length; i++)
      {
        float x = FirePos[i].x;
        float y = FirePos[i].y;

        if (x != 0 || y != 0)
        {
          float absX = (x > 0) ? x : -x;
          float absY = (y > 0) ? y : -y;

          if (absX > absY)
          {
            x *= 32 / absX;
            y *= 32 / absX;
          }
          else
          {
            x *= 32 / absY;
            y *= 32 / absY;
          }
        }
        UIPos[i].x = x;
        UIPos[i].y = y;
      }
    }

    public TV_3DVECTOR GetFirePos()
    {
      if (FirePos.Length == 0)
        return new TV_3DVECTOR(0, 0, 0);

      if (Index >= FirePos.Length)
        Index = 0;

      return FirePos[Index].ToVec3();
    }

    public void Next()
    {
      Index++;
    }
  }
}
