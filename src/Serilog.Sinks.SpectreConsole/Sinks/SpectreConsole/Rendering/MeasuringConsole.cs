using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Serilog.Sinks.SpectreConsole.Rendering
{
    /// <summary>
    /// A console for measuring output, adapted from Spectre.Console.Testing.TestConsole
    /// </summary>
    class MeasuringConsole : IAnsiConsole, IDisposable
    {
        private readonly IAnsiConsole _console;
        private readonly StringWriter _writer;
        private readonly IAnsiConsoleCursor? _cursor;

        /// <inheritdoc/>
        public Profile Profile => _console.Profile;

        /// <inheritdoc/>
        public IExclusivityMode ExclusivityMode => _console.ExclusivityMode;


        /// <inheritdoc/>
        public RenderPipeline Pipeline => _console.Pipeline;

        /// <inheritdoc/>
        public IAnsiConsoleCursor Cursor => _cursor ?? _console.Cursor;

        /// <inheritdoc/>
        IAnsiConsoleInput IAnsiConsole.Input => new NoopAnsiConsoleInput();

        /// <summary>
        /// Gets the console output.
        /// </summary>
        public string Output => _writer.ToString();

        /// <summary>
        /// Initializes a new instance of the <see cref="MeasuringConsole"/> class.
        /// </summary>
        public MeasuringConsole()
        {
            _writer = new StringWriter();
            _cursor = new NoopCursor();

            var factory = new AnsiConsoleFactory();
            _console = factory.Create(new AnsiConsoleSettings
            {
                Ansi = AnsiSupport.Yes,
                ColorSystem = ColorSystemSupport.TrueColor,
                Out = new AnsiConsoleOutput(_writer),
                Interactive = InteractionSupport.No,
                ExclusivityMode = new NoopExclusivityMode(),
                Enrichment = new ProfileEnrichment { UseDefaultEnrichers = false },
            });

            _console.Profile.Width = 80;
            _console.Profile.Height = 24;
            _console.Profile.Capabilities.Ansi = true;
            _console.Profile.Capabilities.Unicode = true;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _writer.Dispose();
        }

        /// <inheritdoc/>
        public void Clear(bool home)
        {
            _console.Clear(home);
        }

        /// <inheritdoc/>
        public void Write(IRenderable renderable)
        {
            foreach (var segment in renderable.GetSegments(this))
            {
                if (segment.IsControlCode)
                {
                    continue;
                }

                Profile.Out.Writer.Write(segment.Text);
            }
        }

        private class NoopAnsiConsoleInput : IAnsiConsoleInput
        {
            public ConsoleKeyInfo? ReadKey(bool intercept) => null;

            public Task<ConsoleKeyInfo?> ReadKeyAsync(bool intercept, CancellationToken cancellationToken) => Task.FromResult<ConsoleKeyInfo?>(null);
        }

        private class NoopCursor : IAnsiConsoleCursor
        {
            public void Show(bool show) {}

            public void SetPosition(int column, int line){}

            public void Move(CursorDirection direction, int steps) {}
        }

        private class NoopExclusivityMode : IExclusivityMode
        {
            public T Run<T>(Func<T> func) => func();

            public Task<T> Run<T>(Func<Task<T>> func) => func();
        }
    }
}
