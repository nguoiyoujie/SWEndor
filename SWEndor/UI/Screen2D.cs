using MTV3D65;
using SWEndor.UI;
using System.Collections.Generic;

namespace SWEndor
{
  public class Screen2D
  {
    private static Screen2D _instance;
    public static Screen2D Instance()
    {
      if (_instance == null) { _instance = new Screen2D(); }
      return _instance;
    }

    // Show toggles
    public bool ShowUI = true;
    public bool ShowStatus = true;
    public bool ShowRadar = true;
    public bool ShowScore = true;

    // fonts
    /*
    internal int FontID08 = -1;
    internal int FontID10 = -1;
    internal int FontID12 = -1;
    internal int FontID14 = -1;
    internal int FontID16 = -1;
    internal int FontID24 = -1;
    internal int FontID36 = -1;
    internal int FontID48 = -1;
    */

    // PreRender Text
    public List<string> LoadingTextLines = new List<string>(); 

    // Page
    public bool ShowPage = false;
    public UIPage CurrentPage = null;

    // Message Text
    public TextInfo PrimaryText = new TextInfo();
    public TextInfo SecondaryText = new TextInfo();

    // Radar
    public bool OverrideTargetingRadar = false;
    public string TargetingRadar_text = "";

    // Draw 3D Box info
    public bool Box3D_Enable = false;
    public TV_3DVECTOR Box3D_min = new TV_3DVECTOR();
    public TV_3DVECTOR Box3D_max = new TV_3DVECTOR();
    public TV_COLOR Box3D_color = new TV_COLOR(1, 1, 1, 1);

    ThreadSafeList<UIWidget> m_Widgets = new ThreadSafeList<UIWidget>();

    private Screen2D()
    {
      m_Widgets.Add(new UIWidget_SideBars());
      m_Widgets.Add(new UIWidget_HitBar());

      m_Widgets.Add(new UIWidget_Radar());

      //m_Widgets.Add(new UIWidget_Score()); // Disabled
      m_Widgets.Add(new UIWidget_AIInfo());
      m_Widgets.Add(new UIWidget_ScenarioInfo());
      m_Widgets.Add(new UIWidget_WeaponInfo());
      m_Widgets.Add(new UIWidget_Steering());
      m_Widgets.Add(new UIWidget_CrossHair());
      m_Widgets.Add(new UIWidget_Target());

      m_Widgets.Add(new UIWidget_3DBox());
      //m_Widgets.Add(new UIWidget_Debug_GeneralInfo());
      //m_Widgets.Add(new UIWidget_Debug_ObjectInfo());
      //m_Widgets.Add(new UIWidget_Debug_SelectInfo());

      m_Widgets.Add(new UIWidget_MessageText());

      m_Widgets.Add(new UIWidget_Page());
      m_Widgets.Add(new UIWidget_PerfText());
      m_Widgets.Add(new UIWidget_Terminal());

      m_Widgets.Add(new UIWidget_MouseLocation());
    }

    public void Draw()
    {
      foreach (UIWidget w in m_Widgets.GetList())
      {
        if (w != null && w.Visible)
          using (new PerfElement("render_" + w.Name))
            w.Draw();
      }
    }

    public void MessageText(string text, float expiretime, TV_COLOR color, int priority = 0)
    {
      if (PrimaryText.Priority <= priority || PrimaryText.ExpireTime < Game.Instance().GameTime)
      {
        PrimaryText.Priority = priority;
        PrimaryText.Text = text;
        PrimaryText.ExpireTime = Game.Instance().GameTime + expiretime;
        PrimaryText.Color = color;
      }
    }

    public void MessageSecondaryText(string text, float expiretime, TV_COLOR color, int priority = 0)
    {
      if (SecondaryText.Priority <= priority || SecondaryText.ExpireTime < Game.Instance().GameTime)
      {
        SecondaryText.Priority = priority;
        SecondaryText.Text = text;
        SecondaryText.ExpireTime = Game.Instance().GameTime + expiretime;
        SecondaryText.Color = color;
      }
    }

    public void ClearText()
    {
      PrimaryText.Text = "";
      SecondaryText.Text = "";
    }
  }
}
