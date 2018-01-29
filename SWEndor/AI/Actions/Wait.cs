using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor.Actions
{
  public class Wait : ActionInfo
  {
    public Wait(float time = 5) : base("Wait")
    {
      WaitTime = time;
    }

    private float WaitTime = 0;
    private float ResumeTime = 0;

    public override string ToString()
    {
      return string.Format("{0},{1},{2}"
                          , Name
                          , ResumeTime - Game.Instance().GameTime
                          , Complete
                          );
    }

    public override void Process(ActorInfo owner)
    {
      if (ResumeTime == 0)
      {
        ResumeTime = Game.Instance().GameTime + WaitTime;
      }
      Complete |= (ResumeTime < Game.Instance().GameTime);
    }
  }
}
