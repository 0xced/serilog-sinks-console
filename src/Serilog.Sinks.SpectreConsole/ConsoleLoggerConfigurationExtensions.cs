// Copyright 2017 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.SpectreConsole;
using Serilog.Sinks.SpectreConsole.Output;
using Serilog.Sinks.SpectreConsole.Themes;
using System;
using Spectre.Console;

namespace Serilog
{
    /// <summary>
    /// Adds the WriteTo.Console() extension method to <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class ConsoleLoggerConfigurationExtensions
    {
        private static readonly object DefaultSyncRoot = new object();
        const string DefaultConsoleOutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

        /// <summary>
        /// Writes log events to <see cref="IAnsiConsole"/>.
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="outConsole"></param>
        /// <param name="errorConsole"></param>
        /// <param name="exceptionSettings"></param>
        /// <param name="restrictedToMinimumLevel">The minimum level for
        /// events passed through the sink. Ignored when <paramref name="levelSwitch"/> is specified.</param>
        /// <param name="outputTemplate">A message template describing the format used to write to the sink.
        /// The default is <code>"[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"</code>.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="levelSwitch">A switch allowing the pass-through minimum level
        /// to be changed at runtime.</param>
        /// <param name="standardErrorFromLevel">Specifies the level at which events will be written to standard error.</param>
        /// <param name="theme">The theme to apply to the styled output. If not specified,
        /// uses <see cref="ConsoleTheme.Literate(ExceptionSettings)"/> with the specified <paramref name="exceptionSettings"/>.</param>
        /// <param name="applyThemeToRedirectedOutput">Applies the selected or default theme even when output redirection is detected.</param>
        /// <param name="syncRoot">An object that will be used to `lock` (sync) access to the console output. If you specify this, you
        /// will have the ability to lock on this object, and guarantee that the sink will not be about to output anything while
        /// the lock is held.</param>
        /// <returns>Configuration object allowing method chaining.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="sinkConfiguration"/> is <code>null</code></exception>
        /// <exception cref="ArgumentNullException">When <paramref name="outputTemplate"/> is <code>null</code></exception>
        public static LoggerConfiguration SpectreConsole(
            this LoggerSinkConfiguration sinkConfiguration,
            IAnsiConsole? outConsole = null,
            IAnsiConsole? errorConsole = null,
            ExceptionSettings? exceptionSettings = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            string outputTemplate = DefaultConsoleOutputTemplate,
            IFormatProvider? formatProvider = null,
            LoggingLevelSwitch? levelSwitch = null,
            LogEventLevel? standardErrorFromLevel = null,
            ConsoleTheme? theme = null,
            bool applyThemeToRedirectedOutput = false,
            object? syncRoot = null)
        {
            if (sinkConfiguration is null) throw new ArgumentNullException(nameof(sinkConfiguration));
            if (outputTemplate is null) throw new ArgumentNullException(nameof(outputTemplate));

            var appliedTheme = !applyThemeToRedirectedOutput && (System.Console.IsOutputRedirected || System.Console.IsErrorRedirected) ?
                ConsoleTheme.None :
                theme ?? ConsoleThemes.Literate(exceptionSettings);

            syncRoot ??= DefaultSyncRoot;

            var formatter = new OutputTemplateRenderer(appliedTheme, outputTemplate, formatProvider);
            var consoleFactory = new AnsiConsoleFactory();
            outConsole ??= consoleFactory.Create(new AnsiConsoleSettings { Out = new AnsiConsoleOutput(Console.Out) });
            errorConsole ??= consoleFactory.Create(new AnsiConsoleSettings { Out = new AnsiConsoleOutput(Console.Error) });
            return sinkConfiguration.Sink(new SpectreConsoleSink(formatter, standardErrorFromLevel, outConsole, errorConsole, syncRoot), restrictedToMinimumLevel, levelSwitch);
        }
    }
}
