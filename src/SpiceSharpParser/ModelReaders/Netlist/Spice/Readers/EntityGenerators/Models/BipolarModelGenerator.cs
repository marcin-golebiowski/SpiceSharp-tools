﻿using System.Collections.Generic;
using SpiceSharp.Components;
using SpiceSharpParser.ModelReaders.Netlist.Spice.Context;
using SpiceSharpParser.Models.Netlist.Spice.Objects;

namespace SpiceSharpParser.ModelReaders.Netlist.Spice.Readers.EntityGenerators.Models
{
    public class BipolarModelGenerator : ModelGenerator
    {
        public override IEnumerable<string> GeneratedTypes
        {
            get
            {
                return new List<string>() { "npn", "pnp" };
            }
        }

        public override SpiceSharp.Components.Model Generate(string name, string type, ParameterCollection parameters, IReadingContext context)
        {
            BipolarJunctionTransistorModel model = new BipolarJunctionTransistorModel(name);

            if (type.ToLower() == "npn")
            {
                model.SetParameter("npn", true);
            }
            else if (type.ToLower() == "pnp")
            {
                model.SetParameter("pnp", true);
            }

            SetParameters(context, model, parameters);

            return model;
        }
    }
}
