using FFImageLoading.Maui;
using Microsoft.Maui.Controls.Compatibility.Hosting;

namespace Maui.AsceticCropper;

public static class MauiProgramExtensions
{
    public static MauiAppBuilder UseAsceticCropper(this MauiAppBuilder program)
    {
        program
            .UseFFImageLoading()
            .UseMauiCompatibility()
            .ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler(typeof(NControlView), typeof(NControlViewRenderer));
            });

        return program;
    }
}
