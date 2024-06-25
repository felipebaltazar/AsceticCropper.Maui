using NGraphics;
using Colors = Microsoft.Maui.Graphics.Colors;
using ICanvas = NGraphics.ICanvas;
using Rect = NGraphics.Rect;

namespace Maui.AsceticCropper;

public abstract class MaskPainter : BindableObject, IPainter
{
    /// <summary>
    /// The Mask width property.
    /// </summary>
    public static BindableProperty MaskWidthProperty =
        BindableProperty.Create(nameof(MaskWidth), typeof(double), typeof(MaskPainter),
            1.0, BindingMode.OneWay);

    /// <summary>
    /// The Mask height property.
    /// </summary>
    public static BindableProperty MaskHeightProperty =
        BindableProperty.Create(nameof(MaskHeight), typeof(double), typeof(MaskPainter),
            1.0, BindingMode.OneWay);

    /// <summary>
    /// The Mask X property.
    /// </summary>
    public static BindableProperty MaskXProperty =
        BindableProperty.Create(nameof(MaskX), typeof(double), typeof(MaskPainter),
            0.0, BindingMode.OneWay);

    /// <summary>
    /// The Mask Y property.
    /// </summary>
    public static BindableProperty MaskYProperty =
        BindableProperty.Create(nameof(MaskY), typeof(double), typeof(MaskPainter),
            0.0, BindingMode.OneWay);

    protected Pen CreateBorderPen(CropperControl control)
    {
        if (control.BorderColor != Colors.Transparent && control.BorderWidth > 0)
        {
            var borderPen = new Pen(control.BorderColor.AsNColor(), FixPlatform(control.BorderWidth));

            if (control.IsDashed)
            {
                borderPen.DashPattern = new float[] { (float)FixPlatform(control.DashPatternStroke), (float)FixPlatform(control.DashPatternSpace) };
            }

            return borderPen;
        }


        return new Pen(Colors.Transparent.AsNColor());
    }

    public abstract void Paint(CropperControl control, ICanvas canvas, Rect rect);

    public void InvokeInvalidate()
    {
        MustInvalidateEvent?.Invoke(this, new EventArgs());
    }

    public event EventHandler MustInvalidateEvent;

    protected double FixPlatform(double value)
    {
        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            return Math.Round(value * DeviceDisplay.MainDisplayInfo.Density);
        }

        return value;
    }

    /// <summary>
    /// Mask width property
    /// </summary>
    /// <value>The width of the mask.</value>
    public double MaskWidth
    {
        get { return (double)GetValue(MaskWidthProperty); }
        set { SetValue(MaskWidthProperty, value); }
    }

    /// <summary>
    /// Mask height property
    /// </summary>
    /// <value>The height of the mask.</value>
    public double MaskHeight
    {
        get { return (double)GetValue(MaskHeightProperty); }
        set { SetValue(MaskHeightProperty, value); }
    }

    /// <summary>
    /// Mask X position
    /// </summary>
    /// <value>The x of the mask.</value>
    public double MaskX
    {
        get { return (double)GetValue(MaskXProperty); }
        set { SetValue(MaskXProperty, value); }
    }

    /// <summary>
    /// Mask Y position
    /// </summary>
    /// <value>The y of the mask.</value>
    public double MaskY
    {
        get { return (double)GetValue(MaskYProperty); }
        set { SetValue(MaskYProperty, value); }
    }
}
