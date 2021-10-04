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

using System;
using System.IO;
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.SpectreConsole.Themes;
using Spectre.Console;
using Padding = Serilog.Sinks.SpectreConsole.Rendering.Padding;

namespace Serilog.Sinks.SpectreConsole.Output
{
    class TimestampTokenRenderer : OutputTemplateTokenRenderer
    {
        readonly ConsoleTheme _theme;
        readonly PropertyToken _token;
        readonly IFormatProvider? _formatProvider;

        public TimestampTokenRenderer(ConsoleTheme theme, PropertyToken token, IFormatProvider? formatProvider)
        {
            _theme = theme;
            _token = token;
            _formatProvider = formatProvider;
        }

        public override void Render(LogEvent logEvent, IAnsiConsole console)
        {
            var timestamp = new ScalarValue(logEvent.Timestamp);
            var buffer = new StringWriter();
            timestamp.Render(buffer, _token.Format, _formatProvider);
            Padding.Apply(console, buffer.ToString(), _theme.GetStyle(ConsoleThemeStyle.SecondaryText), _token.Alignment);
        }
    }
}
