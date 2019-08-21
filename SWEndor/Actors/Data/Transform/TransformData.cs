﻿using MTV3D65;

namespace SWEndor.Actors
{
  public struct TransformData
  {
    public float Scale;
    public float Pitch;
    public float Yaw;
    public float Roll;
    public float X;
    public float Y;
    public float Z;

    public TV_3DVECTOR Position
    {
      get { return new TV_3DVECTOR(X, Y, Z); }
      set { X = value.x; Y = value.y; Z = value.z; }
    }

    public TV_3DVECTOR Rotation
    {
      get { return new TV_3DVECTOR(Pitch, Yaw, Roll); }
      set { Pitch = value.x; Yaw = value.y; Roll = value.z; }
    }

    public TV_3DVECTOR Direction
    {
      get { return Utilities.GetDirection(new TV_3DVECTOR(Pitch, Yaw, Roll)); }
      set
      {
        TV_3DVECTOR r = Utilities.GetRotation(value);
        Pitch = r.x; Yaw = r.y; Roll = r.z;
      }
    }
  }
}