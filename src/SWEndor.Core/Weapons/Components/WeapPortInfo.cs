using MTV3D65;
using Primrose.Primitives.ValueTypes;
using Primitives.FileFormat.INI;
using SWEndor.Primitives.Extensions;

namespace SWEndor.Weapons
{
  internal struct WeapPortInfo
  {
    [INIValue]
    public float3[] FirePositions;

    [INIValue]
    public float2 CooldownRate; // rate, random

    public float2[] UIPos;
    public int Index;
    public float Cooldown;

    public static WeapPortInfo Default = new WeapPortInfo
    {
      FirePositions = new float3[0],
      UIPos = null,
      Index = 0,
      CooldownRate = new float2(1, 0),
      Cooldown = 0
    };

    public void Init()
    {
      UIPos = new float2[FirePositions.Length];

      for (int i = 0; i < UIPos.Length; i++)
      {
        float x = FirePositions[i].x;
        float y = FirePositions[i].y;

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
      if (FirePositions.Length == 0)
        return new TV_3DVECTOR(0, 0, 0);

      if (Index >= FirePositions.Length)
        Index = 0;

      return FirePositions[Index].ToVec3();
    }

    public void Next()
    {
      Index++;
    }
  }
}
