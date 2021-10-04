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
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.SpectreConsole.Formatting;
using Serilog.Sinks.SpectreConsole.Rendering;
using Serilog.Sinks.SpectreConsole.Themes;
using Spectre.Console;

namespace Serilog.Sinks.SpectreConsole.Output
{
    class MessageTemplateOutputTokenRenderer : OutputTemplateTokenRenderer
    {
        readonly ThemedMessageTemplateRenderer _renderer;

        public MessageTemplateOutputTokenRenderer(ConsoleTheme theme, PropertyToken token, IFormatProvider? formatProvider)
        {
            bool isLiteral = false, isJson = false;

            if (token.Format != null)
            {
                foreach (var format in token.Format)
                {
                    if (format == 'l')
                        isLiteral = true;
                    else if (format == 'j')
                        isJson = true;
                }
            }

            var valueFormatter = isJson
                ? (ThemedValueFormatter)new ThemedJsonValueFormatter(theme, formatProvider)
                : new ThemedDisplayValueFormatter(theme, formatProvider);

            _renderer = new ThemedMessageTemplateRenderer(theme, valueFormatter, isLiteral);
        }

        public override void Render(LogEvent logEvent, IAnsiConsole console)
        {
            _renderer.Render(logEvent.MessageTemplate, logEvent.Properties, console);
        }
    }
}