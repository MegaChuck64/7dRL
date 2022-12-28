using _7dRL.RoomCreation;
using Engine.Core;
using Engine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace _7dRL;

public abstract class Spell : IComponent
{
    public GameObject Owner {get; set;}
    public bool IsActive { get; set; }

    public List<(int x, int y)> Path { get; set; }
    public SpriteAtlas Atlas { get; set; }
    public Map Map { get; set; }
    public Color Tint { get; set; } = Color.White;
    public float TintMultiplier { get; set; } = 2f;
    public (int x, int y) AtlasPosition { get; set; } = (0, 0);
    public float AttackDuration { get; set; } = 2f;
    public bool IsAttacking { get; private set; }
    public bool IsAiming { get; set; }
    public List<Character> AttackedCharacters { get; private set; } = new List<Character>();

    public List<(int x, int y)> AttackedPath { get; set; } = new List<(int x, int y)>();


    private float durationTimer = 0f;
    private int damage;

    public Spell(Map map, SpriteAtlas atlas, GameObject owner)
    {
        Map = map;
        Atlas = atlas;
        Owner = owner;
        IsActive = true;
        Path = new List<(int,int)>();
    }

    public abstract void Target(int x, int y, int? max);
        
    public virtual void Attack(int dmg)
    {
        damage = dmg;
        IsAttacking = true;
        
        durationTimer = 0f;
        var atkPth = new (int x, int y)[Path.Count];
        Path.CopyTo(atkPth);
        AttackedPath = atkPth.ToList();
    }

    private List<Character> AttackableCharacters(bool includePlayer = false, bool includeEnemies = true)
    {
        var characters = new List<Character>();
     
        if (IsAttacking && AttackedPath != null)
        {
            foreach (var (x, y) in AttackedPath)
            {

                if (x == Owner.X && y == Owner.Y) continue;

                if (includePlayer)
                {
                    var player = (Owner.Game as MainGame).Player;
                    if (player.X == x && player.Y == y)
                    {
                        characters.Add(player);
                    }
                }
                if (includeEnemies)
                {
                    var enemies = (Owner.Game as MainGame).Enemies;
                    foreach (var enemy in enemies)
                    {
                        if (enemy.X == x && enemy.Y == y)
                        {
                            characters.Add(enemy);
                        }
                    }
                }
                
            }
        }

        return characters;
    }
    public void Start()
    {

    }


    public void Update(float dt)
    {
        if(IsAttacking)
        {
            durationTimer += dt;


            var newAttacks = AttackableCharacters(false, true).Where(c => !AttackedCharacters.Contains(c));
            if (newAttacks.Any())
            {
                foreach (var atk in newAttacks)
                {
                    atk.TakeDamage(damage);
                }
                AttackedCharacters.AddRange(newAttacks);
            }



            if (durationTimer >= AttackDuration)
            {
                durationTimer = 0f;
                IsAttacking = false;
                
                AttackedCharacters.Clear();
                AttackedPath.Clear();
            }

        }
    }
    public void Draw(SpriteBatch sb)
    {
        if (IsAiming)
        {
            if (Path != null)
            {
                foreach (var (x, y) in Path.Skip(1))
                {
                    Atlas.Draw(new Point(x, y), AtlasPosition, Tint, sb, 0.25f);
                }
            }
        }

        if (AttackedPath != null)
        {
            foreach (var (x, y) in AttackedPath.Skip(1))
            {
                Atlas.Draw(new Point(x, y), AtlasPosition, Tint * TintMultiplier, sb, 0.25f);
            }
        }
    }
}