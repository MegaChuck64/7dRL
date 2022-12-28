using _7dRL.RoomCreation;
using Engine.Core;
using Engine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.WpfInterop;
using System.Linq;

namespace _7dRL;

public class Player : Character
{

    private float movementTimer = 0f;
    private float attackTimer = 0f;

    private float glowSpeed = 3f;
    private float glowTimer = 0f;
    private int glowModifer = 0;
    private bool glowingUp = false;
    FireBallSpell spell;

    public Player(Map map, SpriteAtlas atlas, WpfGame game, GameObject? owner = null) : base(map, atlas, game, owner)
    {
        Name = "Player";
        AttackRange = 8;
        AttackRate = 1;
        MaxDamage = 20;

        TilesPerSecond = 4f;
        AtlasPosition = (24, 1);
        Tint = Color.MonoGameOrange;

        if (Map.GetRandomEmptyTile() is MapTile empty)
        {
            X = empty.X;
            Y = empty.Y;
        }

        spell = new FireBallSpell(Map, Atlas, this)
        {
            AttackDuration = 1f / AttackRate
        };

        Components.Add(spell);
    }
    public override void Start()
    {
    }

    public override void Update(float dt)
    {
        UpdateGlow(dt);
        UpdateFOV();
        HandleMovement(dt);
        HandleSpell(dt);
        SightRange = DayCycle.GetSightRangeBasedOnTime(MinSightRange, MaxSightRange);

        base.Update(dt);
    }

    public override void Die()
    {
        base.Die();
        (Game as MainGame).ParentWindow.GameOverGrid.Visibility = System.Windows.Visibility.Visible;
        DayCycle.MinutesPerRealSecond = 0f;
    }
    public void HandleSpell(float dt)
    {
        attackTimer += dt;
        if (attackTimer > 1f / AttackRate)
        {
            if (Input.KeyState.IsKeyDown(Keys.Space))
            {
                spell.IsAiming = true;
                var mousePixelPos = Input.GetMouseScreenPosition();
                var mousePos = Atlas.PixelToCellPosition(mousePixelPos.X, mousePixelPos.Y);
                if (Map.GetTile(mousePos.X, mousePos.Y) is MapTile tile)
                {
                    spell.Target(tile.X, tile.Y, AttackRange);
                    if (Input.WasPressed(Input.MouseButton.Left))
                    {
                        spell.Attack(Random.GetInt(MaxDamage));
                        attackTimer = 0f;
                        //var attackable = spell.AttackableCharacters();
                        //foreach (var enemy in attackable)
                        //{
                        //    enemy.TakeDamage(Random.GetInt(MaxDamage));
                        //}
                    }
                }

            }
            else spell.IsAiming = false;

        }
    }

    public void HandleMovement(float dt)
    {
        if ((Game as MainGame).TimeScale == 0f)
            return;


        var movement = new Vector2(Input.GetAxis(Input.Axis.Horizontal), Input.GetAxis(Input.Axis.Vertical));
        if (movement != Vector2.Zero)
        {
            movementTimer += dt;
            if (movementTimer > 1f/TilesPerSecond 
                //this so if you just started pressing down, you get instant feedback
                ||  Input.WasPressed(Keys.W) 
                ||  Input.WasPressed(Keys.A)
                ||  Input.WasPressed(Keys.S)
                ||  Input.WasPressed(Keys.D)
                ||  Input.WasPressed(Keys.Up)
                ||  Input.WasPressed(Keys.Right)
                ||  Input.WasPressed(Keys.Down)
                ||  Input.WasPressed(Keys.Left))
            {
      
                movementTimer = 0f;
                if (Map.GetTile((int)(X + movement.X), (int)(Y + movement.Y)) is MapTile nextTile 
                    && !nextTile.HasCollider && !Map.ObjectTiles[(nextTile.X, nextTile.Y)].HasCollider)
                {

                    X = nextTile.X;
                    Y = nextTile.Y;
                    (Game as MainGame).Cam.MoveCamera(-Atlas.CellToPixelPosition((int)movement.X, (int)movement.Y));
                }
            }
        }
        else movementTimer = 0f;

    }

    public void UpdateGlow(float dt)
    {
        glowTimer += dt;

        if (glowTimer > 1f / glowSpeed)
        {            
            glowTimer = 0f;
            if (glowingUp)
            {
                glowModifer++;
                if (glowModifer >= 1)
                {
                    glowingUp = false;
                }
            }
            else
            {
                glowModifer--;
                if (glowModifer <= -1)
                {
                    glowingUp = true;
                }
            }
        }
    }

    public void UpdateFOV()
    {
        int range = SightRange - glowModifer;
        Vector3 minExploredTint = new(0.01f, 0.01f, 0.01f);

        
        if (Map.GetTile(X,Y) is MapTile tile && !tile.HasCollider)
        { 

            Map.UpdateFOV(X, Y, range);
            foreach (MapTile t in Map.GetTilesOutOfFOV())
            {
                if (t.Type != TileType.Water)
                {
                    t.ColorMod = minExploredTint;
                    Map.ObjectTiles[(t.X, t.Y)].ColorMod = t.ColorMod;
                }
            }
            foreach (MapTile t in Map.GetTilesInFOV().Concat(Map.GetTilesOfType(TileType.Water)))
            {
                var dist = Map.GetDistance(t.X, t.Y, X, Y);
                var gradient = 1f / (range / (float)dist);
                var multiplier = 1f / (range / MathHelper.Lerp(range, .025f, gradient));

                t.ColorMod = new Vector3(multiplier, multiplier, multiplier);
                Map.ObjectTiles[(t.X, t.Y)].ColorMod = t.ColorMod;

            }
            foreach (MapTile t in Map.GetUnexploredTiles())
            {
                if (t.Type != TileType.Water)
                {
                    t.ColorMod = Vector3.Zero;
                    Map.ObjectTiles[(t.X, t.Y)].ColorMod = t.ColorMod;
                }
            }
        }
    }


}