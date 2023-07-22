﻿using Primrose.Primitives;
using Primrose.Primitives.Extensions;
using Primrose.Primitives.ValueTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor.Game
{
  public static class LookUpString
  {
    private static Cache<int, int, string, int> _displayPercent = new Cache<int, int, string, int>(128);
    private static Cache<int, int, string, int> _displayHP = new Cache<int, int, string, int>(128);
    private static Cache<int, int, string, int> _displayDistance = new Cache<int, int, string, int>(2048);
    private static Cache<int, int, string, int> _displayTime = new Cache<int, int, string, int>(2048);
    private static Cache<Pair<int, int>, int, string, Pair<int, int>> _displayRatio = new Cache<Pair<int, int>, int, string, Pair<int, int>>(1028);
    private static Cache<int, int, string, int> _displayStage = new Cache<int, int, string, int>(16);
    private static Cache<Quad<int, int, int, int>, int, string, Pair<Quad<int, int, int, int>, StringBuilder>> _displayMetric = new Cache<Quad<int, int, int, int>, int, string, Pair<Quad<int, int, int, int>, StringBuilder>>(2048);


    public static string GetIntegerPercent(float value)
    {
      // X%
      if (_displayPercent.Count > 100)
        _displayPercent.Clear();
      int ival = (int)(value + 0.5f);
      return _displayPercent.GetOrDefine(ival, 1, (i) => "{0:0}%".F(i), ival, EqualityComparer<int>.Default);
    }

    public static string GetHPPercent(double value)
    {
      // HP [X%]
      if (_displayHP.Count > 100)
        _displayHP.Clear();
      int ival = (int)(value + 0.5f);
      return _displayHP.GetOrDefine(ival, 1, (i) => "HP [{0}%]".F(i), ival, EqualityComparer<int>.Default);
    }

    public static string GetDistanceDisplay(float value)
    {
      // "DIST {0:000000}"
      if (_displayDistance.Count > 2040)
        _displayDistance.Clear();
      int ival = (int)(value + 0.5f);
      return _displayDistance.GetOrDefine(ival, 1, (i) => "DIST {0:000000}".F(i), ival, EqualityComparer<int>.Default);
    }

    public static string GetTimeDisplay(float timeInSeconds)
    {
      // mm:ss
      if (_displayTime.Count > 2040)
        _displayTime.Clear();
      int ival = (int)(timeInSeconds + 0.5f);
      return _displayTime.GetOrDefine(ival, 1, (i) => "{0:mm\\:ss}".F(TimeSpan.FromSeconds(i)), ival, EqualityComparer<int>.Default);
    }

    public static string GetRatioDisplay(int value, int max)
    {
      // 1 / 2
      if (_displayRatio.Count > 1020)
        _displayRatio.Clear();
      Pair<int, int> pair = new Pair<int, int>(value, max);
      return _displayRatio.GetOrDefine(pair, 1, (p) => "{0} / {1}".F(p.t, p.u), pair, EqualityComparer<int>.Default);
    }

    public static string GetStageDisplay(int stagenumber)
    {
      // STAGE: 1
      if (_displayStage.Count > 15)
        _displayStage.Clear();
      return _displayStage.GetOrDefine(stagenumber, 1, (i) => "STAGE: {0}".F(i), stagenumber, EqualityComparer<int>.Default);
    }

    private static StringBuilder _sbMetric = new StringBuilder(64);
    public static string GetMetricDisplay(int lives, int score, int kills, int hits)
    {
      // LIVES:        X
      // SCORE: 0000000Y
      // KILLS:        Z
      // HITS:         A
      if (_displayMetric.Count > 2040)
        _displayMetric.Clear();
      Quad<int, int, int, int> quad = new Quad<int, int, int, int>(lives, score, kills, hits);
      lock (_sbMetric)
      {
        Pair<Quad<int, int, int, int>, StringBuilder> pair = new Pair<Quad<int, int, int, int>, StringBuilder>(quad, _sbMetric);
        return _displayMetric.GetOrDefine(quad, 1, (p) =>
        {
          //"LIVES: {0,8:0}\nSCORE: {1,8:00000000}\nKILLS: {2,8:0}\nHITS:  {3,8:0}"
          p.u.Length = 0;
          p.u.Append("LIVES:        0\nSCORE: 00000000\nKILLS:        0\nHITS:         0");
          // Lives
          int lv = p.t.t;
          bool neg = lv < 0;
          lv = neg ? -lv : lv;
          lv = ((lv % 100000000) + 100000000) % 100000000;
          for (int i = 7; i >= 0; i--)
          {
            byte digit = (byte)(lv % 10);
            lv /= 10;
            if (lv == 0 && digit == 0)
              break;
            p.u[7 + i] = (char)('0' + digit);
          }
          p.u[6] = neg ? '-' : ' ';

          // Score
          lv = p.t.u;
          neg = lv < 0;
          lv = neg ? -lv : lv;
          lv = ((lv % 100000000) + 100000000) % 100000000;
          for (int i = 7; i >= 0; i--)
          {
            byte digit = (byte)(lv % 10);
            lv /= 10;
            if (lv == 0 && digit == 0)
              break;
            p.u[23 + i] = (char)('0' + digit);
          }
          p.u[22] = neg ? '-' : ' ';

          // Kills
          lv = ((p.t.v % 100000000) + 100000000) % 100000000;
          for (int i = 7; i >= 0; i--)
          {
            byte digit = (byte)(lv % 10);
            lv /= 10;
            if (lv == 0 && digit == 0)
              break;
            p.u[39 + i] = (char)('0' + digit);
          }

          // Hits
          lv = ((p.t.w % 100000000) + 100000000) % 100000000;
          for (int i = 7; i >= 0; i--)
          {
            byte digit = (byte)(lv % 10);
            lv /= 10;
            if (lv == 0 && digit == 0)
              break;
            p.u[55 + i] = (char)('0' + digit);
          }

          return _sbMetric.ToString();
        }, pair, EqualityComparer<int>.Default);
      }
    }
  }
}