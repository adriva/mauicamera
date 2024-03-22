#if IOS || MACCATALYST
using CoreGraphics;
using UIKit;
using ZXing.Common;
using ZXing.Rendering;

namespace Adriva.Camera.Maui.ZXing.Platforms.Apple;

public class BitmapRenderer : IBarcodeRenderer<UIImage>
{
    public CGColor Foreground { get; set; }
    public CGColor Background { get; set; }

    public BitmapRenderer()
    {
        Foreground = new CGColor(0f, 0f, 0f);
        Background = new CGColor(1.0f, 1.0f, 1.0f);
    }
    public UIImage Render(BitMatrix matrix, global::ZXing.BarcodeFormat format, string content)
    {
        return Render(matrix, format, content, new EncodingOptions());
    }

    public UIImage Render(BitMatrix matrix, global::ZXing.BarcodeFormat format, string content, EncodingOptions options)
    {
        using var renderer = new UIGraphicsImageRenderer(new CGSize(matrix.Width, matrix.Height));
        var img = renderer.CreateImage(context =>
        {
            for (int x = 0; x < matrix.Width; x++)
            {
                for (int y = 0; y < matrix.Height; y++)
                {
                    context.CGContext.SetFillColor(matrix[x, y] ? Foreground : Background);
                    context.CGContext.FillRect(new CGRect(x, y, 1, 1));
                }
            }
        });

        return img;
    }
}
#endif
