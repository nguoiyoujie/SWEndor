using Primrose.Primitives.ValueTypes;
using System;

namespace SWEndor.Game.Primitives.Geometry
{
  public struct Circle
  {
    public float2 Position { get; }
    public float Radius { get; }

    public Circle(float2 pos, float r)
    {
      Position = pos;
      Radius = r;
    }

    public bool Contains(float2 pos)
    {
      return (pos - Position).LengthSquared <= Radius * Radius;
    }

    public bool IntersectRayCircle(float2 rayStart, float2 rayEnd, out float2 intersetPoint)
    {
      float r2 = Radius * Radius;
      intersetPoint = rayEnd;
      if (rayStart == rayEnd || r2 <= 0)
      {
        return false;
      }

      float2 nearest = GetNearestPoint(Position, rayStart, rayEnd, false, false);
      float distanceSquared = float2.DistanceSquared(nearest, Position);

      if (distanceSquared > r2)
      {
        return false;
      }

      float2 offset = (rayEnd - rayStart).Normalize() * (float)Math.Sqrt(r2 - distanceSquared);

      if (float2.DistanceSquared(Position, rayStart) < r2)
      {
        intersetPoint = nearest + offset;
      }
      else
      {
        intersetPoint = nearest - offset;
      }
      return true;
    }

    private static float2 GetNearestPoint(float2 location, float2 start, float2 end, bool trimStart, bool trimEnd)
    {
      float2 posvector = location - start;
      float2 difvector = end - start;

      float magnitudeAB = difvector.LengthSquared;
      float ABAPproduct = float2.Dot(posvector, difvector);
      float distance = ABAPproduct / magnitudeAB;

      return (distance < 0 && trimStart) ? start : (distance > 1 && trimEnd) ? end : start + difvector * distance;
    }
  }
}
