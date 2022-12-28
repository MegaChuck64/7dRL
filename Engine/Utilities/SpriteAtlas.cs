using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Utilities;

public class SpriteAtlas
{
    public Texture2D Texture { get; set; }
    public int TileSize { get; set; }
    public float Scale { get; set; }

    public SpriteAtlas(string fileName, int tileSize, float scale = 1f)
    {
        Texture = Graphics.LoadTexture(fileName);
        TileSize = tileSize;
        Scale = scale;
    }

    public Rectangle DestinationRectanlge(Point location)
    {
        var pos = new Point((int)(location.X * TileSize * Scale), (int)(location.Y * TileSize * Scale));
        return new Rectangle(pos, new Point((int)(TileSize * Scale), (int)(TileSize * Scale)));
    }

    public Rectangle SourceRectangle((int x, int y) atlasLocation)
    {
        return new Rectangle(atlasLocation.x * TileSize, atlasLocation.y * TileSize, TileSize, TileSize);
    }

    public (int X, int Y) PixelToCellPosition(float x, float y) => (
     (int)(x / (TileSize * Scale)),
     (int)(y / (TileSize * Scale)));

    public Vector2 CellToPixelPosition(int x, int y) =>
        new(x * TileSize * Scale, y * TileSize * Scale);



    public void Draw(Point location, (int x, int y) atlasPosition, Color tint, SpriteBatch sb, float layer = 0.0f)
    {

        sb.Draw(
            Texture,
            DestinationRectanlge(new Point(location.X, location.Y)),
            SourceRectangle(atlasPosition),
            tint,
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            layer);
    }


}
