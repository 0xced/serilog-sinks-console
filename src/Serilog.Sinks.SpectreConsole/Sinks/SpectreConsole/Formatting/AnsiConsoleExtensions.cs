using System;
using System.IO;
using Spectre.Console;

namespace Serilog.Sinks.SpectreConsole.Formatting
{
    static class AnsiConsoleExtensions
    {
        public static void Write(this IAnsiConsole console, Action<TextWriter> write, Style style)
        {
            var buffer = new StringWriter();
            write(buffer);
            console.Write(buffer.ToString(), style);
        }
    }
}