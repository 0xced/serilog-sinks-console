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
using Serilog.Sinks.SpectreConsole.Rendering;
using Serilog.Sinks.SpectreConsole.Themes;
using Spectre.Console;
using Padding = Serilog.Sinks.SpectreConsole.Rendering.Padding;

namespace Serilog.Sinks.SpectreConsole.Output
{
    class EventPropertyTokenRenderer : OutputTemplateTokenRenderer
    {
        readonly ConsoleTheme _theme;
        readonly PropertyToken _token;
        readonly IFormatProvider? _formatProvider;

        public EventPropertyTokenRenderer(ConsoleTheme theme, PropertyToken token, IFormatProvider? formatProvider)
        {
            _theme = theme;
            _token = token;
            _formatProvider = formatProvider;
        }

        public override void Render(LogEvent logEvent, IAnsiConsole console)
        {
            // If a property is missing, don't render anything (message templates render the raw token here).
            if (!logEvent.Properties.TryGetValue(_token.PropertyName, out var propertyValue))
            {
                Padding.Apply(console, string.Empty, Style.Plain, _token.Alignment);
                return;
            }

            var buffer = new StringWriter();

            // If the value is a scalar string, support some additional formats: 'u' for uppercase
            // and 'w' for lowercase.
            if (propertyValue is ScalarValue { Value: string literalString })
            {
                var cased = Casing.Format(literalString, _token.Format);
                buffer.Write(cased);
            }
            else
            {
                propertyValue.Render(buffer, _token.Format, _formatProvider);
            }

            var style = _theme.GetStyle(ConsoleThemeStyle.SecondaryText);
            Padding.Apply(console, buffer.ToString(), style, _token.Alignment);
        }
    }
}