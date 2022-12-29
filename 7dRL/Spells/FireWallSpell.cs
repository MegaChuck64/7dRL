using _7dRL.RoomCreation;
using Engine.Core;
using Engine.Utilities;
using Microsoft.Xna.Framework;
using System.Linq;


namespace _7dRL.Spells;

public class FireWallSpell : Spell
{
    readonly PathFinder pathfinder;
    public FireWallSpell(Map map, SpriteAtlas atlas, GameObject owner) : base(map, atlas, owner)
    {
        Tint = Color.Red * .5f;
        AtlasPosition = (28, 11);
        pathfinder = new PathFinder(map);
    }

    public override void Target(int x, int y, int? max = null)
    {
        pathfinder.CreatePath(Owner.X, Owner.Y, x, y, true, false);
        Path = max.HasValue ? pathfinder.Path.Take(max.Value).ToList() : pathfinder.Path;

    }
}