using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Color = Microsoft.Xna.Framework.Color;

namespace Engine.Utilities;

public static class Graphics
{
    public static GraphicsDevice GraphicsDevice { get; set; }

    public static Texture2D? LoadTexture(string name)
    {
        if (GraphicsDevice == null)
        {
            return default;
        }


        using var texture = File.OpenRead("../../../Content/" + name + ".png");
        return Texture2D.FromStream(GraphicsDevice, texture);
    }

    public static Image SpriteToImage(SpriteAtlas atlas, (int x, int y) atlasPosition)
    {
        var rect = atlas.SourceRectangle(atlasPosition);
        Color[] data = new Color[rect.Width * rect.Height];
        atlas.Texture.GetData(0, rect, data, 0, rect.Width * rect.Height);

        var bmp = new Bitmap(rect.Width, rect.Height);
        for (int x = 0; x < rect.Width; x++)
        {
            for (int y = 0; y < rect.Height; y++)
            {
                var col = data[rect.Width * x + y];
                bmp.SetPixel(x, y, System.Drawing.Color.FromArgb(col.A, col.R, col.G, col.B));
            }
        }
        return bmp;        
    }


}

[ValueConversion(typeof(System.Drawing.Image), typeof(System.Windows.Media.ImageSource))]
public sealed class ImageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return null;

        var image = (System.Drawing.Image)value;
        var bitmap = new System.Windows.Media.Imaging.BitmapImage();

        bitmap.BeginInit();
        MemoryStream memoryStream = new MemoryStream();
        image.Save(memoryStream, ImageFormat.Png);
        memoryStream.Seek(0, SeekOrigin.Begin);
        bitmap.StreamSource = memoryStream;
        bitmap.EndInit();

        return bitmap;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}
