﻿using SWEndor.Game.Core;
using System;
using System.Windows.Forms;

namespace SWEndor.Game.UI.Forms
{
  public partial class GameForm : Form
  {
    internal GameForm(Engine engine)
    {
      Engine = engine;
      InitializeComponent();
    }

    private readonly Engine Engine;
    private StatForm StatForm;

    private void GameForm_Load(object sender, EventArgs e)
    {
      Width = Engine.Settings.ResolutionX;
      Height = Engine.Settings.ResolutionY;
      CenterToScreen();

      // game
      Engine.Game.StartLoad();
      Engine.LinkForm(this);
      Engine.LinkHandle(pbGame.Handle);
      Engine.InitTV();
      Engine.Game.Run();
    }

    public void Exit()
    {
      Cursor.Show();
      Owner.Invoke(new Action(Owner.Close));
    }

    private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (Engine.Game.IsRunning)
      {
        Engine.Game.Stop();
        e.Cancel = true;
      }
    }

    private void pbGame_MouseLeave(object sender, EventArgs e)
    {
      Cursor.Show();
    }

    private void pbGame_MouseEnter(object sender, EventArgs e)
    {
      Cursor.Hide();
    }

    public void ShowStats()
    {
      if (StatForm == null || StatForm.IsDisposed)
        StatForm = new StatForm(Engine);
      StatForm.Show();
    }
  }
}
