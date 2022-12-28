
using _7dRL.RoomCreation;
using Engine.Core;
using Engine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace _7dRL;

public class MainGame : BaseGame<MainWindow>
{
    //public MapCreator MapCreator { get; set; }
    //public TileMap Map { get; set; }
    public Player Player { get; set; }

    public List<Enemy> Enemies { get; set; } = new List<Enemy>();

    public List<(Color tint, string name)> EnemyColorNames;

    public event EventHandler PauseToggled;
    public event EventHandler UnPaused;

    public MainGame(MainWindow window) : base(window, "tileAtlas", 16, 2f)
    {
        EnemyColorNames = new List<(Color, string)>
        {
            (Color.Yellow, "Yellow Ghost"),
            (Color.Green, "Green Ghost"),
            (Color.Blue, "Blue Ghost"),
            (Color.HotPink, "Pink Ghost"),
        };

        DayCycle.MinutesPerRealSecond = 1f;

        PauseToggled += MainGame_PauseToggled;
    }

    #region Pausing
    private float lastTimeScale = 1f;

    private void MainGame_PauseToggled(object? sender, EventArgs e)
    {
        if (TimeScale == 0f)
        {
            TimeScale = lastTimeScale;
            ParentWindow.PauseMenu.Visibility = System.Windows.Visibility.Hidden;
        }
        else
        {
            lastTimeScale = TimeScale;
            TimeScale = 0f;
            ParentWindow.PauseMenu.Visibility = System.Windows.Visibility.Visible;
        }
    }

    public void TogglePause()
    {
        PauseToggled?.Invoke(this, EventArgs.Empty);
    }
    #endregion

    protected override void Start()
    {

        var map = new Map(100, 100, Atlas, this);
        GameObjects.Add(map);

        Player = new Player(map, Atlas, this)
        {            
            MinSightRange = 3,
            MaxSightRange = 24
        };
        GameObjects.Add(Player);

        Cam.MoveCamera(-Atlas.CellToPixelPosition(Player.X, Player.Y) + 
            new Vector2(GraphicsDevice.Viewport.Width/2 + (Atlas.TileSize * Atlas.Scale * 2),
            GraphicsDevice.Viewport.Height/2 + (Atlas.TileSize * Atlas.Scale * 2)));
        
        for (int n = 0; n < 3; n++)
            for (int i = 0; i < EnemyColorNames.Count; i++)
            {
                var enemy = new Enemy(Player, map, Atlas, this)
                {
                    Tint = EnemyColorNames[i].tint,
                    Name = EnemyColorNames[i].name
                };
                Enemies.Add(enemy);
                GameObjects.Add(enemy);
            }
    }

    protected override void Update(float dt)
    {
        DayCycle.Update(dt);

        if (Input.WasPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
        {
            TogglePause();
        }
    }


    protected override void Draw(SpriteBatch sb)
    {
        var txt = " -PLAYER- " +
            $"\rName:\t {Player.Name} " +
            $"\rHealth:\t {Player.Health}/{Player.MaxHealth} " +
            $"\rSpeed:\t {Player.TilesPerSecond} " +
            $"\rAtk.Rng:\t {Player.AttackRange} " +
            $"\rAtk.Spd:\t {Player.AttackRate} " +
            $"\rSight:\t {Player.SightRange}/{Player.MaxSightRange} " +
            
            $"\rTime:\t {DayCycle.Time:hh:mm tt}";
            

        ParentWindow.StatsTextBox.Text = txt;
        ParentWindow.FPSLabel.Content = "FPS: " + FPS;
    }


}