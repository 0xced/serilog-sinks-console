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

using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.SpectreConsole.Output;
using Spectre.Console;

namespace Serilog.Sinks.SpectreConsole
{
    class SpectreConsoleSink : ILogEventSink
    {
        private readonly LogEventLevel? _standardErrorFromLevel;
        private readonly OutputTemplateRenderer _templateRenderer;
        private readonly IAnsiConsole _outConsole;
        private readonly IAnsiConsole _errorConsole;
        private readonly object _syncRoot;

        public SpectreConsoleSink(
            OutputTemplateRenderer templateRenderer,
            LogEventLevel? standardErrorFromLevel,
            IAnsiConsole outConsole,
            IAnsiConsole errorConsole,
            object syncRoot)
        {
            _standardErrorFromLevel = standardErrorFromLevel;
            _templateRenderer = templateRenderer;
            _outConsole = outConsole;
            _errorConsole = errorConsole;
            _syncRoot = syncRoot;
        }

        public void Emit(LogEvent logEvent)
        {
            var console = SelectConsole(logEvent.Level);
            lock (_syncRoot)
            {
                _templateRenderer.Render(logEvent, console);
            }
        }

        private IAnsiConsole SelectConsole(LogEventLevel logEventLevel)
        {
            if (_standardErrorFromLevel is null)
                return _outConsole;

            return logEventLevel < _standardErrorFromLevel ? _outConsole : _errorConsole;
        }
    }
}
