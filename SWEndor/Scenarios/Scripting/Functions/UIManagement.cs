﻿using MTV3D65;
using SWEndor.Scenarios.Scripting.Expressions;
using System;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class UIManagement
  {
    public static object SetUILine1Color(Context context, object[] ps)
    {
      TV_COLOR color = new TV_COLOR(Convert.ToSingle(ps[0].ToString()), Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()), 1);
      GameScenarioManager.Instance().Line1Color = color;
      return true;
    }

    public static object SetUILine2Color(Context context, object[] ps)
    {
      TV_COLOR color = new TV_COLOR(Convert.ToSingle(ps[0].ToString()), Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()), 1);
      GameScenarioManager.Instance().Line2Color = color;
      return true;
    }

    public static object SetUILine3Color(Context context, object[] ps)
    {
      TV_COLOR color = new TV_COLOR(Convert.ToSingle(ps[0].ToString()), Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()), 1);
      GameScenarioManager.Instance().Line3Color = color;
      return true;
    }

    public static object SetUILine1Text(Context context, object[] ps)
    {
      GameScenarioManager.Instance().Line1Text = ps[0].ToString();
      return true;
    }

    public static object SetUILine2Text(Context context, object[] ps)
    {
      GameScenarioManager.Instance().Line2Text = ps[0].ToString();
      return true;
    }

    public static object SetUILine3Text(Context context, object[] ps)
    {
      GameScenarioManager.Instance().Line3Text = ps[0].ToString();
      return true;
    }
  }
}