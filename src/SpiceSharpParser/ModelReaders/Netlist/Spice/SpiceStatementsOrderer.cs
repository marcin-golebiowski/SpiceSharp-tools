﻿using System.Collections.Generic;
using SpiceSharpParser.Models.Netlist.Spice.Objects;

namespace SpiceSharpParser.ModelReaders.Netlist.Spice
{
    public class SpiceStatementsOrderer : ISpiceStatementsOrderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpiceStatementsOrderer"/> class.
        /// </summary>
        public SpiceStatementsOrderer()
        {
        }

        protected List<string> TopControls { get; set; } = new List<string>() { "st_r", "step_r", "param", "func", "options" };

        protected List<string> ControlsAfterComponents { get; set; } = new List<string>() { "plot", "print", "save" };

        protected List<string> Controls { get; set; } = new List<string>() { "temp", "step", "st", "mc", "op", "ac", "tran", "dc", "ic", "nodeset" };

        /// <summary>
        /// Orders statements for reading.
        /// </summary>
        /// <param name="statements">Statement to order.</param>
        /// <returns>
        /// Ordered statements.
        /// </returns>
        public IEnumerable<Statement> Order(Statements statements)
        {
            return statements.OrderBy((Statement s) => GetOrder(s));
        }

        protected virtual int GetOrder(Statement statement)
        {
            if (statement is Model)
            {
                return 2000;
            }

            if (statement is Component)
            {
                return 3000;
            }

            if (statement is SubCircuit)
            {
                return 1000;
            }

            if (statement is Control c)
            {
                var name = c.Name.ToLower();

                if (ControlsAfterComponents.Contains(name))
                {
                    return 4000;
                }

                if (TopControls.Contains(name))
                {
                    return TopControls.IndexOf(name);
                }

                return 3500 + Controls.IndexOf(name);
            }

            return int.MaxValue;
        }
    }
}