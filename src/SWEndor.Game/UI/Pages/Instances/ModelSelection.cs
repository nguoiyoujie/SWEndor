using MTV3D65;
using SWEndor.Game.ActorTypes;
using SWEndor.Game.ExplosionTypes;
using SWEndor.Game.ProjectileTypes;
using SWEndor.Game.Core;
using SWEndor.Game.ParticleTypes;

namespace SWEndor.Game.UI.Menu.Pages
{
  public class ModelSelection : Page
  {
    // Special selection menu for models
    // This 
    SelectionElement MainText = new SelectionElement();
    SelectionElement ButtonType = new SelectionElement();
    SelectionElement ButtonModel = new SelectionElement();
    SelectionElement ButtonNavigate = new SelectionElement();

    string[] Choices = new string[] { "Actor", "Explosion", "Projectile", "Particle" };
    int ChoiceIndex;
    int SelectedActorTypeIndex = 0;
    ActorTypeInfo[] ActorTypes = null;
    int SelectedExplosionTypeIndex = 0;
    ExplosionTypeInfo[] ExplosionTypes = null;
    int SelectedProjectileTypeIndex = 0;
    ProjectileTypeInfo[] ProjectileTypes = null;
    int SelectedParticleTypeIndex = 0;
    ParticleTypeInfo[] ParticleTypes = null;
    bool Navigating;

    public ModelSelection(Screen2D owner) : base(owner)
    {
      float height_gap = 40;
      float x = 75;
      float y = 120;

      MainText.Text = "Model Viewer";
      MainText.TextPosition = new TV_2DVECTOR(40, 60);
      MainText.SecondaryTextPosition = new TV_2DVECTOR(x + 250, 60);
      MainText.TextFont = FontFactory.Get(Font.T12).ID;
      MainText.HighlightBoxWidth = 650;
      MainText.HighlightBoxHeight = 30;

      ButtonType.Text = "Type";
      ButtonType.TextFont = FontFactory.Get(Font.T14).ID;
      ButtonType.TextPosition = new TV_2DVECTOR(x, y);
      ButtonType.SecondaryTextPosition = new TV_2DVECTOR(x + 250, y);
      y += height_gap;
      ButtonType.HighlightBoxPosition = ButtonType.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonType.HighlightBoxWidth = 650;
      ButtonType.HighlightBoxHeight = 30;
      ButtonType.Selectable = true;
      ButtonType.OnKeyPress += SelectType;
      ButtonType.SecondaryText = Choices[ChoiceIndex];

      ButtonModel.Text = "Model";
      ButtonModel.TextFont = FontFactory.Get(Font.T14).ID;
      ButtonModel.TextPosition = new TV_2DVECTOR(x, y);
      ButtonModel.SecondaryTextPosition = new TV_2DVECTOR(x + 250, y);
      y += height_gap;
      ButtonModel.HighlightBoxPosition = ButtonModel.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonModel.HighlightBoxWidth = 650;
      ButtonModel.HighlightBoxHeight = 30;
      ButtonModel.Selectable = true;
      ButtonModel.OnKeyPress += SelectModel;

      ButtonNavigate.Text = "Navigate";
      ButtonNavigate.TextFont = FontFactory.Get(Font.T14).ID;
      ButtonNavigate.TextPosition = new TV_2DVECTOR(x, y);
      ButtonNavigate.SecondaryTextPosition = new TV_2DVECTOR(x + 250, y);
      y += height_gap;
      ButtonNavigate.HighlightBoxPosition = ButtonNavigate.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonNavigate.HighlightBoxWidth = 250;
      ButtonNavigate.HighlightBoxHeight = 30;
      ButtonNavigate.Selectable = true;
      ButtonNavigate.OnKeyPress += SelectNavigate;

      // Extract types
      ActorTypes = Engine.ActorTypeFactory.GetValues();
      ExplosionTypes = Engine.ExplosionTypeFactory.GetValues();
      ProjectileTypes = Engine.ProjectileTypeFactory.GetValues();
      ParticleTypes = Engine.ParticleTypeFactory.GetValues();

      Elements.Add(MainText);
      Elements.Add(ButtonType);
      Elements.Add(ButtonModel);
      Elements.Add(ButtonNavigate);
      SelectedElementID = Elements.IndexOf(ButtonType);

      UpdateButtons();
    }

    private void UpdateButtons()
    {
      ButtonType.SecondaryText = Choices[ChoiceIndex];
      switch (ChoiceIndex)
      {
        case 1: // Explosions
          ButtonModel.SecondaryText = SelectedExplosionTypeIndex >= 0 && SelectedExplosionTypeIndex < ExplosionTypes.Length ? ExplosionTypes[SelectedExplosionTypeIndex].Name : string.Empty;
          break;

        case 2: // Projectiles
          ButtonModel.SecondaryText = SelectedProjectileTypeIndex >= 0 && SelectedProjectileTypeIndex < ProjectileTypes.Length ? ProjectileTypes[SelectedProjectileTypeIndex].Name : string.Empty;
          break;

        case 3: // Particles
          ButtonModel.SecondaryText = SelectedParticleTypeIndex >= 0 && SelectedParticleTypeIndex < ParticleTypes.Length ? ParticleTypes[SelectedParticleTypeIndex].Name : string.Empty;
          break;

        default: // Actors
          ButtonModel.SecondaryText = SelectedActorTypeIndex >= 0 && SelectedActorTypeIndex < ActorTypes.Length ? LookUpString.GetActorTypeWithDesignation(ActorTypes[SelectedActorTypeIndex]) : string.Empty;
          break;
      }
      MainText.SecondaryText = Navigating ? "Use the arrow, PageUp and PageDown keys to move around the area.\nEscape or Enter to leave navigation" : "Press Enter on 'Navigation' to activate free camera";
      ButtonNavigate.HighlightBoxColor = ColorLocalization.Get(Navigating ? ColorLocalKeys.UI_HIGHLIGHT_SELECT_BACKGROUND : ColorLocalKeys.UI_HIGHLIGHT_BACKGROUND);
    }

    private bool SelectNavigate(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        Navigating = true;
        UpdateButtons();
        return true;
      }
      return false;
    }

    private bool SelectType(CONST_TV_KEY key)
    {
      if (Choices.Length > 0)
      {
        if (key == CONST_TV_KEY.TV_KEY_LEFT)
        {
          ChoiceIndex = ((ChoiceIndex + 1) % Choices.Length + Choices.Length) % Choices.Length;
          UpdateButtons();
          return true;
        }
        else if (key == CONST_TV_KEY.TV_KEY_RIGHT)
        {
          ChoiceIndex = ((ChoiceIndex - 1) % Choices.Length + Choices.Length) % Choices.Length;
          UpdateButtons();
          return true;
        }
      }
      return false;
    }

    private bool SelectModel(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_LEFT)
      {
        switch (ChoiceIndex)
        {
          case 1: // Explosions
            SelectedExplosionTypeIndex = ExplosionTypes.Length == 0 ? 0 : ((SelectedExplosionTypeIndex + 1) % ExplosionTypes.Length + ExplosionTypes.Length) % ExplosionTypes.Length;
            break;

          case 2: // Projectiles
            SelectedProjectileTypeIndex = ProjectileTypes.Length == 0 ? 0 : ((SelectedProjectileTypeIndex + 1) % ProjectileTypes.Length + ProjectileTypes.Length) % ProjectileTypes.Length;
            break;

          case 3: // Particles
            SelectedParticleTypeIndex = ParticleTypes.Length == 0 ? 0 : ((SelectedParticleTypeIndex + 1) % ParticleTypes.Length + ParticleTypes.Length) % ParticleTypes.Length;
            break;

          default: // Actors
            SelectedActorTypeIndex = ActorTypes.Length == 0 ? 0 : ((SelectedActorTypeIndex + 1) % ActorTypes.Length + ActorTypes.Length) % ActorTypes.Length;
            break;
        }
        UpdateButtons();
        return true;
      }
      else if (key == CONST_TV_KEY.TV_KEY_RIGHT)
      {
        switch (ChoiceIndex)
        {
          case 1: // Explosions
            SelectedExplosionTypeIndex = ExplosionTypes.Length == 0 ? 0 : ((SelectedExplosionTypeIndex - 1) % ExplosionTypes.Length + ExplosionTypes.Length) % ExplosionTypes.Length;
            break;

          case 2: // Projectiles
            SelectedProjectileTypeIndex = ProjectileTypes.Length == 0 ? 0 : ((SelectedProjectileTypeIndex - 1) % ProjectileTypes.Length + ProjectileTypes.Length) % ProjectileTypes.Length;
            break;

          case 3: // Particles
            SelectedParticleTypeIndex = ParticleTypes.Length == 0 ? 0 : ((SelectedParticleTypeIndex - 1) % ParticleTypes.Length + ParticleTypes.Length) % ParticleTypes.Length;
            break;

          default: // Actors
            SelectedActorTypeIndex = ActorTypes.Length == 0 ? 0 : ((SelectedActorTypeIndex - 1) % ActorTypes.Length + ActorTypes.Length) % ActorTypes.Length;
            break;
        }
        UpdateButtons();
        return true;
      }
      return false;
    }

    public object GetModelType()
    {
      switch (ChoiceIndex)
      {
        case 1: // Explosions
          return ExplosionTypes[SelectedExplosionTypeIndex];

        case 2: // Projectiles
          return ProjectileTypes[SelectedProjectileTypeIndex];

        default: // Actors
          return ActorTypes[SelectedActorTypeIndex];
      }
    }

    public override void WhileKeyPressed(CONST_TV_KEY key)
    {
      if (Navigating)
      {
        TVCamera tvc = Engine.PlayerCameraInfo.Camera;
        float rate = Engine.InputManager.SHIFT ? 2500 : 500;
        rate *= Game.TimeControl.UpdateInterval;
        switch (key)
        {
          case CONST_TV_KEY.TV_KEY_LEFT:
            tvc.MoveRelative(0, 0, -rate);
            break;

          case CONST_TV_KEY.TV_KEY_RIGHT:
            tvc.MoveRelative(0, 0, rate);
            break;

          case CONST_TV_KEY.TV_KEY_UP:
            tvc.MoveRelative(rate, 0, 0);
            break;

          case CONST_TV_KEY.TV_KEY_DOWN:
            tvc.MoveRelative(-rate, 0, 0);
            break;

          case CONST_TV_KEY.TV_KEY_PAGEUP:
            tvc.MoveRelative(0, rate, 0);
            break;

          case CONST_TV_KEY.TV_KEY_PAGEDOWN:
            tvc.MoveRelative(0, -rate, 0);
            break;
        }
      }
      base.WhileKeyPressed(key);
    }


    public override bool OnKeyPress(CONST_TV_KEY key)
    {
      if (Navigating)
      {
        float rate = Engine.InputManager.SHIFT ? 2500 : 500;
        switch (key)
        {
          case CONST_TV_KEY.TV_KEY_ESCAPE:
          case CONST_TV_KEY.TV_KEY_RETURN:
            Navigating = false;
            UpdateButtons();
            return true;
        }
        return false;
      }

      switch (key)
      {
        case CONST_TV_KEY.TV_KEY_ESCAPE:
          Owner.CurrentPage = new PauseMenu(Owner);
          Owner.ShowPage = true;
          Game.IsPaused = true;
          return true;
      }
      return base.OnKeyPress(key);
    }

    public override void HandleMouse(int mouseX, int mouseY, bool button1, bool button2, bool button3, bool button4, int mouseScroll)
    {
      base.HandleMouse(mouseX, mouseY, button1, button2, button3, button4, mouseScroll);
      if (Navigating)
      {
        Engine.PlayerCameraInfo.RotateCam(mouseX / (float)Engine.ScreenWidth * 2 - 1, mouseY / (float)Engine.ScreenHeight * 2 - 1, mouseScroll);
      }
    }
  }
}
