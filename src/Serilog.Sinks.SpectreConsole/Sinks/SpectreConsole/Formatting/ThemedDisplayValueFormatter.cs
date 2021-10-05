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
using Serilog.Formatting.Json;
using Serilog.Sinks.SpectreConsole.Themes;
using Spectre.Console;

namespace Serilog.Sinks.SpectreConsole.Formatting
{
    class ThemedDisplayValueFormatter : ThemedValueFormatter
    {
        readonly IFormatProvider? _formatProvider;

        public ThemedDisplayValueFormatter(ConsoleTheme theme, IFormatProvider? formatProvider)
            : base(theme)
        {
            _formatProvider = formatProvider;
        }

        protected override int VisitScalarValue(ThemedValueFormatterState state, ScalarValue scalar)
        {
            if (scalar is null)
                throw new ArgumentNullException(nameof(scalar));
            return FormatLiteralValue(scalar, state.Console, state.Format);
        }

        protected override int VisitSequenceValue(ThemedValueFormatterState state, SequenceValue sequence)
        {
            if (sequence is null)
                throw new ArgumentNullException(nameof(sequence));

            var count = 0;

            state.Console.WriteTertiaryText("[", Theme);

            var delim = string.Empty;
            foreach (var element in sequence.Elements)
            {
                if (delim.Length != 0)
                {
                    state.Console.WriteTertiaryText(delim, Theme);
                }

                delim = ", ";
                Visit(state, element);
            }

            state.Console.WriteTertiaryText("]", Theme);

            return count;
        }

        protected override int VisitStructureValue(ThemedValueFormatterState state, StructureValue structure)
        {
            var count = 0;

            if (structure.TypeTag != null)
            {
                state.Console.WriteName(structure.TypeTag, Theme);

                state.Console.Write(" ");
            }

            state.Console.WriteTertiaryText("{", Theme);

            var delim = string.Empty;
            for (var index = 0; index < structure.Properties.Count; ++index)
            {
                if (delim.Length != 0)
                {
                    state.Console.WriteTertiaryText(delim, Theme);
                }

                delim = ", ";

                var property = structure.Properties[index];

                state.Console.WriteName(property.Name, Theme);

                state.Console.WriteTertiaryText("=", Theme);

                count += Visit(state.Nest(), property.Value);
            }

            state.Console.WriteTertiaryText("}", Theme);

            return count;
        }

        protected override int VisitDictionaryValue(ThemedValueFormatterState state, DictionaryValue dictionary)
        {
            var count = 0;

            state.Console.WriteTertiaryText("{", Theme);

            var delim = string.Empty;
            foreach (var element in dictionary.Elements)
            {
                if (delim.Length != 0)
                {
                    state.Console.WriteTertiaryText(delim, Theme);
                }

                delim = ", ";

                state.Console.WriteTertiaryText("[", Theme);

                count += Visit(state.Nest(), element.Key);

                state.Console.WriteTertiaryText("]=", Theme);

                count += Visit(state.Nest(), element.Value);
            }

            state.Console.WriteTertiaryText("}", Theme);

            return count;
        }

        public int FormatLiteralValue(ScalarValue scalar, IAnsiConsole console, string? format)
        {
            var value = scalar.Value;
            var count = 0;

            if (value is null)
            {
                console.WriteNull("null", Theme);
                return count;
            }

            if (value is string str)
            {
                if (format != "l")
                    console.WriteString(buffer => JsonValueFormatter.WriteQuotedJsonString(str, buffer), Theme);
                else
                    console.WriteString(str, Theme);
                return count;
            }

            if (value is ValueType)
            {
                if (value is int || value is uint || value is long || value is ulong ||
                    value is decimal || value is byte || value is sbyte || value is short ||
                    value is ushort || value is float || value is double)
                {
                    console.WriteNumber(buffer => scalar.Render(buffer, format, _formatProvider), Theme);
                    return count;
                }

                if (value is bool b)
                {
                    console.WriteBoolean(b.ToString(), Theme);
                    return count;
                }

                if (value is char ch)
                {
                    console.WriteScalar($"'{ch}'", Theme);
                    return count;
                }
            }

            console.WriteScalar(buffer => scalar.Render(buffer, format, _formatProvider), Theme);
            return count;
        }
    }
}