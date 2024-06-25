using FFImageLoading.Work;

namespace Maui.AsceticCropper;

public interface ICropTransformation : ITransformation
{
    double XOffsetFactor { get; set; }
    double YOffsetFactor { get; set; }
    double WidthFactor { get; set; }
    double HeightFactor { get; set; }
}
