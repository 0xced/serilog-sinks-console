using Serilog.Events;
using Serilog.Sinks.SpectreConsole.Formatting;
using Serilog.Sinks.SpectreConsole.Themes;
using Spectre.Console.Testing;
using Xunit;

namespace Serilog.Sinks.SpectreConsole.Tests.Formatting
{
    public class ThemedDisplayValueFormatterTests
    {
        [Theory]
        [InlineData("Hello", null, "\"Hello\"")]
        [InlineData("Hello", "l", "Hello")]
        public void StringFormattingIsApplied(string value, string format, string expected)
        {
            var formatter = new ThemedDisplayValueFormatter(ConsoleTheme.None, null);
            var console = new TestConsole();
            formatter.FormatLiteralValue(new ScalarValue(value), console, format);
            var actual = console.Output;
            Assert.Equal(expected, actual);
        }
    }
}
