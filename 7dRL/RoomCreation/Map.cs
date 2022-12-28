using Engine.Core;
using Engine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace _7dRL.RoomCreation;

public class Map : GameObject
{
    //tile layer
    //object layer
    public int Width { get; private set; }
    public int Height { get; private set; }
    public Dictionary<(int x, int y), MapTile> Tiles { get; set; } = new ();
    public Dictionary<(int x, int y), ObjectTile> ObjectTiles { get; set; } = new ();
    public SpriteAtlas Atlas { get; set; }

    public Map(int w, int h, SpriteAtlas atlas, MainGame game) : base(game)
    {
        Atlas = atlas;
        Width = w;
        Height = h;
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Tiles.Add((x,y), new MapTile
                {
                    X = x,
                    Y = y,
                    IsInFOV = true,
                    HasCollider = false,
                    HasBeenExplored = false,

                    BaseColor = Color.Green,
                    Type = TileType.Grass,
                    ColorMod = Vector3.One,
                    AtlasPosition = (5,0)
                });


                var rand = Random.GetInt(100);
                var objType = ObjectTileType.Empty;
                var objCol = Color.Transparent;
                var objAtlasPos = (0, 0);
                var objHasCollider = false; 
                if (rand <= 5)
                {
                    objHasCollider = true;
                    objType = (ObjectTileType)Random.GetInt(System.Enum.GetValues<ObjectTileType>().Length);
                    
                    if (objType == ObjectTileType.Tree)
                    {
                        objCol = Color.ForestGreen;
                        objAtlasPos = (4,2);
                    }
                    else if (objType == ObjectTileType.Rock)
                    {
                        objCol = Color.Gray;
                        objAtlasPos = (20, 5);
                    }
                }
                ObjectTiles.Add((x, y), new ObjectTile
                {
                    X = x,
                    Y = y,
                    HasCollider = objHasCollider,
                    BaseColor = objCol,
                    Type = objType,
                    ColorMod = Vector3.One,
                    AtlasPosition = objAtlasPos
                });
            }
        }


        var beach = Tiles.Values.Except(GetTilesInCircle(Width / 2, Height / 2, Width / 2 - 10));
        foreach (var tile in beach)
        {
            tile.IsInFOV = true;
            tile.HasCollider = false;
            tile.AtlasPosition = (2, 0);
            tile.BaseColor = Color.Yellow;
            tile.Type = TileType.Sand;
            ObjectTiles[(tile.X, tile.Y)].Type = ObjectTileType.Empty;
            ObjectTiles[(tile.X, tile.Y)].HasCollider = false;
        }

        var water = Tiles.Values.Except(GetTilesInCircle(Width / 2, Height / 2, Width / 2 - 7));
        foreach (var tile in water)
        {
            tile.IsInFOV = true;
            tile.HasCollider = true;
            tile.AtlasPosition = (7, 0);
            tile.BaseColor = Color.Blue;
            tile.Type = TileType.Water;
            ObjectTiles[(tile.X, tile.Y)].Type = ObjectTileType.Empty;
            ObjectTiles[(tile.X, tile.Y)].HasCollider = false;

        }




    }

    public override void Start()
    {
        //do map creation here

    }

    public override void Update(float dt)
    {
        
    }

    public override void Draw(SpriteBatch sb)
    {
        foreach (var tile in Tiles)
        {
            if (tile.Value.IsInFOV || tile.Value.Type == TileType.Water)
            {
                tile.Value.Draw(Atlas, sb);

                if (ObjectTiles[tile.Key].Type != ObjectTileType.Empty)
                {
                    ObjectTiles[tile.Key].Draw(Atlas, sb);
                }
            }
        }
    }


    public MapTile? GetTile(int x, int y) => Tiles[(x,y)] ?? null;
    public IEnumerable<MapTile> GetTiles(System.Func<MapTile, bool> predicate) =>
    Tiles
    .AsEnumerable()
    .Select(s => s.Value)
    .Where(predicate);


    #region Get By Property

    public IEnumerable<MapTile> GetTilesInFOV() => GetTiles(m => m.IsInFOV);

    public IEnumerable<MapTile> GetTilesOutOfFOV() => GetTiles(t => !t.IsInFOV);

    public IEnumerable<MapTile> GetExploredTiles() => GetTiles(t => t.HasBeenExplored);

    public IEnumerable<MapTile> GetUnexploredTiles() => GetTiles(t => !t.HasBeenExplored);
    public IEnumerable<MapTile> GetTilesOfType(TileType type) => GetTiles(t => t.Type == type);

    #endregion

    #region Get By Location

    public IEnumerable<MapTile> GetBorderTiles() =>
        GetTiles(t => t.X == 0 || t.X == Width - 1 || t.Y == 0 || t.Y == Height - 1);

    public IEnumerable<MapTile> GetTilesInRectangle(int x, int y, int width, int height) =>
        GetTiles(t => t.X >= x && t.X <= x + width && t.Y >= y && t.Y <= y + height);

    public IEnumerable<MapTile> GetTilesInCircle(int x, int y, int r) =>
        GetTiles(t => GetDistance(x, y, t.X, t.Y) <= r);

    public IEnumerable<MapTile> GetAdjacentTiles(int x, int y, bool includeDiagonals = false)
    {
        var adjacent = new List<MapTile>();

        if (GetTile(x + 1, y) is MapTile right)
            adjacent.Add(right);
        if (GetTile(x - 1, y) is MapTile left)
            adjacent.Add(left);
        if (GetTile(x, y - 1) is MapTile up)
            adjacent.Add(up);
        if (GetTile(x, y + 1) is MapTile down)
            adjacent.Add(down);

        if (includeDiagonals)
        {
            if (GetTile(x - 1, y - 1) is MapTile topLeft)
                adjacent.Add(topLeft);
            if (GetTile(x + 1, y - 1) is MapTile topRight)
                adjacent.Add(topRight);
            if (GetTile(x - 1, y + 1) is MapTile bottomLeft)
                adjacent.Add(bottomLeft);
            if (GetTile(x + 1, y + 1) is MapTile bottomRight)
                adjacent.Add(bottomRight);
        }


        return adjacent;
    }

    #endregion

    public static int GetDistance(int startX, int startY, int endX, int endY) =>
        (int)Vector2.Distance(new Vector2(startX, startY), new Vector2(endX, endY));



    /// <summary>
    /// Get empty tile if possible
    /// </summary>
    /// <param name="exclude">an optional tile to exclude from possible random tile</param>
    /// <returns>empty tile or null if can't find one</returns>
    public MapTile? GetRandomEmptyTile(params (int x, int y)?[] exclude)
    {
        while (true)
        {
            int x = Random.GetInt(Width - 1);
            int y = Random.GetInt(Height - 1);
            if (GetTile(x, y) is MapTile tile && !tile.HasCollider)
            {
                if (exclude == null || exclude.Length == 0 || !exclude.Contains((x, y)))
                    return tile;
            }
        }
    }

    public void UpdateFOV(int x, int y, int radius)
    {
        var tilesInFOV = new HashSet<MapTile>();
        ShadowCaster.ShadowCaster.ComputeFieldOfViewWithShadowCasting(
                x,
                y,
                radius,

                (tx, ty) => Tiles[(tx, ty)].HasCollider || ObjectTiles[(tx,ty)].HasCollider,        

                (tx, ty) =>
                {
                    if (GetTile(tx, ty) is MapTile tl)
                    {
                        tilesInFOV.Add(tl);
                        tl.HasBeenExplored = true;
                    }
                }
            );

        foreach (var tile in Tiles)
        {
            tile.Value.IsInFOV = tilesInFOV.Contains(tile.Value);
            //UpdateCell(tile.X, tile.Y, tile.State, tilesInFOV.Contains(tile), tile.HasBeenExplored);

        }
    }


}


public class MapTile
{
    public int X { get; set; }
    public int Y { get; set; }
    public bool IsInFOV { get; set; }
    public bool HasBeenExplored { get; set; }
    public bool HasCollider { get; set; }
    public Color BaseColor { get; set; }
    public Vector3 ColorMod { get; set; }
    public TileType Type { get; set; }
    public (int x, int y) AtlasPosition { get; set; }
    
    public Color ModifiedColor()
    {
        BaseColor.Deconstruct(out float r, out float g, out float b);
        ColorMod.Deconstruct(out float mR, out float mG, out float mB);

        return new Color(r * mR, g * mG, b * mB);
    }
        

    public void Draw(SpriteAtlas atlas, SpriteBatch sb) 
        => atlas.Draw(new Point(X, Y), AtlasPosition, ModifiedColor(), sb);   
}



public class ObjectTile
{
    public int X { get; set; }
    public int Y { get; set; }
    public bool HasCollider { get; set; }
    public Color BaseColor { get; set; }
    public Vector3 ColorMod { get; set; }
    public (int x, int y) AtlasPosition { get; set; }
    public ObjectTileType Type { get; set; }
    public Color ModifiedColor()
    {
        BaseColor.Deconstruct(out float r, out float g, out float b);
        ColorMod.Deconstruct(out float mR, out float mG, out float mB);

        return new Color(r * mR, g * mG, b * mB);
    }


    public void Draw(SpriteAtlas atlas, SpriteBatch sb)
        => atlas.Draw(new Point(X, Y), AtlasPosition, ModifiedColor(), sb, 0.1f);
}


public enum ObjectTileType
{
    Tree,
    Rock,
    Empty,
}

public enum TileType
{
    Water,
    Grass,
    Sand,
    //Tree, object layer stuff
}