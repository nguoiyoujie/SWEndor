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

        foreach (ActorInfo ainfo in ActorInfo.Factory.GetActorList())
        {
          new ActorParser(ainfo).Generate(sb);
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
      Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(Engine.Instance().ScreenWidth / 2 - 80
                                                          , Engine.Instance().ScreenWidth / 2 - 20
                                                          , Engine.Instance().ScreenWidth / 2 + 80
                                                          , Engine.Instance().ScreenWidth / 2 + 20
                                                          , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());

      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(Engine.Instance().ScreenWidth / 2 - 80
                                                          , Engine.Instance().ScreenWidth / 2 - 20
                                                          , Engine.Instance().ScreenWidth / 2 + 80
                                                          , Engine.Instance().ScreenWidth / 2 + 20
                                                          , new TV_COLOR(0.5f, 0.5f, 0.5f, 1f).GetIntColor());

      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(Engine.Instance().ScreenWidth / 2 - 75
                                                          , Engine.Instance().ScreenWidth / 2 - 15
                                                          , Engine.Instance().ScreenWidth / 2 + 75
                                                          , Engine.Instance().ScreenWidth / 2 + 15
                                                          , new TV_COLOR(0.5f, 0.5f, 0.5f, 1f).GetIntColor());
      Engine.Instance().TVScreen2DImmediate.Action_End2D();

      Engine.Instance().TVScreen2DText.Action_BeginText();
      Engine.Instance().TVScreen2DText.TextureFont_DrawText("Saving Game..."
                                                          , Engine.Instance().ScreenWidth / 2 - 60
                                                          , Engine.Instance().ScreenWidth / 2 - 10
                                                          , new TV_COLOR(0.5f, 0.5f, 0.5f, 1f).GetIntColor()
                                                          , Font.Factory.Get("Text_14").ID
      );
      Engine.Instance().TVScreen2DText.Action_EndText();

      //Thread.Sleep(1000);
    }
  }
}
