using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class SceneRoomATI : StaticSceneGroup
  {
    private static SceneRoomATI _instance;
    public static SceneRoomATI Instance()
    {
      if (_instance == null) { _instance = new SceneRoomATI(); }
      return _instance;
    }

    private SceneRoomATI() : base("Scene Room")
    {
      /*
      float min_x = -40000.0f;
      float max_x = 40000.0f;
      float min_z = -40000.0f;
      float max_z = 40000.0f;
      float bottom = -25000.0f;
      float height = 40000.0f;
      float tilesize = 120000.0f;
      */

      SourceMesh = Engine.Instance().TVScene.CreateMeshBuilder("room");

      string roombg = Path.Combine(Globals.ImagePath, @"stars_big.bmp");
      int texstars = Engine.Instance().TVTextureFactory.LoadTexture(roombg, roombg);

      SourceMesh.CreateSphere(60000.0f);
      SourceMesh.SetTexture(texstars);
      SourceMesh.SetCullMode(CONST_TV_CULLING.TV_FRONT_CULL);
      
      /*
      // walls
      SourceMesh.AddWall(Engine.Instance().TVGlobals.GetTex(@"stars4.bmp"), max_x, min_z, min_x, min_z, height, bottom, (max_x - min_x)/tilesize, (height) / tilesize);
      SourceMesh.AddWall(Engine.Instance().TVGlobals.GetTex(@"stars4.bmp"), min_x, min_z, min_x, max_z, height, bottom, (max_z - min_z) / tilesize, (height) / tilesize);
      SourceMesh.AddWall(Engine.Instance().TVGlobals.GetTex(@"stars4.bmp"), min_x, max_z, max_x, max_z, height, bottom, (max_x - min_x) / tilesize, (height) / tilesize);
      SourceMesh.AddWall(Engine.Instance().TVGlobals.GetTex(@"stars4.bmp"), max_x, max_z, max_x, min_z, height, bottom, (max_z - min_z) / tilesize, (height) / tilesize);
      SourceMesh.AddWall(Engine.Instance().TVGlobals.GetTex(@"stars4.bmp"), max_x, min_z * 0.4f, max_x * 0.4f, min_z, height, bottom, (max_x - min_x) / tilesize, (height) / tilesize);
      SourceMesh.AddWall(Engine.Instance().TVGlobals.GetTex(@"stars4.bmp"), min_x * 0.4f, min_z, min_x, min_z * 0.4f, height, bottom, (max_x - min_x) / tilesize, (height) / tilesize);
      SourceMesh.AddWall(Engine.Instance().TVGlobals.GetTex(@"stars4.bmp"), min_x, max_z * 0.4f, min_x * 0.4f, max_z, height, bottom, (max_x - min_x) / tilesize, (height) / tilesize);
      SourceMesh.AddWall(Engine.Instance().TVGlobals.GetTex(@"stars4.bmp"), max_x * 0.4f, max_z, max_x, max_z * 0.4f, height, bottom, (max_x - min_x) / tilesize, (height) / tilesize);

      SourceMesh.AddFloor(Engine.Instance().TVGlobals.GetTex(@"stars4.bmp"), min_x, min_z, max_x, max_z, bottom, (max_x - min_x) / tilesize, (max_z - min_z) / tilesize);
      SourceMesh.AddFaceFromPoint(Engine.Instance().TVGlobals.GetTex(@"stars4.bmp"), min_x, height + bottom, min_z, max_x, height + bottom, min_z, min_x, height + bottom, max_z, max_x, height + bottom, max_z, (max_x - min_x) / tilesize, (max_z - min_z) / tilesize);
      */
      SourceMesh.SetCollisionEnable(false);
    }
  }
}


