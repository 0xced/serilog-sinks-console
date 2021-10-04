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

using System.Collections.Generic;
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.SpectreConsole.Themes;
using Spectre.Console;
using Padding = Serilog.Sinks.SpectreConsole.Rendering.Padding;

namespace Serilog.Sinks.SpectreConsole.Output
{
    class LevelTokenRenderer : OutputTemplateTokenRenderer
    {
        readonly ConsoleTheme _theme;
        readonly PropertyToken _levelToken;

        static readonly Dictionary<LogEventLevel, ConsoleThemeStyle> Levels = new Dictionary<LogEventLevel, ConsoleThemeStyle>
        {
            { LogEventLevel.Verbose, ConsoleThemeStyle.LevelVerbose },
            { LogEventLevel.Debug, ConsoleThemeStyle.LevelDebug },
            { LogEventLevel.Information, ConsoleThemeStyle.LevelInformation },
            { LogEventLevel.Warning, ConsoleThemeStyle.LevelWarning },
            { LogEventLevel.Error, ConsoleThemeStyle.LevelError },
            { LogEventLevel.Fatal, ConsoleThemeStyle.LevelFatal },
        };

        public LevelTokenRenderer(ConsoleTheme theme, PropertyToken levelToken)
        {
            _theme = theme;
            _levelToken = levelToken;
        }

        public override void Render(LogEvent logEvent, IAnsiConsole console)
        {
            var moniker = LevelOutputFormat.GetLevelMoniker(logEvent.Level, _levelToken.Format);
            var levelStyle = Levels.TryGetValue(logEvent.Level, out var themeStyle) ? themeStyle : ConsoleThemeStyle.Invalid;
            Padding.Apply(console, moniker, _theme.GetStyle(levelStyle), _levelToken.Alignment);
        }
    }
}