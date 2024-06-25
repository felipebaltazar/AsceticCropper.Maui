
namespace Maui.AsceticCropper;

public static class Mixins
{
    public static NGraphics.Color AsNColor(this Color color)
    {
        return new NGraphics.Color(color.Red, color.Green, color.Blue, color.Alpha);
    }
}
