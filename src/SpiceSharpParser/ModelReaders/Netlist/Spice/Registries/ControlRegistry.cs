﻿using System;
using SpiceSharpParser.ModelsReaders.Netlist.Spice.Readers.Controls;

namespace SpiceSharpParser.ModelsReaders.Netlist.Spice.Registries
{
    /// <summary>
    /// Registry for <see cref="BaseControl"/>s
    /// </summary>
    public class ControlRegistry : BaseRegistry<BaseControl>, IControlRegistry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlRegistry"/> class.
        /// </summary>
        public ControlRegistry()
        {
        }
    }
}