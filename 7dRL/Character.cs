using _7dRL.RoomCreation;
using Engine.Core;
using Engine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using System.Collections.Generic;

namespace _7dRL;

public interface ICharacter
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public float TilesPerSecond { get; set; }
    public int SightRange { get; set; }
    public int MaxSightRange { get; set; }
    public int MinSightRange { get; set; }
    public int AttackRange { get; set; }
    public float AttackRate { get; set; }
    public int MaxDamage { get; set; }
    public List<Affect> Affects { get; set; }    
}

public abstract class Affect
{
    public Character? Affector { get; set; }
    public Character? Affected { get; set; }

    public abstract void Apply();
    public Affect(Character? affector, Character? affected)
    {
        Affector = affector;
        Affected = affected;    
    }
}


public abstract class Character : GameObject, ICharacter
{
    public Map Map { get; set; }
    public SpriteAtlas Atlas { get; set; }
    public (int x, int y) AtlasPosition { get; set; }
    public Color Tint { get; set; } = Color.White;


    public string Name { get; set; } = "New Character";
    public int Health { get; set; } = 100;
    public int MaxHealth { get; set; } = 100;
    public float TilesPerSecond { get; set; } = 2f;
    //public int SightRange { get; set; } = 5;
    public int MaxSightRange { get; set; }
    public int MinSightRange { get; set; }
    public int SightRange { get; set; } = 5;
    
    public int AttackRange { get; set; } = 5;
    public float AttackRate { get; set; } = .5f;
    public int MaxDamage { get; set; } = 20;

    public List<Affect> Affects { get; set; }

    private float damageBlinkTimer = 0f;
    private float blinksPerSecond = 4f;
    private Color blinkColor = Color.Red;
    private bool isBlinking = false;
    private bool blinkOn = false;
    private int blinksLeft = 0;
    private float blinkDuration = 1f;
    private Color tempColor = Color.White;

    protected Character(Map map, SpriteAtlas atlas, WpfGame game, GameObject? owner = null) : base(game, owner)
    {
        Map = map;
        Atlas = atlas;
        Affects = new List<Affect>();
    }

    public override void Draw(SpriteBatch sb)
    {
        Atlas.Draw(new Point(X, Y), AtlasPosition, Tint, sb, 0.5f);
    }

    public virtual void TakeDamage(int dmg)
    {
        if (dmg <= 0f)
            return;
     
        Health -= dmg;
        
        if (Health <= 0)
        {
            Health = 0;
            EventLogger.AddEvent($"DEATH - {Name}");
            EventLogger.AddEvent($"\tDamage: \t{dmg}", true);
            Die();
        }
        else
        {
            EventLogger.AddEvent($"HIT - {Name}");
            EventLogger.AddEvent($"\tDamge: \t{dmg}");
            EventLogger.AddEvent($"\tHealth: \t{Health}");
        }
        
        if (!isBlinking)
        {
            isBlinking = true;
            blinksLeft = (int)(blinkDuration * blinksPerSecond);
            damageBlinkTimer = 0f;
            tempColor = Tint;
        }
    }

    public override void Update(float dt)
    {
        HandleDamageBlinking(dt);
    }

    public void HandleDamageBlinking(float dt)
    {
        if (isBlinking && blinksLeft > 0)
        {
            damageBlinkTimer += dt;
            if (damageBlinkTimer >= 1f / blinksPerSecond)
            {
                damageBlinkTimer = 0f;
                blinksLeft--;
                blinkOn = !blinkOn;
                Tint = blinkOn && blinksLeft > 0 ? blinkColor : tempColor;
            }
        }
        else isBlinking = false;
    }



    public virtual void Die()
    {
        IsActive = false;
    }

}