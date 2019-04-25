using System.Text;
using System.IO;
using MTV3D65;
using SWEndor.Actors;
using SWEndor.UI;

namespace SWEndor
{
  public static class GameSaver
  {
    public static bool Save(string filename)
    {
      // UI
      UpdateUI();

      if (!Directory.Exists(Globals.SaveGamePath))
        Directory.CreateDirectory(Globals.SaveGamePath);

      try
      {
        StringBuilder sb = new StringBuilder();

        foreach (int actorID in Globals.Engine.ActorFactory.GetList())
        {
          ActorInfo a = Globals.Engine.ActorFactory.Get(actorID);
          if (a != null)
            new ActorParser(a).Generate(sb);
        }

        string filepath = Path.Combine(Globals.SaveGamePath, filename);
        if (File.Exists(filepath)) 
          File.Delete(filepath);

        File.AppendAllText(filepath, sb.ToString());
        return true;
      }
      catch
      {
        return false;
      }
    }

    private static void UpdateUI()
    {
      Globals.Engine.TrueVision.TVScreen2DImmediate.Action_Begin2D();
      Globals.Engine.TrueVision.TVScreen2DImmediate.Draw_FilledBox(Globals.Engine.ScreenWidth / 2 - 80
                                                          , Globals.Engine.ScreenWidth / 2 - 20
                                                          , Globals.Engine.ScreenWidth / 2 + 80
                                                          , Globals.Engine.ScreenWidth / 2 + 20
                                                          , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());

      Globals.Engine.TrueVision.TVScreen2DImmediate.Draw_FilledBox(Globals.Engine.ScreenWidth / 2 - 80
                                                          , Globals.Engine.ScreenWidth / 2 - 20
                                                          , Globals.Engine.ScreenWidth / 2 + 80
                                                          , Globals.Engine.ScreenWidth / 2 + 20
                                                          , new TV_COLOR(0.5f, 0.5f, 0.5f, 1f).GetIntColor());

      Globals.Engine.TrueVision.TVScreen2DImmediate.Draw_FilledBox(Globals.Engine.ScreenWidth / 2 - 75
                                                          , Globals.Engine.ScreenWidth / 2 - 15
                                                          , Globals.Engine.ScreenWidth / 2 + 75
                                                          , Globals.Engine.ScreenWidth / 2 + 15
                                                          , new TV_COLOR(0.5f, 0.5f, 0.5f, 1f).GetIntColor());
      Globals.Engine.TrueVision.TVScreen2DImmediate.Action_End2D();

      Globals.Engine.TrueVision.TVScreen2DText.Action_BeginText();
      Globals.Engine.TrueVision.TVScreen2DText.TextureFont_DrawText("Saving Game..."
                                                          , Globals.Engine.ScreenWidth / 2 - 60
                                                          , Globals.Engine.ScreenWidth / 2 - 10
                                                          , new TV_COLOR(0.5f, 0.5f, 0.5f, 1f).GetIntColor()
                                                          , Font.Factory.Get("Text_14").ID
      );
      Globals.Engine.TrueVision.TVScreen2DText.Action_EndText();

      //Thread.Sleep(1000);
    }
  }
}
