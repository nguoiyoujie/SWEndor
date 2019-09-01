﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTV3D65;
using SWEndor;
using SWEndor.Primitives;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.ActorTypes.Instances;

namespace SWEndorTest
{
  [TestClass]
  public class UnitTest1
  {
    private void Init()
    {
      Globals.PreInit();
      Globals.InitEngine();
      Globals.Engine.LinkHandle(new IntPtr(0));
      Globals.Engine.InitTrueVision();

      Globals.Engine.ActorTypeFactory.Initialise();
    }

    private ActorTypeInfo _MockActorType;
    private ActorTypeInfo MockActorType
    {
      get
      {
        if (_MockActorType == null)
        {
          _MockActorType = new ActorTypeInfo(Globals.Engine.ActorTypeFactory, "dummy");
          Globals.Engine.ActorTypeFactory.Register(_MockActorType);
        }
        return _MockActorType;
      }
    }

    private ActorInfo CreateMockActor()
    {
      return Globals.Engine.ActorFactory.Create(new ActorCreationInfo(MockActorType));
    }

    int RunCount = 100000;
    public const string TEST = "test";

    [TestMethod]
    public void TransformTest()
    {
      Log.Write(TEST, "TransformTest start");
      Init();

      ActorInfo d = CreateMockActor();

      TV_3DMATRIX m;
      TV_3DMATRIX m2;
      TV_3DVECTOR t1 = new TV_3DVECTOR(1, 2, 3);
      TV_3DVECTOR t2 = new TV_3DVECTOR(100, 99, 98);
      TV_3DVECTOR r1 = new TV_3DVECTOR(5, 10, 15);
      TV_3DVECTOR r2 = new TV_3DVECTOR(15, 30, 45);
      for (int i = 0; i < RunCount; i++)
      {
        Log.Write(TEST, "TransformTest {0}".F(i));
        d.Position = t1;
        d.Rotation = r1;

        m = d.GetMatrix();

        d.Position = t2;
        d.Rotation = r2;

        m2 = d.GetMatrix();
        Assert.AreEqual(m, m2);

        Globals.Engine.Game.GameTime += .5f;
        m2 = d.GetMatrix();
        Assert.AreNotEqual(m, m2);
        Globals.Engine.Game.GameTime += .5f;
      }

      Log.Write(TEST, "TransformTest end");
    }
  }
}
