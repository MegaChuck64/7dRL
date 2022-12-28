
using Engine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Engine.Core;


public abstract class BaseGame<T> : WpfGame where T : Window
{
    private IGraphicsDeviceService graphicsDeviceManager;
    private SpriteBatch sb;
    private System.DateTime lastTime;
    private int framesRendered;
    private string atlasFile;


    public List<GameObject> GameObjects = new();
    public int FPS { get; protected set; }
    public T ParentWindow;
    public SpriteAtlas Atlas { get; set; }
    public int TileSize { get; set; }
    public float TileScale { get; set; }
    public float TimeScale { get; set; } = 1f;
    public Camera Cam { get; set; }

 
    public BaseGame(T window, string atlasfile, int tileSize, float tileScale)
    {
        ParentWindow = window;
        atlasFile = atlasfile;
        TileSize = tileSize;
        TileScale = tileScale;
        Cam = new Camera();
    }

    protected override void Initialize()
    {
        graphicsDeviceManager = new WpfGraphicsDeviceService(this);
        sb = new SpriteBatch(GraphicsDevice);


        base.Initialize();
        Input.Init(this, Cam);
        Graphics.GraphicsDevice = GraphicsDevice;

        Atlas = new SpriteAtlas(atlasFile, TileSize, TileScale);

        Start();
        foreach (var go in GameObjects)
        {
            go.OnStart();
        }


    }

    protected override void Update(GameTime time)
    {
        framesRendered++;

        if ((DateTime.Now - lastTime).TotalSeconds >= 1)
        {
            // one second has elapsed 

            FPS = framesRendered;
            framesRendered = 0;
            lastTime = System.DateTime.Now;
        }

        Input.Update();

        var dt = (float)time.ElapsedGameTime.TotalSeconds * TimeScale;
        foreach (var go in GameObjects)
        {
            go.OnUpdate(dt);
        }
        Update(dt);
        Cam.Update();
    }

    protected override void Draw(GameTime time)
    {
        GraphicsDevice.Clear(Color.Black);
        sb.Begin(
            sortMode: SpriteSortMode.FrontToBack,
            samplerState: SamplerState.PointClamp,
            transformMatrix: Cam.Transform
            );
        foreach (var go in GameObjects)
        {
            go.OnDraw(sb);
        }
        Draw(sb);
        sb.End();
    }

    protected abstract void Start();
    protected abstract void Update(float dt);
    protected abstract void Draw(SpriteBatch sb);
}

