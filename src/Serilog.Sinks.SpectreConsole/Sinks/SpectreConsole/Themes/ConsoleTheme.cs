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
using System.Linq;
using Spectre.Console;

namespace Serilog.Sinks.SpectreConsole.Themes
{
    /// <summary>
    /// The base class for styled terminal output.
    /// </summary>
    public class ConsoleTheme
    {
        private readonly Dictionary<ConsoleThemeStyle,Style> _styles;

        /// <summary>
        /// No styling applied.
        /// </summary>
        public static ConsoleTheme None { get; } = new EmptyConsoleTheme();

        /// <summary>
        /// A 256-color theme along the lines of Visual Studio Code.
        /// </summary>
        /// <param name="exceptionSettings">The <see cref="ExceptionSettings"/> that controls how exceptions are rendered.</param>
        public static ConsoleTheme Code(ExceptionSettings? exceptionSettings = null) => ConsoleThemes.Code(exceptionSettings);

        /// <summary>
        /// A 256-color theme along the lines of Visual Studio Code.
        /// </summary>
        /// <param name="format">The <see cref="ExceptionFormats"/> that controls how exceptions are formatted.</param>
        public static ConsoleTheme Code(ExceptionFormats format) => ConsoleThemes.Code(new ExceptionSettings { Format = format });

        /// <summary>
        /// A theme using only gray, black and white.
        /// </summary>
        /// <param name="exceptionSettings">The <see cref="ExceptionSettings"/> that controls how exceptions are rendered.</param>
        public static ConsoleTheme Grayscale(ExceptionSettings? exceptionSettings = null) => ConsoleThemes.Grayscale(exceptionSettings);

        /// <summary>
        /// A theme using only gray, black and white.
        /// </summary>
        /// <param name="format">The <see cref="ExceptionFormats"/> that controls how exceptions are formatted.</param>
        public static ConsoleTheme Grayscale(ExceptionFormats format) => ConsoleThemes.Grayscale(new ExceptionSettings { Format = format });

        /// <summary>
        /// A theme in the style of the original <i>Serilog.Sinks.Literate</i>.
        /// </summary>
        /// <param name="exceptionSettings">The <see cref="ExceptionSettings"/> that controls how exceptions are rendered.</param>
        public static ConsoleTheme Literate(ExceptionSettings? exceptionSettings = null) => ConsoleThemes.Literate(exceptionSettings);

        /// <summary>
        /// A theme in the style of the original <i>Serilog.Sinks.Literate</i>.
        /// </summary>
        /// <param name="format">The <see cref="ExceptionFormats"/> that controls how exceptions are formatted.</param>
        public static ConsoleTheme Literate(ExceptionFormats format) => ConsoleThemes.Literate(new ExceptionSettings { Format = format });

        /// <summary>
        /// A theme based on the original Serilog "colored console" sink.
        /// </summary>
        /// <param name="exceptionSettings">The <see cref="ExceptionSettings"/> that controls how exceptions are rendered.</param>
        public static ConsoleTheme Colored(ExceptionSettings? exceptionSettings = null) => ConsoleThemes.Colored(exceptionSettings);

        /// <summary>
        /// A theme based on the original Serilog "colored console" sink.
        /// </summary>
        /// <param name="format">The <see cref="ExceptionFormats"/> that controls how exceptions are formatted.</param>
        public static ConsoleTheme Colored(ExceptionFormats format) => ConsoleThemes.Colored(new ExceptionSettings { Format = format });

        /// <summary>
        /// The <see cref="ExceptionSettings"/> used to render exceptions.
        /// </summary>
        public ExceptionSettings ExceptionSettings { get; }

        /// <summary>
        /// Construct a theme given a set of styles.
        /// </summary>
        /// <param name="styles">Styles to apply within the theme.</param>
        /// <param name="exceptionSettings"></param>
        /// <exception cref="ArgumentNullException">When <paramref name="styles"/> is <code>null</code></exception>
        public ConsoleTheme(IReadOnlyDictionary<ConsoleThemeStyle, Style> styles, ExceptionSettings? exceptionSettings)
        {
            if (styles is null) throw new ArgumentNullException(nameof(styles));
            _styles = styles.ToDictionary(kv => kv.Key, kv => kv.Value);
            ExceptionSettings = exceptionSettings ?? new ExceptionSettings();
        }

        /// <summary>
        /// Get the Spectre.Console <see cref="Style"/> corresponding to the given <paramref name="themeStyle"/>.
        /// </summary>
        /// <param name="themeStyle">The theme style.</param>
        /// <returns>The Spectre.Console <see cref="Style"/> corresponding to the given <paramref name="themeStyle"/>.</returns>
        public Style GetStyle(ConsoleThemeStyle themeStyle)
        {
            return _styles.TryGetValue(themeStyle, out var style) ? style : Style.Plain;
        }
    }
}