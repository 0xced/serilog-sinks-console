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
using System.Collections.Generic;
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.SpectreConsole.Formatting;
using Serilog.Sinks.SpectreConsole.Themes;
using Spectre.Console;

namespace Serilog.Sinks.SpectreConsole.Rendering
{
    class ThemedMessageTemplateRenderer
    {
        readonly ConsoleTheme _theme;
        readonly ThemedValueFormatter _valueFormatter;
        readonly bool _isLiteral;

        public ThemedMessageTemplateRenderer(ConsoleTheme theme, ThemedValueFormatter valueFormatter, bool isLiteral)
        {
            _theme = theme ?? throw new ArgumentNullException(nameof(theme));
            _valueFormatter = valueFormatter;
            _isLiteral = isLiteral;
        }

        public void Render(MessageTemplate template, IReadOnlyDictionary<string, LogEventPropertyValue> properties, IAnsiConsole console)
        {
            foreach (var token in template.Tokens)
            {
                if (token is TextToken textToken)
                {
                    RenderTextToken(textToken, console);
                }
                else
                {
                    RenderPropertyToken((PropertyToken)token, properties, console);
                }
            }
        }

        void RenderTextToken(TextToken tt, IAnsiConsole console)
        {
            console.WriteText(tt.Text, _theme);
        }

        void RenderPropertyToken(PropertyToken pt, IReadOnlyDictionary<string, LogEventPropertyValue> properties, IAnsiConsole console)
        {
            if (!properties.TryGetValue(pt.PropertyName, out var propertyValue))
            {
                console.WriteInvalid(pt.ToString(), _theme);
                return;
            }

            if (!pt.Alignment.HasValue)
            {
                RenderValue(_theme, _valueFormatter, propertyValue, console, pt.Format);
            }
            else
            {
                RenderAlignedPropertyTokenUnbuffered(pt, console, propertyValue);
            }
        }

        void RenderAlignedPropertyTokenUnbuffered(PropertyToken pt, IAnsiConsole console, LogEventPropertyValue propertyValue)
        {
            if (pt.Alignment == null) throw new ArgumentException("The PropertyToken should have a non-null Alignment.", nameof(pt));

            var measuringConsole = new MeasuringConsole();
            RenderValue(_theme, _valueFormatter, propertyValue, measuringConsole, pt.Format);

            var valueLength = measuringConsole.Output.Length;
            if (valueLength >= pt.Alignment.Value.Width)
            {
                RenderValue(_theme, _valueFormatter, propertyValue, console, pt.Format);
                return;
            }

            if (pt.Alignment.Value.Direction == AlignmentDirection.Left)
            {
                RenderValue(_theme, _valueFormatter, propertyValue, console, pt.Format);
                Padding.Apply(console, string.Empty, Style.Plain, pt.Alignment.Value.Widen(-valueLength));
            }
            else
            {
                Padding.Apply(console, string.Empty, Style.Plain, pt.Alignment.Value.Widen(-valueLength));
                RenderValue(_theme, _valueFormatter, propertyValue, console, pt.Format);
            }

        }

        void RenderValue(ConsoleTheme theme, ThemedValueFormatter valueFormatter, LogEventPropertyValue propertyValue, IAnsiConsole console, string? format)
        {
            if (_isLiteral && propertyValue is ScalarValue { Value: string text })
            {
                console.WriteString(text, theme);
            }
            else
            {
                valueFormatter.Format(propertyValue, console, format, _isLiteral);
            }
        }
    }
}
