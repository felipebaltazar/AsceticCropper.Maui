# asceticcropper
Forked from the amazingly popular original [asceticcropper](https://github.com/jocontacter/asceticcropper) Library, this Compat version aims to ease your migration from Xamarin.Forms to .NET MAUI with a compatible implementation to get you up and running without rewriting the parts of your app that relied on the original library.

Thanks to the Original Authors: jocontacter.


 [![NuGet](https://img.shields.io/nuget/v/AsceticCropper.Maui.svg)](https://www.nuget.org/packages/AsceticCropper.Maui/)

 ![](https://img.shields.io/github/issues/felipebaltazar/asceticcropper.maui.svg)

## Getting started

- Install the AsceticCropper.Maui package

 ```
 Install-Package AsceticCropper.Maui
 ```

- Add AsceticCropper.Maui declaration to your MauiAppBuilder

```csharp
public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseAsceticCropper()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		return builder.Build();
	}
}
```

## Demo


![](https://jocontacter.github.io/asceticcropper/Images/scr1.png)|![](https://jocontacter.github.io/asceticcropper/Images/scr2.png)|![](https://jocontacter.github.io/asceticcropper/Images/scr3.png)
-|-|-


### Features

- Customizable masks: dash properties, border color, border width, CornerRadius;
- Predefined masks: `RectangleMaskPainter` with CornerRadius(with apply corners), `CircleMaskPainter` with different side ratio's: oval/circle, `GridMaskPainter`;
- Inherit from `MaskPainter` and make your own;
