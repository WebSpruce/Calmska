using Microsoft.Extensions.Logging;

namespace Calmska
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("CanelaBlack.otf", "CanelaBlack");
                    fonts.AddFont("CanelaLight.otf", "CanelaLight");
                    fonts.AddFont("CanelaMedium.otf", "CanelaMedium");
                    fonts.AddFont("CanelaRegular.otf", "CanelaRegular");
                    fonts.AddFont("MADEINFINITYBlack.otf", "MADEINFINITYBlack");
                    fonts.AddFont("MADEINFINITYLight.otf", "MADEINFINITYLight");
                    fonts.AddFont("MADEINFINITYMedium.otf", "MADEINFINITYMedium");
                    fonts.AddFont("MADEINFINITYRegular.otf", "MADEINFINITYRegular");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
