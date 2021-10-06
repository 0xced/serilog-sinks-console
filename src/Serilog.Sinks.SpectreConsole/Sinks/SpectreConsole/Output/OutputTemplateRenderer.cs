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
using Serilog.Formatting.Display;
using Serilog.Parsing;
using Serilog.Sinks.SpectreConsole.Themes;
using Spectre.Console;

namespace Serilog.Sinks.SpectreConsole.Output
{
    class OutputTemplateRenderer
    {
        readonly OutputTemplateTokenRenderer[] _renderers;

        public OutputTemplateRenderer(ConsoleTheme theme, string outputTemplate, IFormatProvider? formatProvider)
        {
            if (outputTemplate is null) throw new ArgumentNullException(nameof(outputTemplate));
            var template = new MessageTemplateParser().Parse(outputTemplate);

            var renderers = new List<OutputTemplateTokenRenderer>();
            foreach (var token in template.Tokens)
            {
                if (token is TextToken textToken)
                {
                    renderers.Add(new TextTokenRenderer(theme, textToken.Text));
                }
                else
                {
                    var propertyToken = (PropertyToken)token;
                    OutputTemplateTokenRenderer renderer = propertyToken.PropertyName switch
                    {
                        OutputProperties.LevelPropertyName => new LevelTokenRenderer(theme, propertyToken),
                        OutputProperties.NewLinePropertyName => new NewLineTokenRenderer(propertyToken.Alignment),
                        OutputProperties.ExceptionPropertyName => new ExceptionTokenRenderer(theme, propertyToken),
                        OutputProperties.MessagePropertyName => new MessageTemplateOutputTokenRenderer(theme, propertyToken, formatProvider),
                        OutputProperties.TimestampPropertyName => new TimestampTokenRenderer(theme, propertyToken, formatProvider),
                        OutputProperties.PropertiesPropertyName => new PropertiesTokenRenderer(theme, propertyToken, template, formatProvider),
                        _ => new EventPropertyTokenRenderer(theme, propertyToken, formatProvider)
                    };
                    renderers.Add(renderer);
                }
            }

            _renderers = renderers.ToArray();
        }

        public void Render(LogEvent logEvent, IAnsiConsole console)
        {
            if (logEvent is null) throw new ArgumentNullException(nameof(logEvent));
            if (console is null) throw new ArgumentNullException(nameof(console));

            foreach (var renderer in _renderers)
                renderer.Render(logEvent, console);
        }
    }
}
