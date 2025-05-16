using Microsoft.Extensions.Logging;
using MauiApp1.Services;
using MauiApp1.ViewModels;
using MauiApp1.Views;
using System.Diagnostics;

namespace MauiApp1;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
		{
			Debug.WriteLine($"UNHANDLED EXCEPTION: {e.ExceptionObject}");
		};
		
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		// Register HttpClient
		builder.Services.AddSingleton<HttpClient>(sp => 
		{
			var handler = new HttpClientHandler
			{
				ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
			};
			
			var client = new HttpClient(handler);
			client.Timeout = TimeSpan.FromSeconds(30);
			
			Debug.WriteLine("HttpClient créé avec timeout de 30 secondes");
			return client;
		});
		
		// Configure API URL based on platform
		string apiUrl;
		
#if ANDROID
		// Android emulator needs to use 10.0.2.2 to access host machine's localhost
		apiUrl = "http://10.0.2.2:5000";
#elif IOS
		// iOS simulator can use localhost
		apiUrl = "http://localhost:5000";
#else
		// Windows and other platforms
		apiUrl = "http://localhost:5000";
#endif

		Debug.WriteLine($"API URL configurée: {apiUrl}");
		builder.Services.AddSingleton<string>(serviceProvider => apiUrl);
		
		// Register Services
		builder.Services.AddSingleton<TaskService>();
		
		// Register ViewModels
		builder.Services.AddSingleton<TaskListViewModel>();
		builder.Services.AddTransient<TaskDetailViewModel>();
		builder.Services.AddTransient<NewTaskViewModel>();
		
		// Register Views
		builder.Services.AddSingleton<TaskListPage>();
		builder.Services.AddTransient<TaskDetailPage>();
		builder.Services.AddTransient<NewTaskPage>();

		var app = builder.Build();
		Debug.WriteLine("Application MAUI construite avec succès");
		return app;
	}
}
