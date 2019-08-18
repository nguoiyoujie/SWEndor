using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SWEndor.Actors.Traits;
using MTV3D65;
using SWEndor;
using SWEndor.Primitives;

namespace SWEndorTest
{
  [TestClass]
  public class UnitTest1
  {
    private void Init()
    {
      Globals.PreInit();
      Engine engine = Globals.InitEngine();
      engine.LinkHandle(new IntPtr(0));
      engine.InitTrueVision();
    }

    int RunCount = 100000;
    public const string TEST = "test";

    [TestMethod]
    public void TransformTest()
    {
      Log.Write(TEST, "TransformTest start");
      Init();
      DummyTestActor d = new DummyTestActor();
      Transform trm = d.AddTrait(new Transform());

      TV_3DMATRIX m;
      TV_3DMATRIX m2;
      TV_3DVECTOR t1 = new TV_3DVECTOR(1, 2, 3);
      TV_3DVECTOR t2 = new TV_3DVECTOR(100, 99, 98);
      TV_3DVECTOR r1 = new TV_3DVECTOR(5, 10, 15);
      TV_3DVECTOR r2 = new TV_3DVECTOR(15, 30, 45);
      for (int i = 0; i < RunCount; i++)
      {
        Log.Write(TEST, "TransformTest {0}".F(i));
        trm.Position = t1;
        trm.Rotation = r1;

        m = trm.GetWorldMatrix(d, i);

        trm.Position = t2;
        trm.Rotation = r2;

        m2 = trm.GetWorldMatrix(d, i);
        Assert.AreEqual(m, m2);

        m2 = trm.GetWorldMatrix(d, i + .5f);
        Assert.AreNotEqual(m, m2);
      }

      Log.Write(TEST, "TransformTest end");
    }
  }
}
