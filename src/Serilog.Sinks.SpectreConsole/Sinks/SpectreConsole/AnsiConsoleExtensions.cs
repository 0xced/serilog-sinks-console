using System;
using System.IO;
using Serilog.Sinks.SpectreConsole.Themes;
using Spectre.Console;

namespace Serilog.Sinks.SpectreConsole
{
    static class AnsiConsoleExtensions
    {
        public static void Write(this IAnsiConsole console, Action<TextWriter> write, Style style)
        {
            var buffer = new StringWriter();
            write(buffer);
            console.Write(buffer.ToString(), style);
        }

        public static void WriteText(this IAnsiConsole console, string text, ConsoleTheme theme)
            => console.Write(text, theme.GetStyle(ConsoleThemeStyle.Text));

        public static void WriteSecondaryText(this IAnsiConsole console, string text, ConsoleTheme theme)
            => console.Write(text, theme.GetStyle(ConsoleThemeStyle.SecondaryText));

        public static void WriteTertiaryText(this IAnsiConsole console, string text, ConsoleTheme theme)
            => console.Write(text, theme.GetStyle(ConsoleThemeStyle.TertiaryText));

        public static void WriteInvalid(this IAnsiConsole console, string text, ConsoleTheme theme)
            => console.Write(text, theme.GetStyle(ConsoleThemeStyle.Invalid));

        public static void WriteNull(this IAnsiConsole console, string text, ConsoleTheme theme)
            => console.Write(text, theme.GetStyle(ConsoleThemeStyle.Null));

        public static void WriteName(this IAnsiConsole console, string text, ConsoleTheme theme)
            => console.Write(text, theme.GetStyle(ConsoleThemeStyle.Name));

        public static void WriteName(this IAnsiConsole console, Action<TextWriter> write, ConsoleTheme theme)
            => console.Write(write, theme.GetStyle(ConsoleThemeStyle.Name));

        public static void WriteString(this IAnsiConsole console, string text, ConsoleTheme theme)
            => console.Write(text, theme.GetStyle(ConsoleThemeStyle.String));

        public static void WriteString(this IAnsiConsole console, Action<TextWriter> write, ConsoleTheme theme)
            => console.Write(write, theme.GetStyle(ConsoleThemeStyle.String));

        public static void WriteNumber(this IAnsiConsole console, string text, ConsoleTheme theme)
            => console.Write(text, theme.GetStyle(ConsoleThemeStyle.Number));

        public static void WriteNumber(this IAnsiConsole console, Action<TextWriter> write, ConsoleTheme theme)
            => console.Write(write, theme.GetStyle(ConsoleThemeStyle.Number));

        public static void WriteBoolean(this IAnsiConsole console, string text, ConsoleTheme theme)
            => console.Write(text, theme.GetStyle(ConsoleThemeStyle.Boolean));

        public static void WriteScalar(this IAnsiConsole console, string text, ConsoleTheme theme)
            => console.Write(text, theme.GetStyle(ConsoleThemeStyle.Scalar));

        public static void WriteScalar(this IAnsiConsole console, Action<TextWriter> write, ConsoleTheme theme)
            => console.Write(write, theme.GetStyle(ConsoleThemeStyle.Scalar));
    }
}