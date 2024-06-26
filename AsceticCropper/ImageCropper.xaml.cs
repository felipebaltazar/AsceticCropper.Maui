using FFImageLoading.Maui;
using FFImageLoading.Work;
using FFImageLoading;
using Microsoft.Maui.Controls;
using ImageSource = Microsoft.Maui.Controls.ImageSource;

namespace Maui.AsceticCropper;

public partial class ImageCropperView : Grid
{
	public ImageCropperView()
	{
		InitializeComponent();
        image.SizeChanged += Image_SizeChanged;
    }

    public static readonly BindableProperty MaskPainterProperty = BindableProperty.Create(nameof(MaskPainter), typeof(MaskPainter), typeof(ImageCropperView), new CircleMaskPainter() { MaskWidth = 100, MaskHeight = 100 }, BindingMode.OneWay);
    public static readonly BindableProperty PhotoSourceProperty = BindableProperty.Create(nameof(PhotoSource), typeof(ImageSource), typeof(ImageCropperView), null, BindingMode.OneWay);
    /// <summary>
    /// Background color property.
    /// </summary>
    public static new BindableProperty BackgroundColorProperty =
        BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(ImageCropperView), Colors.Transparent, BindingMode.OneWay);

    /// <summary>
    /// The border width property.
    /// </summary>
    public static BindableProperty BorderWidthProperty =
        BindableProperty.Create(nameof(BorderWidth), typeof(double), typeof(ImageCropperView), 5.0, BindingMode.OneWay);

    /// <summary>
    /// The border color property.
    /// </summary>
    public static BindableProperty BorderColorProperty =
        BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(ImageCropperView), Colors.CornflowerBlue, BindingMode.OneWay);

    /// <summary>
    /// The border dash property.
    /// </summary>
    public static BindableProperty IsDashedProperty =
        BindableProperty.Create(nameof(IsDashed), typeof(bool), typeof(ImageCropperView),
            false, BindingMode.OneWay);

    /// <summary>
    /// The border stroke pattern property.
    /// </summary>
    public static BindableProperty DashPatternStrokeProperty =
        BindableProperty.Create(nameof(DashPatternStroke), typeof(float), typeof(ImageCropperView),
            4.0f, BindingMode.OneWay);

    /// <summary>
    /// The border stroke pattern spacing property.
    /// </summary>
    public static BindableProperty DashPatternSpaceProperty =
        BindableProperty.Create(nameof(DashPatternSpace), typeof(float), typeof(ImageCropperView),
            4.0f, BindingMode.OneWay);

    /// <summary>
    /// Gets or sets the color which will fill the border of a VisualElement. This is a bindable property.
    /// </summary>
    /// <value>The color of the border.</value>
    public Color BorderColor
    {
        get => (Color)GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the width which will have the border of a VisualElement. This is a bindable property.
    /// </summary>
    /// <value>The width of the border.</value>
    public double BorderWidth
    {
        get => (double)GetValue(BorderWidthProperty);
        set => SetValue(BorderWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the color which will fill the background of a VisualElement. This is a bindable property.
    /// </summary>
    /// <value>The color of the background.</value>
    public new Color BackgroundColor
    {
        get => (Color)GetValue(BackgroundColorProperty);
        set => SetValue(BackgroundColorProperty, value);
    }

    public MaskPainter MaskPainter
    {
        get => (MaskPainter)GetValue(MaskPainterProperty);
        set => SetValue(MaskPainterProperty, value);
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

    /// <summary>
    /// ImageInformation.
    /// </summary>
    public static BindableProperty ImageInformationProperty =
        BindableProperty.Create(nameof(ImageInformation), typeof(ImageInformation), typeof(ImageCropperView),
            null, BindingMode.OneWayToSource);

    /// <summary>
    /// Gets or sets the ImageInformation. This is a bindable property.
    /// </summary>
    /// <value>ImageInformation.</value>
    public ImageInformation ImageInformation
    {
        get => (ImageInformation)GetValue(ImageInformationProperty);
        set => SetValue(ImageInformationProperty, value);
    }

    public ImageSource PhotoSource => (ImageSource)GetValue(PhotoSourceProperty);

    protected override void OnPropertyChanged(string propertyName)
    {
        base.OnPropertyChanged(propertyName);

        switch (propertyName)
        {
            case nameof(MaskPainter):
                frame.CustomPainter = MaskPainter;
                break;
            case nameof(PhotoSource):
                image.Source = PhotoSource;
                break;
            case nameof(BackgroundColor):
                frame.BackgroundColor = BackgroundColor;
                break;
            case nameof(BorderColor):
                frame.BorderColor = BorderColor;
                break;
            case nameof(BorderWidth):
                frame.BorderWidth = BorderWidth;
                break;
            case nameof(IsDashed):
                frame.IsDashed = IsDashed;
                break;
            case nameof(DashPatternStroke):
                frame.DashPatternStroke = DashPatternStroke;
                break;
            case nameof(DashPatternSpace):
                frame.DashPatternSpace = DashPatternSpace;
                break;
        }
    }

    double startX;
    double startY;
    double imageScale;
    //to exclude approximations during multiple scalings in pinch gestures
    double originalMaskWidth;
    double originalMaskRatio;
    double originalCornerRadius;
    bool IsNearestHorizontal;
    double imageWidth;
    double imageHeight;
    double horizontalPadding;
    double verticalPadding;

    private bool IsPanStarted;
    void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (e.StatusType == GestureStatus.Started)
        {
            Console.WriteLine($"Pan STARTED coords X:{e.TotalX}, Y:{e.TotalY}");
            startX = MaskPainter.MaskX;
            startY = MaskPainter.MaskY;
            IsPanStarted = true;
        }
        else if (e.StatusType == GestureStatus.Running && IsPanStarted)
        {
            //Console.WriteLine($"Pan coords X:{e.TotalX}, Y:{e.TotalY}" );

            var translationX = startX + e.TotalX;
            var translationY = startY + e.TotalY;

            translationX = Clamp(translationX
                , horizontalPadding + MaskPainter.MaskWidth / 2
                , imageWidth + horizontalPadding - MaskPainter.MaskWidth / 2);

            translationY = Clamp(translationY
                , verticalPadding + MaskPainter.MaskHeight / 2
                , imageHeight + verticalPadding - MaskPainter.MaskHeight / 2);

            MaskPainter.MaskX = translationX;
            MaskPainter.MaskY = translationY;
            MaskPainter.InvokeInvalidate();
        }
    }

    void PinchGestureRecognizer_PinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
    {
        if (e.Status == GestureStatus.Running)
        {
            //Console.WriteLine("Pinch Scale: " + e.Scale);

            double w;
            double h;

            if (IsNearestHorizontal)
            {
                w = MaskPainter.MaskWidth * e.Scale;
                w = Clamp(w, 100, imageWidth);
                h = w / originalMaskRatio;
            }
            else
            {
                h = MaskPainter.MaskHeight * e.Scale;
                h = Clamp(h, 100, imageHeight);
                w = h * originalMaskRatio;
            }

            var xminoffset = MaskPainter.MaskX - (horizontalPadding + w / 2);
            var xmaxoffset = horizontalPadding + imageWidth - w / 2 - MaskPainter.MaskX;
            var yminoffset = MaskPainter.MaskY - (verticalPadding + h / 2);
            var ymaxoffset = verticalPadding + imageHeight - h / 2 - MaskPainter.MaskY;

            if (xminoffset <= 0 && xmaxoffset <= 0)
            {
                return;
            }
            if (yminoffset <= 0 && ymaxoffset <= 0)
            {
                return;
            }

            if (xminoffset < 0)
            {
                MaskPainter.MaskX -= xminoffset;
            }
            if (xmaxoffset < 0 && xminoffset > 0)
            {
                MaskPainter.MaskX += xmaxoffset;
            }
            if (yminoffset < 0)
            {
                MaskPainter.MaskY -= yminoffset;
            }
            if (ymaxoffset < 0)
            {
                MaskPainter.MaskY += ymaxoffset;
            }

            MaskPainter.MaskWidth = w;
            MaskPainter.MaskHeight = h;
            if (MaskPainter is RectangleMaskPainter rectanglePainter)
            {
                var currentScale = MaskPainter.MaskWidth / originalMaskWidth;
                rectanglePainter.CornerRadius = originalCornerRadius * currentScale * e.Scale;
            }

            MaskPainter.InvokeInvalidate();
        }
        else if (e.Status == GestureStatus.Completed || e.Status == GestureStatus.Canceled)
        {
            Console.WriteLine("Pinch Finished");
        }
        else if (e.Status == GestureStatus.Started)
        {
            Console.WriteLine("Pinch Started");
        }
    }

    T Clamp<T>(T value, T minimum, T maximum) where T : IComparable
    {
        if (value.CompareTo(minimum) < 0)
            return minimum;
        if (value.CompareTo(maximum) > 0)
            return maximum;

        return value;
    }

    void CachedImage_Success(object sender, CachedImageEvents.SuccessEventArgs e)
    {
        ImageInformation = e.ImageInformation;
        if (image.Height > 0)
        {
            CalculateScale(ImageInformation);
        }
    }

    private void Image_SizeChanged(object sender, EventArgs e)
    {
        if (ImageInformation != null)
        {
            CalculateScale(ImageInformation);
        }
    }

    public void InvalidateMaskLayout()
    {
        if (ImageInformation != null)
        {
            CalculateScale(ImageInformation);
        }
    }

    void CalculateScale(ImageInformation info)
    {
        var isLandscape = info.CurrentWidth > info.CurrentHeight;
        var viewRatio = image.Width / image.Height;
        var imageRatio = (double)info.CurrentWidth / info.CurrentHeight;

        if (viewRatio > imageRatio)
        {
            imageScale = image.Height / info.CurrentHeight;
        }
        else
        {
            imageScale = image.Width / info.CurrentWidth;
        }

        imageWidth = (isLandscape ? info.CurrentHeight : info.CurrentWidth) * imageScale;
        imageHeight = (isLandscape ? info.CurrentWidth : info.CurrentHeight) * imageScale;
        horizontalPadding = (image.Width - imageWidth) / 2;
        verticalPadding = (image.Height - imageHeight) / 2;

        MainThread.InvokeOnMainThreadAsync(() =>
        {
            MaskPainter.MaskX = image.Width / 2;
            MaskPainter.MaskY = image.Height / 2;
            //var min = Math.Min(imageWidth, imageHeight);
            var maskRatio = MaskPainter.MaskWidth / MaskPainter.MaskHeight;
            IsNearestHorizontal = imageRatio < maskRatio;

            if (IsNearestHorizontal)
            {
                MaskPainter.MaskWidth = imageWidth;
                MaskPainter.MaskHeight = MaskPainter.MaskWidth / maskRatio;
            }
            else
            {
                MaskPainter.MaskHeight = imageHeight;
                MaskPainter.MaskWidth = MaskPainter.MaskHeight * maskRatio;
            }

            //saving reference values 
            originalMaskRatio = maskRatio;
            originalMaskWidth = MaskPainter.MaskWidth;
            originalCornerRadius = (MaskPainter as RectangleMaskPainter)?.CornerRadius ?? 0;

            MaskPainter.InvokeInvalidate();
        });
    }

    /// <summary>
    /// Gets the image as JPEG stream.
    /// </summary>
    /// <returns>The image as JPEG async.</returns>
    /// <param name="quality">Quality.</param>
    /// <param name="maxWidth">Max width.</param>
    /// <param name="maxHeight">Max height.</param>
    public Task<Stream> GetImageAsJpegAsync(int quality = 90, int maxWidth = 0, int maxHeight = 0)
    {
#if IOS
	    var imageService = this.FindMauiContext()
                .Services
                .GetRequiredService<IImageService<UIKit.UIImage>>();
#else 
        var imageService = this.FindMauiContext()
            .Services
            .GetRequiredService<IImageService<FFImageLoading.Drawables.SelfDisposingBitmapDrawable>>();
#endif
        TaskParameter task = null;

        if (PhotoSource is FileImageSource file)
        {

            if (file.File.Split('/').Length == 1)
            {
                // Here we when resource file used.
                task = imageService.LoadCompiledResource(file.File);
            }
            else
            {
                task = imageService.LoadFile(file.File);
            }
        }
        else if (PhotoSource is StreamImageSource stream)
        {
            task = imageService.LoadStream(stream.Stream);
        }
        else if (PhotoSource is UriImageSource uri)
        {
            task = imageService.LoadUrl(uri.Uri.OriginalString);
        }

        var transformations = new List<ITransformation>();

        //0.0->1.0
        var xPreOffset = (MaskPainter.MaskX - MaskPainter.MaskWidth / 2 - horizontalPadding) / imageWidth;
        var yPreOffset = (MaskPainter.MaskY - MaskPainter.MaskHeight / 2 - verticalPadding) / imageHeight;

        var cropTransformation = CrossCropTransformation.Current;
        cropTransformation.XOffsetFactor = xPreOffset;
        cropTransformation.YOffsetFactor = yPreOffset;
        cropTransformation.WidthFactor = MaskPainter.MaskWidth / imageWidth;
        cropTransformation.HeightFactor = MaskPainter.MaskHeight / imageHeight;

        transformations.Insert(0, cropTransformation);

        //if (ImageRotation != 0)
        //    transformations.Insert(0, new RotateTransformation(Math.Abs(ImageRotation), ImageRotation < 0) { Resize = true });

        return task
            .WithCache(FFImageLoading.Cache.CacheType.Disk)
            .Transform(transformations)
            .DownSample(maxWidth, maxHeight)
            .AsJPGStreamAsync(imageService, quality);
    }
}