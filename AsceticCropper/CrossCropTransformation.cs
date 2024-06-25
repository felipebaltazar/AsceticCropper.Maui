using System;
using FFImageLoading.Transformations;
using Microsoft.Maui.Controls.Shapes;
using FFImageLoading.Work;


#if ANDROID
using Android.Graphics;
using Matrix = Android.Graphics.Matrix;
using Paint = Android.Graphics.Paint;
using RectF = Android.Graphics.RectF;
#else
using CoreGraphics;
using UIKit;
#endif

namespace Maui.AsceticCropper;

/// <summary>
/// Cross transformation
/// </summary>
public static class CrossCropTransformation
{
    static Lazy<ICropTransformation> implementation = new Lazy<ICropTransformation>(() => CreateTransformation(), LazyThreadSafetyMode.PublicationOnly);

    /// <summary>
    /// Gets if the plugin is supported on the current platform.
    /// </summary>
    public static bool IsSupported => implementation.Value == null ? false : true;

    /// <summary>
    /// Current plugin implementation to use
    /// </summary>
    public static ICropTransformation Current
    {
        get
        {
            ICropTransformation ret = implementation.Value;
            if (ret == null)
            {
                throw NotImplementedInReferenceAssembly();
            }
            return ret;
        }
    }

    static ICropTransformation CreateTransformation()
    {
        return new CropTransformation();
    }

    internal static Exception NotImplementedInReferenceAssembly() =>
        new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");

}

#if ANDROID
public class CropTransformation : TransformationBase, ICropTransformation
{
    public double XOffsetFactor { get; set; }
    public double YOffsetFactor { get; set; }
    public double WidthFactor { get; set; }
    public double HeightFactor { get; set; }

    public override string Key
    {
        get
        {
            return string.Format("CropTransformation,xOffset={0},yOffset={1},width={2},height={3}",
            XOffsetFactor, YOffsetFactor, WidthFactor, HeightFactor);
        }
    }

    public CropTransformation()
    { }

    protected override Bitmap Transform(Bitmap sourceBitmap, string path, FFImageLoading.Work.ImageSource source, bool isPlaceholder, string key)
    {
        return ToCropped(sourceBitmap, XOffsetFactor, YOffsetFactor, WidthFactor, HeightFactor);
    }

    public static Android.Graphics.Bitmap ToCropped(Android.Graphics.Bitmap source, double xOffset, double yOffset, double widthFactor, double heightFactor)
    {
        var config = source.GetConfig();
        if (config == null)
            config = Android.Graphics.Bitmap.Config.Argb8888;    // This will support transparency

        var bitmap = Android.Graphics.Bitmap.CreateBitmap((int)(source.Width * widthFactor), (int)(source.Height * heightFactor), config);

        using (var canvas = new Canvas(bitmap))
        using (Paint paint = new Paint())
        using (BitmapShader shader = new BitmapShader(source, Shader.TileMode.Clamp, Shader.TileMode.Clamp))
        using (Matrix matrix = new Matrix())
        {
            if (xOffset != 0 || yOffset != 0)
            {
                matrix.SetTranslate((float)(-xOffset * source.Width), (float)(-yOffset * source.Height));
                shader.SetLocalMatrix(matrix);
            }

            paint.SetShader(shader);
            paint.AntiAlias = false;

            var rectF = new RectF(0, 0, (float)(source.Width * widthFactor), (float)(source.Height * heightFactor));
            canvas.DrawRect(rectF, paint);

            return bitmap;
        }
    }
}
#else

public class CropTransformation : TransformationBase, ICropTransformation
{
    public double XOffsetFactor { get; set; }
    public double YOffsetFactor { get; set; }
    public double WidthFactor { get; set; }
    public double HeightFactor { get; set; }

    public CropTransformation()
    { }


    public override string Key
    {
        get
        {
            return string.Format("CropTransformation,XOffsetFactor={0},YOffsetFactor={1},width={2},height={3}",
            XOffsetFactor, YOffsetFactor, WidthFactor, HeightFactor);
        }
    }

    protected override UIImage Transform(UIImage sourceBitmap, string path, FFImageLoading.Work.ImageSource source, bool isPlaceholder, string key)
    {
        return ToCropped(sourceBitmap, XOffsetFactor, YOffsetFactor, WidthFactor, HeightFactor);
    }

    public static UIImage ToCropped(UIImage image, double XOffsetFactor, double YOffsetFactor, double WidthFactor, double HeightFactor)
    {
        if (image == null)
        {
            throw new NullReferenceException("Bitmap is null!");
        }

        var imgSize = image.Size;
        UIGraphics.BeginImageContextWithOptions(new CGSize(WidthFactor * imgSize.Width, HeightFactor * imgSize.Height), false, (nfloat)0.0);

        try
        {
            using (var context = UIGraphics.GetCurrentContext())
            {
                var clippedRect = new CGRect(0, 0, WidthFactor * imgSize.Width, HeightFactor * imgSize.Height);
                context.ClipToRect(clippedRect);
                var x = -XOffsetFactor * imgSize.Width;
                var y = -YOffsetFactor * imgSize.Height;
                var drawRect = new CGRect(x, y, imgSize.Width, imgSize.Height);
                image.Draw(drawRect);
                var modifiedImage = UIGraphics.GetImageFromCurrentImageContext();
                return modifiedImage;
            }
        }
        finally
        {
            UIGraphics.EndImageContext();
        }
    }
}
#endif
