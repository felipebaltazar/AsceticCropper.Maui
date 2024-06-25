using ICanvas = NGraphics.ICanvas;
using Rect = NGraphics.Rect;

namespace Maui.AsceticCropper;

public interface IPainter
{
    void Paint(CropperControl control, ICanvas canvas, Rect rect);
}
