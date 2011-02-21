using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Opt
{
    /// <summary>
    /// This class can be descended from to get the normal verbose and help options.
    /// </summary>
    public class OptionsBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether to produce verbose output.
        /// </summary>
        [BooleanOption("--verbose")]
        [BooleanOption("-v")]
        [Description("Verbose output")]
        public bool Verbose
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the help text.
        /// </summary>
        [BooleanOption("--help")]
        [BooleanOption("-h")]
        [Description("Show the command line help")]
        public bool ShowHelp
        {
            get;
            set;
        }
    }
}