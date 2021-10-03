﻿// Copyright 2017 Serilog Contributors
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

using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.IO;
using System.Text;

namespace Serilog.Sinks.SystemConsole
{
    class SpectreConsoleSink : ILogEventSink
    {
        readonly LogEventLevel? _standardErrorFromLevel;
        readonly ConsoleTheme _theme;
        readonly ITextFormatter _formatter;

        const int DefaultWriteBufferCapacity = 256;

        public SpectreConsoleSink(
            ConsoleTheme theme,
            ITextFormatter formatter,
            LogEventLevel? standardErrorFromLevel)
        {
            _standardErrorFromLevel = standardErrorFromLevel;
            _theme = theme ?? throw new ArgumentNullException(nameof(theme));
            _formatter = formatter;
        }

        public void Emit(LogEvent logEvent)
        {
            var output = SelectOutputStream(logEvent.Level);

            // ANSI escape codes can be pre-rendered into a buffer; however, if we're on Windows and
            // using its console coloring APIs, the color switches would happen during the off-screen
            // buffered write here and have no effect when the line is actually written out.
            if (_theme.CanBuffer)
            {
                var buffer = new StringWriter(new StringBuilder(DefaultWriteBufferCapacity));
                _formatter.Format(logEvent, buffer);
                var formattedLogEventText = buffer.ToString();
                output.Write(formattedLogEventText);
                output.Flush();
            }
            else
            {
                _formatter.Format(logEvent, output);
                output.Flush();
            }
        }

        TextWriter SelectOutputStream(LogEventLevel logEventLevel)
        {
            if (_standardErrorFromLevel is null)
                return Console.Out;

            return logEventLevel < _standardErrorFromLevel ? Console.Out : Console.Error;
        }
    }
}
