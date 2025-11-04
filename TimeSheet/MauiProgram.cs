using System.Reflection;
using CommunityToolkit.Maui;
using Maui.NullableDateTimePicker;
using Microsoft.Extensions.Logging;
using TimeSheet.Extensions;
using TimeSheet.Interfaces;
using TimeSheet.Services;

namespace TimeSheet;

public static class MauiProgram {

    public static Assembly ExecutingAssembly => Assembly.GetExecutingAssembly();

    public static MauiApp CreateMauiApp() {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureNullableDateTimePicker()
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<App>();
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddConfigStrategies(typeof(IConfigStrategy).Assembly);
        builder.Services.AddTransient<IInvoiceService, PdfService>();
        
        builder.ConfigureContainer(new DefaultServiceProviderFactory(
            new ServiceProviderOptions {
                ValidateOnBuild = true,
                ValidateScopes = true
            }));

        return builder.Build();
    }
}