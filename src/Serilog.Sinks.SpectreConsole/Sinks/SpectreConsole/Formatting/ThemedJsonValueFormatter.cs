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

        public ThemedJsonValueFormatter(ConsoleTheme theme, IFormatProvider? formatProvider)
            : base(theme)
        {
            _displayFormatter = new ThemedDisplayValueFormatter(theme, formatProvider);
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

            state.Console.WriteTertiaryText("[", Theme);

            var delim = string.Empty;
            foreach (var element in sequence.Elements)
            {
                if (delim.Length != 0)
                {
                    state.Console.WriteTertiaryText(delim, Theme);
                }

                delim = ", ";
                Visit(state.Nest(), element);
            }

            state.Console.WriteTertiaryText("]", Theme);

            return 0;
        }

        protected override int VisitStructureValue(ThemedValueFormatterState state, StructureValue structure)
        {
            state.Console.WriteTertiaryText("{", Theme);

            var delim = string.Empty;
            foreach (var property in structure.Properties)
            {
                if (delim.Length != 0)
                {
                    state.Console.WriteTertiaryText(delim, Theme);
                }

                delim = ", ";

                state.Console.WriteName(buffer => JsonValueFormatter.WriteQuotedJsonString(property.Name, buffer), Theme);

                state.Console.WriteTertiaryText(": ", Theme);

                Visit(state.Nest(), property.Value);
            }


            if (structure.TypeTag != null)
            {
                state.Console.WriteTertiaryText(delim, Theme);

                state.Console.WriteName(buffer => JsonValueFormatter.WriteQuotedJsonString("$type", buffer), Theme);

                state.Console.WriteTertiaryText(": ", Theme);

                state.Console.WriteString(buffer => JsonValueFormatter.WriteQuotedJsonString(structure.TypeTag, buffer), Theme);
            }

            state.Console.WriteTertiaryText("}", Theme);

            return 0;
        }

        protected override int VisitDictionaryValue(ThemedValueFormatterState state, DictionaryValue dictionary)
        {
            state.Console.WriteTertiaryText("{", Theme);

            var delim = string.Empty;
            foreach (var element in dictionary.Elements)
            {
                if (delim.Length != 0)
                {
                    state.Console.WriteTertiaryText(delim, Theme);
                }

                delim = ", ";

                var themeStyle = element.Key.Value == null
                    ? ConsoleThemeStyle.Null
                    : element.Key.Value is string
                        ? ConsoleThemeStyle.String
                        : ConsoleThemeStyle.Scalar;

                var style = Theme.GetStyle(themeStyle);
                state.Console.Write(buffer => JsonValueFormatter.WriteQuotedJsonString((element.Key.Value ?? "null").ToString() ?? "", buffer), style);

                state.Console.WriteTertiaryText(": ", Theme);

                Visit(state.Nest(), element.Value);
            }

            state.Console.WriteTertiaryText("}", Theme);

            return 0;
        }

        int FormatLiteralValue(ScalarValue scalar, IAnsiConsole console)
        {
            var value = scalar.Value;

            if (value is null)
            {
                console.WriteNull("null", Theme);
                return 0;
            }

            if (value is string str)
            {
                console.WriteString(buffer => JsonValueFormatter.WriteQuotedJsonString(str, buffer), Theme);
                return 0;
            }

            if (value is ValueType)
            {
                if (value is int || value is uint || value is long || value is ulong || value is decimal || value is byte || value is sbyte || value is short || value is ushort)
                {
                    console.WriteNumber(((IFormattable)value).ToString(null, CultureInfo.InvariantCulture), Theme);
                    return 0;
                }

                if (value is double d)
                {
                    if (double.IsNaN(d) || double.IsInfinity(d))
                        console.WriteNumber(buffer => JsonValueFormatter.WriteQuotedJsonString(d.ToString(CultureInfo.InvariantCulture), buffer), Theme);
                    else
                        console.WriteNumber(d.ToString("R", CultureInfo.InvariantCulture), Theme);
                    return 0;
                }

                if (value is float f)
                {
                    if (double.IsNaN(f) || double.IsInfinity(f))
                        console.WriteNumber(buffer => JsonValueFormatter.WriteQuotedJsonString(f.ToString(CultureInfo.InvariantCulture), buffer), Theme);
                    else
                        console.WriteNumber(f.ToString("R", CultureInfo.InvariantCulture), Theme);
                    return 0;
                }

                if (value is bool b)
                {
                    console.WriteBoolean(b ? "true" : "false", Theme);
                    return 0;
                }

                if (value is char ch)
                {
                    console.WriteScalar(buffer => JsonValueFormatter.WriteQuotedJsonString(ch.ToString(), buffer), Theme);
                    return 0;
                }

                if (value is DateTime || value is DateTimeOffset)
                {
                    console.WriteScalar($"\"{((IFormattable)value).ToString("O", CultureInfo.InvariantCulture)}\"", Theme);
                    return 0;
                }
            }

            console.WriteScalar(buffer => JsonValueFormatter.WriteQuotedJsonString(value.ToString() ?? "", buffer), Theme);
            return 0;
        }
    }
}