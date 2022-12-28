using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using System.Collections.Generic;


namespace Engine.Core;

public abstract class GameObject : IComponent
{
    private bool isActive = true;
    public bool IsActive
    {
        get => isActive;
        set
        {
            isActive = value;
            if (isActive)
                OnStart();
        }
    }

    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;

    public float Scale { get; set; } = 1f;

    public WpfGame Game { get; private set; }
    public GameObject? Owner { get; set; }

    public List<IComponent> Components = new();

    public GameObject(WpfGame game, GameObject? owner = null)
    {
        Game = game;
        Owner = owner;
    }

    public void OnStart()
    {
        if (!isActive) return;
        foreach (var comp in Components)
        {
            if (comp.IsActive)
                comp.Start();
        }
        Start();
    }
    public void OnUpdate(float dt)
    {
        if (!isActive) return;
        foreach (var comp in Components)
        {
            if (comp.IsActive)
                comp.Update(dt);
        }
        Update(dt);
    }
    public void OnDraw(SpriteBatch sb)
    {
        if (!isActive) return;

        foreach (var comp in Components)
        {
            if (comp.IsActive)
                comp.Draw(sb);
        }
        Draw(sb);
    }

    public abstract void Start();
    public abstract void Update(float dt);
    public abstract void Draw(SpriteBatch sb);

}