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
using System.Globalization;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.SpectreConsole.Themes;
using Spectre.Console;

namespace Serilog.Sinks.SpectreConsole.Formatting
{
    class ThemedJsonValueFormatter : ThemedValueFormatter
    {
        readonly ThemedDisplayValueFormatter _displayFormatter;
        readonly IFormatProvider? _formatProvider;

        public ThemedJsonValueFormatter(ConsoleTheme theme, IFormatProvider? formatProvider)
            : base(theme)
        {
            _displayFormatter = new ThemedDisplayValueFormatter(theme, formatProvider);
            _formatProvider = formatProvider;
        }

        public override ThemedValueFormatter SwitchTheme(ConsoleTheme theme)
        {
            return new ThemedJsonValueFormatter(theme, _formatProvider);
        }

        protected override int VisitScalarValue(ThemedValueFormatterState state, ScalarValue scalar)
        {
            if (scalar is null)
                throw new ArgumentNullException(nameof(scalar));

            // At the top level, for scalar values, use "display" rendering.
            if (state.IsTopLevel)
                return _displayFormatter.FormatLiteralValue(scalar, state.Console, state.Format);

            return FormatLiteralValue(scalar, state.Console);
        }

        protected override int VisitSequenceValue(ThemedValueFormatterState state, SequenceValue sequence)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            var count = 0;

            var tertiaryTextStyle = Theme.GetStyle(ConsoleThemeStyle.TertiaryText);

            state.Console.Write("[", tertiaryTextStyle);

            var delim = string.Empty;
            foreach (var element in sequence.Elements)
            {
                if (delim.Length != 0)
                {
                    state.Console.Write(delim, tertiaryTextStyle);
                }

                delim = ", ";
                Visit(state.Nest(), element);
            }

            state.Console.Write("]", tertiaryTextStyle);

            return count;
        }

        protected override int VisitStructureValue(ThemedValueFormatterState state, StructureValue structure)
        {
            var count = 0;

            var tertiaryTextStyle = Theme.GetStyle(ConsoleThemeStyle.TertiaryText);

            state.Console.Write("{", tertiaryTextStyle);

            var delim = string.Empty;
            foreach (var property in structure.Properties)
            {
                if (delim.Length != 0)
                {
                    state.Console.Write(delim, tertiaryTextStyle);
                }

                delim = ", ";

                var nameStyle = Theme.GetStyle(ConsoleThemeStyle.Name);
                state.Console.Write(buffer => JsonValueFormatter.WriteQuotedJsonString(property.Name, buffer), nameStyle);

                state.Console.Write(": ", tertiaryTextStyle);

                count += Visit(state.Nest(), property.Value);
            }


            if (structure.TypeTag != null)
            {
                var nameStyle = Theme.GetStyle(ConsoleThemeStyle.Name);
                var stringStyle = Theme.GetStyle(ConsoleThemeStyle.String);

                state.Console.Write(delim, tertiaryTextStyle);

                state.Console.Write(buffer => JsonValueFormatter.WriteQuotedJsonString("$type", buffer), nameStyle);

                state.Console.Write(": ", tertiaryTextStyle);

                state.Console.Write(buffer => JsonValueFormatter.WriteQuotedJsonString(structure.TypeTag, buffer), stringStyle);
            }

            state.Console.Write("}", tertiaryTextStyle);

            return count;
        }

        protected override int VisitDictionaryValue(ThemedValueFormatterState state, DictionaryValue dictionary)
        {
            var count = 0;

            var tertiaryTextStyle = Theme.GetStyle(ConsoleThemeStyle.TertiaryText);

            state.Console.Write("{", tertiaryTextStyle);

            var delim = string.Empty;
            foreach (var element in dictionary.Elements)
            {
                if (delim.Length != 0)
                {
                    state.Console.Write(delim, tertiaryTextStyle);
                }

                delim = ", ";

                var themeStyle = element.Key.Value == null
                    ? ConsoleThemeStyle.Null
                    : element.Key.Value is string
                        ? ConsoleThemeStyle.String
                        : ConsoleThemeStyle.Scalar;

                var style = Theme.GetStyle(themeStyle);
                state.Console.Write(buffer => JsonValueFormatter.WriteQuotedJsonString((element.Key.Value ?? "null").ToString() ?? "", buffer), style);

                state.Console.Write(": ", tertiaryTextStyle);

                count += Visit(state.Nest(), element.Value);
            }

            state.Console.Write("}", tertiaryTextStyle);

            return count;
        }

        int FormatLiteralValue(ScalarValue scalar, IAnsiConsole console)
        {
            var value = scalar.Value;
            var count = 0;

            if (value is null)
            {
                var nullStyle = Theme.GetStyle(ConsoleThemeStyle.Null);
                console.Write("null", nullStyle);
                return count;
            }

            if (value is string str)
            {
                var stringStyle = Theme.GetStyle(ConsoleThemeStyle.String);
                console.Write(buffer => JsonValueFormatter.WriteQuotedJsonString(str, buffer), stringStyle);
                return count;
            }

            if (value is ValueType)
            {
                var numberStyle = Theme.GetStyle(ConsoleThemeStyle.Number);

                if (value is int || value is uint || value is long || value is ulong || value is decimal || value is byte || value is sbyte || value is short || value is ushort)
                {
                    console.Write(((IFormattable)value).ToString(null, CultureInfo.InvariantCulture), numberStyle);
                    return count;
                }

                if (value is double d)
                {
                    if (double.IsNaN(d) || double.IsInfinity(d))
                        console.Write(buffer => JsonValueFormatter.WriteQuotedJsonString(d.ToString(CultureInfo.InvariantCulture), buffer), numberStyle);
                    else
                        console.Write(d.ToString("R", CultureInfo.InvariantCulture), numberStyle);
                    return count;
                }

                if (value is float f)
                {
                    if (double.IsNaN(f) || double.IsInfinity(f))
                        console.Write(buffer => JsonValueFormatter.WriteQuotedJsonString(f.ToString(CultureInfo.InvariantCulture), buffer), numberStyle);
                    else
                        console.Write(f.ToString("R", CultureInfo.InvariantCulture), numberStyle);
                    return count;
                }

                if (value is bool b)
                {
                    var booleanStyle = Theme.GetStyle(ConsoleThemeStyle.Boolean);
                    console.Write(b ? "true" : "false", booleanStyle);
                    return count;
                }

                var scalarStyle = Theme.GetStyle(ConsoleThemeStyle.Scalar);

                if (value is char ch)
                {
                    console.Write(buffer => JsonValueFormatter.WriteQuotedJsonString(ch.ToString(), buffer), scalarStyle);
                    return count;
                }

                if (value is DateTime || value is DateTimeOffset)
                {
                    console.Write($"\"{((IFormattable)value).ToString("O", CultureInfo.InvariantCulture)}\"", scalarStyle);
                    return count;
                }
            }

            var style = Theme.GetStyle(ConsoleThemeStyle.Scalar);
            console.Write(buffer => JsonValueFormatter.WriteQuotedJsonString(value.ToString() ?? "", buffer), style);
            return count;
        }
    }
}