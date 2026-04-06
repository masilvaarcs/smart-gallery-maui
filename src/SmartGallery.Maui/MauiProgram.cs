using Microsoft.Extensions.Logging;
using SmartGallery.Maui.Services;
using SmartGallery.Maui.ViewModels;
using SmartGallery.Maui.Views;

namespace SmartGallery.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Services
		builder.Services.AddSingleton<SettingsService>();
		builder.Services.AddSingleton(sp =>
		{
			var settings = sp.GetRequiredService<SettingsService>();
			var http = new HttpClient { BaseAddress = new Uri(settings.ApiUrl + "/") };
			http.DefaultRequestHeaders.Add("Accept", "application/json");
			return http;
		});
		builder.Services.AddSingleton<GalleryApiService>();

		// ViewModels
		builder.Services.AddTransient<GalleryViewModel>();
		builder.Services.AddTransient<UploadViewModel>();
		builder.Services.AddTransient<DetailViewModel>();
		builder.Services.AddTransient<DashboardViewModel>();
		builder.Services.AddTransient<SettingsViewModel>();

		// Pages
		builder.Services.AddTransient<GalleryPage>();
		builder.Services.AddTransient<UploadPage>();
		builder.Services.AddTransient<DetailPage>();
		builder.Services.AddTransient<DashboardPage>();
		builder.Services.AddTransient<SettingsPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
