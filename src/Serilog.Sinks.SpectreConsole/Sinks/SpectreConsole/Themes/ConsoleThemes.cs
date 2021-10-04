using System.Collections.Generic;
using Spectre.Console;

namespace Serilog.Sinks.SpectreConsole.Themes
{
    static class ConsoleThemes
    {
        public static ConsoleTheme Code(ExceptionSettings? exceptionSettings) => new ConsoleTheme(
            new Dictionary<ConsoleThemeStyle, Style>
            {
                [ConsoleThemeStyle.Text] = new Style(foreground: Color.Grey85),
                [ConsoleThemeStyle.SecondaryText] = new Style(foreground: Color.Grey58),
                [ConsoleThemeStyle.TertiaryText] = new Style(foreground: Color.Grey42),
                [ConsoleThemeStyle.Invalid] = new Style(foreground: Color.Yellow, decoration: Decoration.Bold),
                [ConsoleThemeStyle.Null] = new Style(foreground: Color.DeepSkyBlue2),
                [ConsoleThemeStyle.Name] = new Style(foreground: Color.SteelBlue1_1),
                [ConsoleThemeStyle.String] = new Style(foreground: Color.LightSalmon1),
                [ConsoleThemeStyle.Number] = new Style(foreground: Color.DarkSeaGreen2),
                [ConsoleThemeStyle.Boolean] = new Style(foreground: Color.DeepSkyBlue2),
                [ConsoleThemeStyle.Scalar] = new Style(foreground: Color.Aquamarine3),
                [ConsoleThemeStyle.LevelVerbose] = new Style(foreground: Color.White),
                [ConsoleThemeStyle.LevelDebug] = new Style(foreground: Color.White),
                [ConsoleThemeStyle.LevelInformation] = new Style(foreground: Color.White, decoration: Decoration.Bold),
                [ConsoleThemeStyle.LevelWarning] = new Style(foreground: Color.Wheat1),
                [ConsoleThemeStyle.LevelError] = new Style(foreground: Color.DeepPink2, background: Color.Grey27),
                [ConsoleThemeStyle.LevelFatal] = new Style(foreground: Color.DeepPink2, background: Color.Grey27),
            }, exceptionSettings);

        public static ConsoleTheme Literate(ExceptionSettings? exceptionSettings) => new ConsoleTheme(
            new Dictionary<ConsoleThemeStyle, Style>
            {
                [ConsoleThemeStyle.Text] = new Style(foreground: Color.White),
                [ConsoleThemeStyle.SecondaryText] = new Style(foreground: Color.Silver),
                [ConsoleThemeStyle.TertiaryText] = new Style(foreground: Color.Grey),
                [ConsoleThemeStyle.Invalid] = new Style(foreground: Color.Yellow),
                [ConsoleThemeStyle.Null] = new Style(foreground: Color.Blue),
                [ConsoleThemeStyle.Name] = new Style(foreground: Color.Silver),
                [ConsoleThemeStyle.String] = new Style(foreground: Color.Aqua),
                [ConsoleThemeStyle.Number] = new Style(foreground: Color.Fuchsia),
                [ConsoleThemeStyle.Boolean] = new Style(foreground: Color.Blue),
                [ConsoleThemeStyle.Scalar] = new Style(foreground: Color.Lime),
                [ConsoleThemeStyle.LevelVerbose] = new Style(foreground: Color.Silver),
                [ConsoleThemeStyle.LevelDebug] = new Style(foreground: Color.Silver),
                [ConsoleThemeStyle.LevelInformation] = new Style(foreground: Color.White),
                [ConsoleThemeStyle.LevelWarning] = new Style(foreground: Color.Yellow),
                [ConsoleThemeStyle.LevelError] = new Style(foreground: Color.White, background: Color.Red),
                [ConsoleThemeStyle.LevelFatal] = new Style(foreground: Color.White, background: Color.Red),
            }, exceptionSettings);

        public static ConsoleTheme Grayscale(ExceptionSettings? exceptionSettings) => new ConsoleTheme(
            new Dictionary<ConsoleThemeStyle, Style>
            {
                [ConsoleThemeStyle.Text] = new Style(foreground: Color.White),
                [ConsoleThemeStyle.SecondaryText] = new Style(foreground: Color.Silver),
                [ConsoleThemeStyle.TertiaryText] = new Style(foreground: Color.Grey),
                [ConsoleThemeStyle.Invalid] = new Style(foreground: Color.White, background: Color.Grey),
                [ConsoleThemeStyle.Null] = new Style(foreground: Color.White),
                [ConsoleThemeStyle.Name] = new Style(foreground: Color.Silver),
                [ConsoleThemeStyle.String] = new Style(foreground: Color.White),
                [ConsoleThemeStyle.Number] = new Style(foreground: Color.White),
                [ConsoleThemeStyle.Boolean] = new Style(foreground: Color.White),
                [ConsoleThemeStyle.Scalar] = new Style(foreground: Color.White),
                [ConsoleThemeStyle.LevelVerbose] = new Style(foreground: Color.Grey),
                [ConsoleThemeStyle.LevelDebug] = new Style(foreground: Color.Grey),
                [ConsoleThemeStyle.LevelInformation] = new Style(foreground: Color.White),
                [ConsoleThemeStyle.LevelWarning] = new Style(foreground: Color.White, background: Color.Grey),
                [ConsoleThemeStyle.LevelError] = new Style(foreground: Color.Black, background: Color.White),
                [ConsoleThemeStyle.LevelFatal] = new Style(foreground: Color.Black, background: Color.White),
            }, exceptionSettings);

        public static ConsoleTheme Colored(ExceptionSettings? exceptionSettings) => new ConsoleTheme(
            new Dictionary<ConsoleThemeStyle, Style>
            {
                [ConsoleThemeStyle.Text] = new Style(foreground: Color.Silver),
                [ConsoleThemeStyle.SecondaryText] = new Style(foreground: Color.Grey),
                [ConsoleThemeStyle.TertiaryText] = new Style(foreground: Color.Grey),
                [ConsoleThemeStyle.Invalid] = new Style(foreground: Color.Yellow),
                [ConsoleThemeStyle.Null] = new Style(foreground: Color.White),
                [ConsoleThemeStyle.Name] = new Style(foreground: Color.White),
                [ConsoleThemeStyle.String] = new Style(foreground: Color.White),
                [ConsoleThemeStyle.Number] = new Style(foreground: Color.White),
                [ConsoleThemeStyle.Boolean] = new Style(foreground: Color.White),
                [ConsoleThemeStyle.Scalar] = new Style(foreground: Color.White),
                [ConsoleThemeStyle.LevelVerbose] = new Style(foreground: Color.Silver, background: Color.Grey),
                [ConsoleThemeStyle.LevelDebug] = new Style(foreground: Color.White, background: Color.Grey),
                [ConsoleThemeStyle.LevelInformation] = new Style(foreground: Color.White, background: Color.Blue),
                [ConsoleThemeStyle.LevelWarning] = new Style(foreground: Color.Grey, background: Color.Yellow),
                [ConsoleThemeStyle.LevelError] = new Style(foreground: Color.White, background: Color.Red),
                [ConsoleThemeStyle.LevelFatal] = new Style(foreground: Color.White, background: Color.Red),
            }, exceptionSettings);
    }
}