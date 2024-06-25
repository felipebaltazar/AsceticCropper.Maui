using NGraphics;
using System;
using System.Collections.Generic;
using Color = Microsoft.Maui.Graphics.Color;
using Colors = Microsoft.Maui.Graphics.Colors;

namespace Maui.AsceticCropper;

public class CropperControl : NControlView
{
    /// <summary>
    /// The painter property.
    /// </summary>
    public static BindableProperty CustomPainterProperty =
        BindableProperty.Create(nameof(CustomPainter), typeof(IPainter), typeof(CropperControl), default,
            BindingMode.OneWay, null, (bindable, oldValue, newValue) =>
            {
                var control = bindable as CropperControl;

                if (oldValue is MaskPainter oldPainter)
                {
                    oldPainter.MustInvalidateEvent -= control.OnMustInvalidate;
                }

                if (newValue is MaskPainter painter)
                {
                    painter.MustInvalidateEvent += control.OnMustInvalidate;
                }

                control.Invalidate();
            });

    private void OnMustInvalidate(object sender, EventArgs e)
    {
        Invalidate();
    }

    /// <summary>
    /// The fill color property.
    /// </summary>
    public static new BindableProperty BackgroundColorProperty =
        BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(CropperControl),
            Colors.Transparent, BindingMode.OneWay, null, (bindable, oldValue, newValue) => (bindable as CropperControl).Invalidate());



    /// <summary>
    /// The border color property.
    /// </summary>
    public static BindableProperty BorderColorProperty =
        BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(CropperControl),
            Colors.Black, BindingMode.OneWay, null, (bindable, oldValue, newValue) => (bindable as CropperControl).Invalidate());

    /// <summary>
    /// The border width property.
    /// </summary>
    public static BindableProperty BorderWidthProperty =
        BindableProperty.Create(nameof(BorderWidth), typeof(double), typeof(CropperControl),
            0.0, BindingMode.OneWay, null, (bindable, oldValue, newValue) => (bindable as CropperControl).Invalidate());

    /// <summary>
    /// The border dash property.
    /// </summary>
    public static BindableProperty IsDashedProperty =
        BindableProperty.Create(nameof(IsDashed), typeof(bool), typeof(CropperControl),
            false, BindingMode.OneWay);

    /// <summary>
    /// The border stroke pattern property.
    /// </summary>
    public static BindableProperty DashPatternStrokeProperty =
        BindableProperty.Create(nameof(DashPatternStroke), typeof(float), typeof(CropperControl),
            4.0f, BindingMode.OneWay);

    /// <summary>
    /// The border stroke pattern spacing property.
    /// </summary>
    public static BindableProperty DashPatternSpaceProperty =
        BindableProperty.Create(nameof(DashPatternSpace), typeof(float), typeof(CropperControl),
            4.0f, BindingMode.OneWay);

    /// <summary>
    /// Gets or sets the mask painter
    /// </summary>
    /// <value>mask painter.</value>
    public IPainter CustomPainter
    {
        get { return (IPainter)GetValue(CustomPainterProperty); }
        set { SetValue(CustomPainterProperty, value); }
    }

    /// <summary>
    /// Gets or sets the color which will fill the background of a VisualElement. This is a bindable property.
    /// </summary>
    /// <value>The color of the background.</value>
    public new Color BackgroundColor
    {
        get { return (Color)GetValue(BackgroundColorProperty); }
        set { SetValue(BackgroundColorProperty, value); }
    }

    /// <summary>
    /// Gets or sets the color which will fill the border of a VisualElement. This is a bindable property.
    /// </summary>
    /// <value>The color of the border.</value>
    public Color BorderColor
    {
        get { return (Color)GetValue(BorderColorProperty); }
        set { SetValue(BorderColorProperty, value); }
    }

    /// <summary>
    /// Gets or sets the width which will have the border of a VisualElement. This is a bindable property.
    /// </summary>
    /// <value>The width of the border.</value>
    public double BorderWidth
    {
        get { return (double)GetValue(BorderWidthProperty); }
        set { SetValue(BorderWidthProperty, value); }
    }

    /// <summary>
    /// Gets or sets the dash which will have the border of a VisualElement. This is a bindable property.
    /// </summary>
    /// <value>The width of the border.</value>
    public bool IsDashed
    {
        get { return (bool)GetValue(IsDashedProperty); }
        set { SetValue(IsDashedProperty, value); }
    }

    /// <summary>
    /// The border stroke pattern property.
    /// </summary>
    public float DashPatternStroke
    {
        get { return (float)GetValue(DashPatternStrokeProperty); }
        set { SetValue(DashPatternStrokeProperty, value); }
    }

    /// <summary>
    /// The border stroke pattern spacing property.
    /// </summary>
    public float DashPatternSpace
    {
        get { return (float)GetValue(DashPatternSpaceProperty); }
        set { SetValue(DashPatternSpaceProperty, value); }
    }

    public override void Draw(NGraphics.ICanvas canvas, NGraphics.Rect rect)
    {
        if (BackgroundColor == Colors.Transparent && BorderColor == Microsoft.Maui.Graphics.Colors.Transparent)
        {
            return;
        }

        CustomPainter.Paint(this, canvas, rect);
    }
}
