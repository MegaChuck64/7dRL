using _7dRL.RoomCreation;
using Engine.Core;
using Engine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using System.Linq;
using Random = Engine.Utilities.Random;

namespace _7dRL;

public class Enemy : Character
{
    PathFinder pathFinder;
    Player player;
    private float movementTimer = 0f;


    private float attackTimer = 0f;
    public Enemy(Player player, Map map, SpriteAtlas atlas, WpfGame game, GameObject? owner = null)
        : base(map, atlas, game, owner)
    {        
        TilesPerSecond = 1.5f;
        AttackRange = 3;
        AttackRate = 1;
        MaxDamage = 5;
        Health = 25;
                
        AtlasPosition = Random.GetInt(2) == 0 ? (26, 6) : (29, 6);

        this.player = player;
        var startPos = Map.GetRandomEmptyTile((player.X, player.Y));
        if (startPos != null)
        {
            X = startPos.X;
            Y = startPos.Y;
        }
        pathFinder = new PathFinder(Map);
    }

    public override void Start()
    {
    }

    public override void Update(float dt)
    {
        HandleMovement(dt);

        if (player.IsActive)
            HandleAttacking(dt);

        base.Update(dt);
    }


    public void HandleAttacking(float dt)
    {
        attackTimer += dt;
        if (attackTimer > 1f / AttackRate)
        {
            var playerDist = Map.GetDistance(X, Y, player.X, player.Y);
            if (playerDist < AttackRange)
            {
                player.TakeDamage(Random.GetInt(MaxDamage));
                attackTimer = 0f;
            }
        }
    }


    public void HandleMovement(float dt)
    {
        if (Map.GetDistance(player.X, player.Y, X, Y) <= SightRange)
        {
            var playerAdjacent = Map.GetAdjacentTiles(player.X, player.Y, true).ToList();
            if (playerAdjacent.Count > 0)
            {
                var target = Random.GetInt(playerAdjacent.Count);
                pathFinder.CreatePath(X, Y, playerAdjacent[target].X, playerAdjacent[target].Y);
            }

            movementTimer += dt;
            if (movementTimer > 1f / TilesPerSecond)
            {
                if (pathFinder.TryStepForward(out (int x, int y)? nextPos))
                {
                    movementTimer = 0f;
                    if (nextPos.HasValue)
                    {
                        if (Map.GetTile(nextPos.Value.x, nextPos.Value.y) is MapTile nextTile
                        && !nextTile.HasCollider)
                        {
                            X = nextTile.X;
                            Y = nextTile.Y;
                        }
                    }
                }
            }

        }
        else movementTimer = 0f;
    }

    public override void Draw(SpriteBatch sb)
    {
        if (Map.GetTile(X, Y) is MapTile tile && (tile.IsInFOV))
        {
            base.Draw(sb);        
            //DrawTargetPath(sb);
        }
    }

    public void DrawTargetPath(SpriteBatch sb)
    {
        if (pathFinder.Path != null)
            for (int i = 0; i < pathFinder.Path.Count; i++)
            {
                Atlas.Draw(
                    new Point(pathFinder.Path[i].x, pathFinder.Path[i].y),
                    (2, 0),
                    new Color(33, 33, 33),
                    sb,
                    0.25f);
            }
    }
}