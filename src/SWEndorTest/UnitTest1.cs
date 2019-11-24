using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTV3D65;
using SWEndor;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using Primrose.Primitives.Extensions;
using SWEndor.Core;

namespace SWEndorTest
{
  [TestClass]
  public class UnitTest1
  {
    private void Init()
    {
      Globals.PreInit();
      MockEngine = Globals.InitEngine();
      MockEngine.LinkHandle(new IntPtr(0));
      MockEngine.InitTV();

      //MockEngine.ActorTypeFactory.RegisterBase();
      //Globals.Engine.ActorTypeFactory.Initialise();
    }

    private Engine MockEngine;
    private ActorTypeInfo _MockActorType;
    private ActorTypeInfo MockActorType
    {
      get
      {
        if (_MockActorType == null)
        {
          _MockActorType = new ActorTypeInfo(actortype_f, "$NULL", "Null");
          actortype_f.Register(_MockActorType);
        }
        return _MockActorType;
      }
    }

    private ActorInfo CreateMockActor()
    {
      return actor_f.Create(new ActorCreationInfo(MockActorType));
    }

    int RunCount = 10000;
    public const string TEST = "test";
    Factory<ActorInfo, ActorCreationInfo, ActorTypeInfo> actor_f = new Factory<ActorInfo, ActorCreationInfo, ActorTypeInfo>(null, (e, f, n, i) => { return new ActorInfo(e, f, n, i); }, 10000);
    ActorTypeInfo.Factory actortype_f = new ActorTypeInfo.Factory(null);

    //[TestMethod]
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

        m = d.GetWorldMatrix();

        d.Position = t2;
        d.Rotation = r2;

        m2 = d.GetWorldMatrix();
        Assert.AreEqual(m, m2);

        MockEngine.Game.GameTime += .5f;
        m2 = d.GetWorldMatrix();
        Assert.AreNotEqual(m, m2);
        MockEngine.Game.GameTime += .5f;
      }

      Log.Write(TEST, "TransformTest end");
    }
  }
}
