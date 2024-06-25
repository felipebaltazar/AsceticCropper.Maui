using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Platform;
using NGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NGraphics;
using Microsoft.Maui.Handlers;
using Rect = NGraphics.Rect;
using Colors = Microsoft.Maui.Graphics.Colors;
using ICanvas = NGraphics.ICanvas;
using Size = NGraphics.Size;

using Color = NGraphics.Color;
using Font = NGraphics.Font;
using IImage = NGraphics.IImage;
using Point = NGraphics.Point;
using Brush = NGraphics.Brush;
using RadialGradientBrush = NGraphics.RadialGradientBrush;
using LinearGradientBrush = NGraphics.LinearGradientBrush;
using GradientStop = NGraphics.GradientStop;
using TextAlignment = NGraphics.TextAlignment;


#if ANDROID
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Hardware.Lights;
using Android.Content;
#elif IOS
using CoreText;
using Foundation;

using ImageIO;
using UIKit;
using CoreGraphics;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
#endif


namespace Maui.AsceticCropper;

public class NControlView : ContentView
{
    #region Events

    /// <summary>
    /// Occurs when on invalidate.
    /// </summary>
    public event System.EventHandler OnInvalidate;

    /// <summary>
    /// Touches began
    /// </summary>
    public event System.EventHandler<IEnumerable<NGraphics.Point>> OnTouchesBegan;

    /// <summary>
    /// Touches moved
    /// </summary>
    public event System.EventHandler<IEnumerable<NGraphics.Point>> OnTouchesMoved;

    /// <summary>
    /// Touches ended
    /// </summary>
    public event System.EventHandler<IEnumerable<NGraphics.Point>> OnTouchesEnded;

    /// <summary>
    /// Touches cancelled
    /// </summary>
    public event System.EventHandler<IEnumerable<NGraphics.Point>> OnTouchesCancelled;

    #endregion

    #region Delegates and Callbacks

    /// <summary>
    /// Get platform delegate
    /// </summary>
    public delegate IPlatform GetPlatformDelegate();

    /// <summary>
    /// Occurs when on create canvas.
    /// </summary>
    public event GetPlatformDelegate OnGetPlatform;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="NControl.Forms.Xamarin.Plugins.iOS.NControlNativeView"/> class.
    /// </summary>
    public NControlView()
    {
        BackgroundColor = Colors.Transparent;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NControl.Forms.Xamarin.Plugins.iOS.NControlNativeView"/> class with
    /// a callback action for drawing
    /// </summary>
    /// <param name="drawingFunc">Drawing func.</param>
    public NControlView(Action<NGraphics.ICanvas, Rect> drawingFunc) : this()
    {
        DrawingFunction = drawingFunc;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the drawing function.
    /// </summary>
    /// <value>The drawing function.</value>	    
    public Action<ICanvas, Rect> DrawingFunction { get; set; }

    /// <summary>
    /// Gets the platform.
    /// </summary>
    /// <value>The platform.</value>
    public IPlatform Platform
    {
        get
        {
            if (OnGetPlatform == null)
                throw new ArgumentNullException("OnGetPlatform");

            return OnGetPlatform();
        }
    }

    #endregion

    #region Drawing

    /// <summary>
    /// Invalidate this instance.
    /// </summary>
    public void Invalidate()
    {
        if (OnInvalidate != null)
            OnInvalidate(this, EventArgs.Empty);
    }

    /// <summary>
    /// Draw the specified canvas.
    /// </summary>
    /// <param name="canvas">Canvas.</param>
    public virtual void Draw(ICanvas canvas, Rect rect)
    {
        if (DrawingFunction != null)
            DrawingFunction(canvas, rect);
    }

    #endregion

    #region Touches

    /// <summary>
    /// Touchs down.
    /// </summary>
    /// <param name="point">Point.</param>
    public virtual bool TouchesBegan(IEnumerable<NGraphics.Point> points)
    {
        if (OnTouchesBegan != null)
        {
            OnTouchesBegan(this, points);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Toucheses the moved.
    /// </summary>
    /// <param name="point">Point.</param>
    public virtual bool TouchesMoved(IEnumerable<NGraphics.Point> points)
    {
        if (OnTouchesMoved != null)
        {
            OnTouchesMoved(this, points);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Toucheses the cancelled.
    /// </summary>
    public virtual bool TouchesCancelled(IEnumerable<NGraphics.Point> points)
    {
        if (OnTouchesCancelled != null)
        {
            OnTouchesCancelled(this, points);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Toucheses the ended.
    /// </summary>
    public virtual bool TouchesEnded(IEnumerable<NGraphics.Point> points)
    {
        if (OnTouchesEnded != null)
        {
            OnTouchesEnded(this, points);
            return true;
        }

        return false;
    }

    #endregion
}


#if ANDROID
/// <summary>
/// NControlView renderer.
/// </summary>
public class NControlViewRenderer : VisualElementRenderer<NControlView>
{
    public NControlViewRenderer(Context context) : base(context)
    {
    }

    /// <summary>
    /// Used for registration with dependency service
    /// </summary>
    public static void Init()
    {
        var temp = DateTime.Now;
    }

    /// <summary>
    /// Raises the element changed event.
    /// </summary>
    /// <param name="e">E.</param>
    protected override void OnElementChanged(ElementChangedEventArgs<NControlView> e)
    {
        base.OnElementChanged(e);

        if (e.OldElement != null)
        {
            e.OldElement.OnInvalidate -= HandleInvalidate;
            e.OldElement.OnGetPlatform -= OnGetPlatformHandler;
        }

        if (e.NewElement != null)
        {
            e.NewElement.OnInvalidate += HandleInvalidate;
            e.NewElement.OnGetPlatform += OnGetPlatformHandler;
        }

        // Lets have a clear background
        this.SetBackgroundColor(Android.Graphics.Color.Transparent);

        Invalidate();
    }

    /// <summary>
    /// Override to avoid setting the background to a given color
    /// </summary>
    protected override void UpdateBackgroundColor()
    {
        // Do NOT call update background here.
        // base.UpdateBackgroundColor();
    }

    /// <summary>
    /// Raises the element property changed event.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">E.</param>
    protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        base.OnElementPropertyChanged(sender, e);

        if (e.PropertyName == NControlView.BackgroundColorProperty.PropertyName)
            Element.Invalidate();
        else if (e.PropertyName == NControlView.IsVisibleProperty.PropertyName)
            Element.Invalidate();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && Element != null)
        {
            Element.OnInvalidate -= HandleInvalidate;
            Element.OnGetPlatform -= OnGetPlatformHandler;
        }
        base.Dispose(disposing);
    }

    #region Native Drawing 

    /// <Docs>The Canvas to which the View is rendered.</Docs>
    /// <summary>
    /// Draw the specified canvas.
    /// </summary>
    /// <param name="canvas">Canvas.</param>
    public override void Draw(Android.Graphics.Canvas canvas)
    {
        if (Element == null)
        {
            base.Draw(canvas);
            return;
        }

        // Draws the background and default android setup. Children will also be redrawn here
        // base.Draw(canvas);

        // Set up clipping
        if (Element.IsClippedToBounds)
            canvas.ClipRect(new Android.Graphics.Rect(0, 0, Width, Height));

        // Perform custom drawing from the NGraphics subsystems
        var ncanvas = new CanvasCanvas(canvas);

        var rect = new NGraphics.Rect(0, 0, Width, Height);

        // Fill background 
        ncanvas.FillRectangle(rect, new NGraphics.Color(Element.BackgroundColor.Red, Element.BackgroundColor.Green, Element.BackgroundColor.Blue, Element.BackgroundColor.Alpha));

        // Custom drawing
        Element.Draw(ncanvas, rect);

        // Redraw children - since we might have a composite control containing both children 
        // and custom drawing code, we want children to be drawn last. The reason for this double-
        // drawing is that the base.Draw(canvas) call will handle background which is needed before
        // doing NGraphics drawing - but unfortunately it also draws children - which then will 
        // be drawn below NGraphics drawings.
        base.Draw(canvas);
    }

    #endregion

    #region Touch Handling

    /// <Docs>The motion event.</Docs>
    /// <returns>To be added.</returns>
    /// <para tool="javadoc-to-mdoc">Implement this method to handle touch screen motion events.</para>
    /// <format type="text/html">[Android Documentation]</format>
    /// <since version="Added in API level 1"></since>
    /// <summary>
    /// Raises the touch event event.
    /// </summary>
    /// <param name="e">E.</param>
    public override bool OnTouchEvent(MotionEvent e)
    {
        var scale = Element.Width / Width;

        var touchInfo = new List<NGraphics.Point>();
        for (var i = 0; i < e.PointerCount; i++)
        {
            var coord = new MotionEvent.PointerCoords();
            e.GetPointerCoords(i, coord);
            touchInfo.Add(new NGraphics.Point(coord.X * scale, coord.Y * scale));
        }

        var result = false;

        // Handle touch actions
        switch (e.Action)
        {
            case MotionEventActions.Down:
                if (Element != null)
                    result = Element.TouchesBegan(touchInfo);
                break;

            case MotionEventActions.Move:
                if (Element != null)
                    result = Element.TouchesMoved(touchInfo);
                break;

            case MotionEventActions.Up:
                if (Element != null)
                    result = Element.TouchesEnded(touchInfo);
                break;

            case MotionEventActions.Cancel:
                if (Element != null)
                    result = Element.TouchesCancelled(touchInfo);
                break;
        }

        return result;
    }

    #endregion

    #region Private Members

    /// <summary>
    /// Handles the invalidate.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="args">Arguments.</param>
    private void HandleInvalidate(object sender, System.EventArgs args)
    {
        Invalidate();
    }

    /// <summary>
    /// Callback for the OnGetPlatform event in the abstract control
    /// </summary>
    private IPlatform OnGetPlatformHandler()
    {
        return new AndroidPlatform();
    }

    /// <summary>
    /// Gets the size of the screen.
    /// </summary>
    /// <returns>The screen size.</returns>
    protected Size GetScreenSize()
    {
        var metrics = Context.Resources.DisplayMetrics;
        return new Size(metrics.WidthPixels, metrics.HeightPixels);
    }
    #endregion
}

#else
public class NControlViewRenderer : VisualElementRenderer<NControlView>
{
    /// <summary>
    /// The gesture recognizer.
    /// </summary>
    private UITouchesGestureRecognizer _gestureRecognizer;

    /// <summary>
    /// Used for registration with dependency service
    /// </summary>
    public new static void Init()
    {
        var temp = DateTime.Now;
    }

    /// <summary>
    /// Raises the element changed event.
    /// </summary>
    /// <param name="e">E.</param>
    protected override void OnElementChanged(ElementChangedEventArgs<NControlView> e)
    {
        base.OnElementChanged(e);

        if (e.OldElement != null)
        {
            if (null != _gestureRecognizer)
            {
                RemoveGestureRecognizer(_gestureRecognizer);
                _gestureRecognizer = null;
            }

            e.OldElement.OnInvalidate -= HandleInvalidate;
            e.OldElement.OnGetPlatform -= OnGetPlatformHandler;
        }

        if (e.NewElement != null)
        {
            e.NewElement.OnInvalidate += HandleInvalidate;

            if ((null == _gestureRecognizer) && (null != NativeView))
            {
                _gestureRecognizer = new UITouchesGestureRecognizer(e.NewElement, NativeView);
                NativeView.AddGestureRecognizer(_gestureRecognizer);
            }

            e.NewElement.OnGetPlatform += OnGetPlatformHandler;

            e.NewElement.Invalidate();
        }
    }

    /// <summary>
    /// Raises the element property changed event.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">E.</param>
    protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        base.OnElementPropertyChanged(sender, e);

        if (e.PropertyName == NControlView.IsClippedToBoundsProperty.PropertyName)
            Layer.MasksToBounds = Element.IsClippedToBounds;
        else if (e.PropertyName == NControlView.BackgroundColorProperty.PropertyName)
            Element.Invalidate();
        else if (e.PropertyName == NControlView.IsVisibleProperty.PropertyName)
            Element.Invalidate();
    }

    #region Drawing

    /// <summary>
    /// Draw the specified rect.
    /// </summary>
    /// <param name="rect">Rect.</param>
    public override void Draw(CoreGraphics.CGRect rect)
    {
        base.Draw(rect);

        using (CGContext context = UIGraphics.GetCurrentContext())
        {
            context.SetAllowsAntialiasing(true);
            context.SetShouldAntialias(true);
            context.SetShouldSmoothFonts(true);

            var canvas = new CGContextCanvas(context);
            Element.Draw(canvas, new NGraphics.Rect(rect.Left, rect.Top, rect.Width, rect.Height));
        }
    }
    #endregion

    #region Touch Handlers

    public override void TouchesBegan(Foundation.NSSet touches, UIEvent evt)
    {
        // Ignore buggy Xamarin touch events on iOS
    }

    public override void TouchesEnded(Foundation.NSSet touches, UIEvent evt)
    {
        // Ignore buggy Xamarin touch events on iOS
    }

    public override void TouchesCancelled(Foundation.NSSet touches, UIEvent evt)
    {
        // Ignore buggy Xamarin touch events on iOS
    }

    public override void TouchesMoved(Foundation.NSSet touches, UIEvent evt)
    {
        // Ignore buggy Xamarin touch events on iOS
    }

    #endregion

    #region Private Members

    /// <summary>
    /// Callback for the OnGetPlatform event in the abstract control
    /// </summary>
    private IPlatform OnGetPlatformHandler()
    {
        return new ApplePlatform();
    }

    /// <summary>
    /// Handles the invalidate.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="args">Arguments.</param>
    private void HandleInvalidate(object sender, System.EventArgs args)
    {
        SetNeedsDisplay();
    }
    #endregion
}

public sealed class UITouchesGestureRecognizer : UIGestureRecognizer
{
    #region Private Members

    /// <summary>
    /// The element.
    /// </summary>
    private NControlView _element;

    /// <summary>
    /// The native view.
    /// </summary>
    private UIView _nativeView;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="NControl.iOS.UITouchesGestureRecognizer"/> class.
    /// </summary>
    /// <param name="element">Element.</param>
    /// <param name="nativeView">Native view.</param>
    public UITouchesGestureRecognizer(NControlView element, UIView nativeView)
    {
        if (null == element)
        {
            throw new ArgumentNullException("element");
        }

        if (null == nativeView)
        {
            throw new ArgumentNullException("nativeView");
        }

        _element = element;
        _nativeView = nativeView;
    }

    /// <summary>
    /// Gets the touch points.
    /// </summary>
    /// <returns>The touch points.</returns>
    /// <param name="touches">Touches.</param>
    private IEnumerable<NGraphics.Point> GetTouchPoints(
        Foundation.NSSet touches)
    {
        var points = new List<NGraphics.Point>((int)touches.Count);
        foreach (UITouch touch in touches)
        {
            CGPoint touchPoint = touch.LocationInView(_nativeView);
            points.Add(new NGraphics.Point((double)touchPoint.X, (double)touchPoint.Y));
        }

        return points;
    }

    /// <summary>
    /// Toucheses the began.
    /// </summary>
    /// <param name="touches">Touches.</param>
    /// <param name="evt">Evt.</param>
    public override void TouchesBegan(Foundation.NSSet touches, UIEvent evt)
    {
        base.TouchesBegan(touches, evt);

        if (this._element.TouchesBegan(GetTouchPoints(touches)))
            this.State = UIGestureRecognizerState.Began;
        else
            this.State = UIGestureRecognizerState.Cancelled;
    }

    /// <summary>
    /// Toucheses the moved.
    /// </summary>
    /// <param name="touches">Touches.</param>
    /// <param name="evt">Evt.</param>
    public override void TouchesMoved(Foundation.NSSet touches, UIEvent evt)
    {
        base.TouchesMoved(touches, evt);

        if (this._element.TouchesMoved(GetTouchPoints(touches)))
            this.State = UIGestureRecognizerState.Changed;
    }

    /// <summary>
    /// Toucheses the ended.
    /// </summary>
    /// <param name="touches">Touches.</param>
    /// <param name="evt">Evt.</param>
    public override void TouchesEnded(Foundation.NSSet touches, UIEvent evt)
    {
        base.TouchesEnded(touches, evt);

        if (this._element.TouchesEnded(GetTouchPoints(touches)))
            this.State = UIGestureRecognizerState.Ended;
    }

    /// <summary>
    /// Toucheses the cancelled.
    /// </summary>
    /// <param name="touches">Touches.</param>
    /// <param name="evt">Evt.</param>
    public override void TouchesCancelled(Foundation.NSSet touches, UIEvent evt)
    {
        base.TouchesCancelled(touches, evt);

        this._element.TouchesCancelled(GetTouchPoints(touches));
        this.State = UIGestureRecognizerState.Cancelled;
    }
}
public class ApplePlatform : IPlatform
{
    public string Name
    {
        get
        {
#if __IOS__
            return "iOS";
#else
				return "Mac";
#endif
        }
    }

    public Task<Stream> OpenFileStreamForWritingAsync(string path)
    {
        return Task.FromResult((Stream)new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read));
    }

    public IImageCanvas CreateImageCanvas(Size size, double scale = 1.0, bool transparency = true)
    {
        var pixelWidth = (int)Math.Ceiling(size.Width * scale);
        var pixelHeight = (int)Math.Ceiling(size.Height * scale);
        var bitmapInfo = transparency ? CGImageAlphaInfo.PremultipliedFirst : CGImageAlphaInfo.NoneSkipFirst;
        var bitsPerComp = 8;
        var bytesPerRow = transparency ? 4 * pixelWidth : 4 * pixelWidth;
        var colorSpace = CGColorSpace.CreateDeviceRGB();
        var bitmap = new CGBitmapContext(IntPtr.Zero, pixelWidth, pixelHeight, bitsPerComp, bytesPerRow, colorSpace, bitmapInfo);
        return new CGBitmapContextCanvas(bitmap, scale);
    }

    public IImage CreateImage(Color[] colors, int width, double scale = 1.0)
    {
        var pixelWidth = width;
        var pixelHeight = colors.Length / width;
        var bitmapInfo = CGImageAlphaInfo.PremultipliedFirst;
        var bitsPerComp = 8;
        var bytesPerRow = width * 4;// ((4 * pixelWidth + 3)/4) * 4;
        var colorSpace = CGColorSpace.CreateDeviceRGB();
        var bitmap = new CGBitmapContext(IntPtr.Zero, pixelWidth, pixelHeight, bitsPerComp, bytesPerRow, colorSpace, bitmapInfo);
        var data = bitmap.Data;
        unsafe
        {
            fixed (Color* c = colors)
            {
                for (var y = 0; y < pixelHeight; y++)
                {
                    var s = (byte*)c + 4 * pixelWidth * y;
                    var d = (byte*)data + bytesPerRow * y;
                    for (var x = 0; x < pixelWidth; x++)
                    {
                        var b = *s++;
                        var g = *s++;
                        var r = *s++;
                        var a = *s++;
                        *d++ = a;
                        *d++ = (byte)((r * a) >> 8);
                        *d++ = (byte)((g * a) >> 8);
                        *d++ = (byte)((b * a) >> 8);
                    }
                }
            }
        }
        var image = bitmap.ToImage();
        return new CGImageImage(image, scale);
    }
    public IImage LoadImage(Stream stream)
    {
        var mem = new MemoryStream((int)stream.Length);
        stream.CopyTo(mem);
        unsafe
        {

#if NET45
                fixed (byte* x = mem.GetBuffer())
#else
            ArraySegment<byte> segment;
            if (!mem.TryGetBuffer(out segment))
            {
                throw new Exception("Could not get buffer from stream.");
            }
            fixed (byte* x = segment.Array)
#endif
            {
                var provider = new CGDataProvider(new IntPtr(x), (int)mem.Length, false);
                var image = CGImage.FromPNG(provider, null, false, CGColorRenderingIntent.Default)
                    ?? CGImage.FromJPEG(provider, null, false, CGColorRenderingIntent.Default);
                return new CGImageImage(image, 1);
            }
        }
    }
    public IImage LoadImage(string path)
    {
        var provider = new CGDataProvider(path);
        CGImage image;
        if (System.IO.Path.GetExtension(path).ToLowerInvariant() == ".png")
        {
            image = CGImage.FromPNG(provider, null, false, CGColorRenderingIntent.Default);
        }
        else
        {
            image = CGImage.FromJPEG(provider, null, false, CGColorRenderingIntent.Default);
        }
        return new CGImageImage(image, 1);
    }

    public static TextMetrics GlobalMeasureText(string text, Font font)
    {
        if (string.IsNullOrEmpty(text))
            return new TextMetrics();
        if (font == null)
            throw new ArgumentNullException("font");

        using (var atext = new NSMutableAttributedString(text))
        {

            atext.AddAttributes(new CTStringAttributes
            {
                ForegroundColorFromContext = true,
                Font = font.GetCTFont(),
            }, new NSRange(0, text.Length));

            using (var l = new CTLine(atext))
            {
                nfloat asc, desc, lead;

                var len = l.GetTypographicBounds(out asc, out desc, out lead);

                return new TextMetrics
                {
                    Width = len,
                    Ascent = asc,
                    Descent = desc,
                };
            }
        }

    }

    public TextMetrics MeasureText(string text, Font font)
    {
        return GlobalMeasureText(text, font);
    }
}

public class CGBitmapContextCanvas : CGContextCanvas, IImageCanvas
{
    CGBitmapContext context;
    readonly double scale;

    public Size Size { get { return new Size(context.Width / scale, context.Height / scale); } }
    public Size SizeInPixels { get { return new Size(context.Width, context.Height); } }
    public double Scale { get { return scale; } }

    public CGBitmapContextCanvas(CGBitmapContext context, double scale)
        : base(context)
    {
        this.context = context;
        this.scale = scale;

        var nscale = (nfloat)scale;
        this.context.TranslateCTM(0, context.Height);
        this.context.ScaleCTM(nscale, -nscale);
    }

    public IImage GetImage()
    {
        return new CGImageImage(this.context.ToImage(), scale);
    }
}

public class CGImageImage : IImage
{
    readonly CGImage image;
    readonly double scale;
    readonly Size size;

    public CGImage Image { get { return image; } }
    public double Scale { get { return scale; } }
    public Size Size { get { return size; } }

    public CGImageImage(CGImage image, double scale)
    {
        if (image == null)
            throw new ArgumentNullException("image");
        this.image = image;
        this.scale = scale;
        this.size = new Size(image.Width / scale, image.Height / scale);
    }

    public void SaveAsPng(string path)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentException("path");
        using (var dest = CGImageDestination.Create(NSUrl.FromFilename(path), "public.png", 1))
        {
            if (dest == null)
            {
                throw new InvalidOperationException(string.Format("Could not create image destination {0}.", path));
            }
            dest.AddImage(image);
            dest.Close();
        }
    }

    public void SaveAsPng(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException();

        using (var data = new NSMutableData())
        {
            using (var dest = CGImageDestination.Create(data, "public.png", 1))
            {
                if (dest == null)
                {
                    throw new InvalidOperationException(string.Format("Could not create image destination from {0}.", stream));
                }
                dest.AddImage(image);
                dest.Close();
            }
            data.AsStream().CopyTo(stream);
        }
    }
}

public class CGContextCanvas : ICanvas
{
    readonly CGContext context;

    public CGContext Context { get { return context; } }

    public CGContextCanvas(CGContext context)
    {
        this.context = context;
        //			context.InterpolationQuality = CGInterpolationQuality.High;
        context.TextMatrix = CGAffineTransform.MakeScale(1, -1);
    }

    public void SaveState()
    {
        context.SaveState();
    }
    public void Transform(Transform transform)
    {
        context.ConcatCTM(new CGAffineTransform(
            (nfloat)transform.A, (nfloat)transform.B,
            (nfloat)transform.C, (nfloat)transform.D,
            (nfloat)transform.E, (nfloat)transform.F));
    }
    public void RestoreState()
    {
        context.RestoreState();
    }

    CGGradient CreateGradient(IList<GradientStop> stops)
    {
        var n = stops.Count;
        var locs = new nfloat[n];
        var comps = new nfloat[4 * n];
        for (var i = 0; i < n; i++)
        {
            var s = stops[i];
            locs[i] = (nfloat)s.Offset;
            comps[4 * i + 0] = (nfloat)s.Color.Red;
            comps[4 * i + 1] = (nfloat)s.Color.Green;
            comps[4 * i + 2] = (nfloat)s.Color.Blue;
            comps[4 * i + 3] = (nfloat)s.Color.Alpha;
        }
        var cs = CGColorSpace.CreateDeviceRGB();
        return new CGGradient(cs, comps, locs);
    }

    private static NSString NSFontAttributeName = new NSString("NSFontAttributeName");

    public TextMetrics MeasureText(string text, Font font)
    {
        return ApplePlatform.GlobalMeasureText(text, font);
    }

    public void DrawText(string text, Rect frame, Font font, TextAlignment alignment = TextAlignment.Left, Pen pen = null, Brush brush = null)
    {
        if (string.IsNullOrEmpty(text))
            return;
        if (font == null)
            throw new ArgumentNullException("font");

        SetBrush(brush);

        using (var atext = new NSMutableAttributedString(text))
        {

            atext.AddAttributes(new CTStringAttributes
            {
                ForegroundColorFromContext = true,
                StrokeColor = pen != null ? pen.Color.GetCGColor() : null,
                Font = font.GetCTFont(),
            }, new NSRange(0, text.Length));

            using (var l = new CTLine(atext))
            {
                nfloat asc, desc, lead;
                var len = l.GetTypographicBounds(out asc, out desc, out lead);
                var pt = frame.TopLeft;

                switch (alignment)
                {
                    case TextAlignment.Left:
                        pt.X = frame.X;
                        break;
                    case TextAlignment.Center:
                        pt.X = frame.X + (frame.Width - len) / 2;
                        break;
                    case TextAlignment.Right:
                        pt.X = frame.Right - len;
                        break;
                }

                context.SaveState();
                context.TranslateCTM((nfloat)(pt.X), (nfloat)(pt.Y));
                context.TextPosition = CGPoint.Empty;
                l.Draw(context);
                context.RestoreState();
            }
        }
    }

    void DrawElement(Func<Rect> add, Pen pen = null, Brush brush = null)
    {
        if (pen == null && brush == null)
            return;

        var lgb = brush as LinearGradientBrush;
        if (lgb != null)
        {
            var cg = CreateGradient(lgb.Stops);
            context.SaveState();
            var frame = add();
            context.Clip();
            CGGradientDrawingOptions options = CGGradientDrawingOptions.DrawsBeforeStartLocation | CGGradientDrawingOptions.DrawsAfterEndLocation;
            var size = frame.Size;
            var start = Conversions.GetCGPoint(lgb.Absolute ? lgb.Start : frame.Position + lgb.Start * size);
            var end = Conversions.GetCGPoint(lgb.Absolute ? lgb.End : frame.Position + lgb.End * size);
            context.DrawLinearGradient(cg, start, end, options);
            context.RestoreState();
            brush = null;
        }

        var rgb = brush as RadialGradientBrush;
        if (rgb != null)
        {
            var cg = CreateGradient(rgb.Stops);
            context.SaveState();
            var frame = add();
            context.Clip();
            CGGradientDrawingOptions options = CGGradientDrawingOptions.DrawsBeforeStartLocation | CGGradientDrawingOptions.DrawsAfterEndLocation;
            var size = frame.Size;
            var start = Conversions.GetCGPoint(rgb.GetAbsoluteCenter(frame));
            var r = (nfloat)rgb.GetAbsoluteRadius(frame).Max;
            var end = Conversions.GetCGPoint(rgb.GetAbsoluteFocus(frame));
            context.DrawRadialGradient(cg, start, 0, end, r, options);
            context.RestoreState();
            brush = null;
        }

        if (pen != null || brush != null)
        {
            var mode = SetPenAndBrush(pen, brush);

            add();
            context.DrawPath(mode);
        }
    }

    static bool IsValid(double v)
    {
        return !double.IsNaN(v) && !double.IsInfinity(v);
    }

    public void DrawPath(IEnumerable<PathOp> ops, Pen pen = null, Brush brush = null)
    {
        if (pen == null && brush == null)
            return;

        DrawElement(() => {

            var bb = new BoundingBoxBuilder();

            foreach (var op in ops)
            {
                var mt = op as MoveTo;
                if (mt != null)
                {
                    var p = mt.Point;
                    if (!IsValid(p.X) || !IsValid(p.Y))
                        continue;
                    context.MoveTo((nfloat)p.X, (nfloat)p.Y);
                    bb.Add(p);
                    continue;
                }
                var lt = op as LineTo;
                if (lt != null)
                {
                    var p = lt.Point;
                    if (!IsValid(p.X) || !IsValid(p.Y))
                        continue;
                    context.AddLineToPoint((nfloat)p.X, (nfloat)p.Y);
                    bb.Add(p);
                    continue;
                }
                var at = op as ArcTo;
                if (at != null)
                {
                    var p = at.Point;
                    if (!IsValid(p.X) || !IsValid(p.Y))
                        continue;
                    var pp = Conversions.GetPoint(context.GetPathCurrentPoint());
                    if (pp == p)
                        continue;
                    Point c1, c2;
                    at.GetCircles(pp, out c1, out c2);

                    var circleCenter = (at.LargeArc ^ at.SweepClockwise) ? c1 : c2;

                    var startAngle = (float)Math.Atan2(pp.Y - circleCenter.Y, pp.X - circleCenter.X);
                    var endAngle = (float)Math.Atan2(p.Y - circleCenter.Y, p.X - circleCenter.X);

                    if (!IsValid(circleCenter.X) || !IsValid(circleCenter.Y) || !IsValid(startAngle) || !IsValid(endAngle))
                    {
                        context.MoveTo((nfloat)p.X, (nfloat)p.Y);
                        continue;
                    }

                    var clockwise = !at.SweepClockwise;

                    context.AddArc((nfloat)circleCenter.X, (nfloat)circleCenter.Y, (nfloat)at.Radius.Min, startAngle, endAngle, clockwise);

                    bb.Add(p);
                    continue;
                }
                var ct = op as CurveTo;
                if (ct != null)
                {
                    var p = ct.Point;
                    if (!IsValid(p.X) || !IsValid(p.Y))
                        continue;
                    var c1 = ct.Control1;
                    var c2 = ct.Control2;
                    if (!IsValid(c1.X) || !IsValid(c1.Y) || !IsValid(c2.X) || !IsValid(c2.Y))
                    {
                        context.MoveTo((nfloat)p.X, (nfloat)p.Y);
                        continue;
                    }
                    context.AddCurveToPoint((nfloat)c1.X, (nfloat)c1.Y, (nfloat)c2.X, (nfloat)c2.Y, (nfloat)p.X, (nfloat)p.Y);
                    bb.Add(p);
                    bb.Add(c1);
                    bb.Add(c2);
                    continue;
                }
                var cp = op as ClosePath;
                if (cp != null)
                {
                    context.ClosePath();
                    continue;
                }

                throw new NotSupportedException("Path Op " + op);
            }

            return bb.BoundingBox;

        }, pen, brush);
    }
    // http://stackoverflow.com/a/2835659/338
    void AddRoundedRect(CGRect rrect, CGSize corner)
    {
        var rx = corner.Width;
        if (rx * 2 > rrect.Width)
        {
            rx = rrect.Width / 2;
        }
        var ry = corner.Height;
        if (ry * 2 > rrect.Height)
        {
            ry = rrect.Height / 2;
        }
        var path = CGPath.FromRoundedRect(rrect, rx, ry);
        context.AddPath(path);
    }
    public void DrawRectangle(Rect frame, Size corner, Pen pen = null, Brush brush = null)
    {
        if (pen == null && brush == null)
            return;

        DrawElement(() => {
            if (corner.Width > 0 || corner.Height > 0)
            {
                AddRoundedRect(Conversions.GetCGRect(frame), Conversions.GetCGSize(corner));
            }
            else
            {
                context.AddRect(Conversions.GetCGRect(frame));
            }
            return frame;
        }, pen, brush);
    }
    public void DrawEllipse(Rect frame, Pen pen = null, Brush brush = null)
    {
        if (pen == null && brush == null)
            return;

        DrawElement(() => {
            context.AddEllipseInRect(Conversions.GetCGRect(frame));
            return frame;
        }, pen, brush);
    }
    public void DrawImage(IImage image, Rect frame, double alpha = 1.0)
    {
        var cgi = image as CGImageImage;

        if (cgi != null)
        {
            var i = cgi.Image;
            var h = frame.Height;
            context.SaveState();
            context.SetAlpha((nfloat)alpha);
            context.TranslateCTM((nfloat)frame.X, (nfloat)(h + frame.Y));
            context.ScaleCTM(1, -1);
            context.DrawImage(new CGRect(0, 0, (nfloat)frame.Width, (nfloat)frame.Height), cgi.Image);
            context.RestoreState();
        }
    }

    CGPathDrawingMode SetPenAndBrush(Pen pen, Brush brush)
    {
        var mode = CGPathDrawingMode.EOFill;
        if (brush != null)
        {
            SetBrush(brush);
            if (pen != null)
                mode = CGPathDrawingMode.EOFillStroke;
        }
        if (pen != null)
        {
            SetPen(pen);
            if (brush == null)
                mode = CGPathDrawingMode.Stroke;
        }
        return mode;
    }

    void SetPen(Pen pen)
    {
        context.SetStrokeColor((nfloat)pen.Color.Red, (nfloat)pen.Color.Green, (nfloat)pen.Color.Blue, (nfloat)pen.Color.Alpha);
        context.SetLineWidth((nfloat)pen.Width);

        if (pen.DashPattern != null && pen.DashPattern.Any())
        {
            var pattern = pen.DashPattern
                .Select(dp => (nfloat)dp)
                .ToArray();

            context.SetLineDash(0, pattern, pattern.Length);
        }
        else
        {
            context.SetLineDash(0, null, 0);
        }
    }

    void SetBrush(Brush brush)
    {
        var sb = brush as SolidBrush;
        if (sb != null)
        {
            context.SetFillColor((nfloat)sb.Color.Red, (nfloat)sb.Color.Green, (nfloat)sb.Color.Blue, (nfloat)sb.Color.Alpha);
        }
    }
}

public static class Conversions
{
    public static CGPoint GetCGPoint(this Point point)
    {
        return new CGPoint((nfloat)point.X, (nfloat)point.Y);
    }
    public static Point GetPoint(this CGPoint point)
    {
        return new Point(point.X, point.Y);
    }
    public static Point ToPoint(this CGPoint point)
    {
        return new Point(point.X, point.Y);
    }
    public static Size GetSize(this CGSize size)
    {
        return new Size(size.Width, size.Height);
    }
    public static CGSize GetCGSize(this Size size)
    {
        return new CGSize((nfloat)size.Width, (nfloat)size.Height);
    }
    public static CGRect GetCGRect(this Rect frame)
    {
        return new CGRect((nfloat)frame.X, (nfloat)frame.Y, (nfloat)frame.Width, (nfloat)frame.Height);
    }
    public static Rect GetRect(this CGRect rect)
    {
        return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
    }
    public static CTFont GetCTFont(this Font font)
    {
        return new CTFont(font.Name, (nfloat)font.Size);
    }
    public static CGColor GetCGColor(this Color color)
    {
        return new CGColor((nfloat)color.Red, (nfloat)color.Green, (nfloat)color.Blue, (nfloat)color.Alpha);
    }
    public static Color GetColor(this CGColor color)
    {
        var c = color.Components;
        return Color.FromRGB(c[0], c[1], c[2], c[3]);
    }
#if __IOS__ || __TVOS__
    public static UIKit.UIColor GetUIColor(this Color color)
    {
        return UIKit.UIColor.FromRGBA(color.R, color.G, color.B, color.A);
    }
    public static Color GetColor(this UIKit.UIColor color)
    {
        nfloat r, g, b, a;
        color.GetRGBA(out r, out g, out b, out a);
        return Color.FromRGB(r, g, b, a);
    }
    public static UIKit.UIImage GetUIImage(this IImage image)
    {
        var c = (CGImageImage)image;
        return new UIKit.UIImage(c.Image, (nfloat)c.Scale, UIKit.UIImageOrientation.Up);
    }
#else
		public static AppKit.NSImage GetNSImage (this IImage image)
		{
			var c = (CGImageImage)image;
			return new AppKit.NSImage (c.Image, Conversions.GetCGSize (c.Size));
		}
#endif
}
#endif