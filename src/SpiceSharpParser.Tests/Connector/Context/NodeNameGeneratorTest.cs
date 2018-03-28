﻿using SpiceSharpParser.Connector.Context;
using Xunit;

namespace SpiceSharpParser.Tests.Connector.Context
{
    public class NodeNameGeneratorTest
    {
        [Fact]
        public void GenerateNoSubcircuitTest()
        {
            var generator = new NodeNameGenerator();

            // ground nodes
            Assert.Equal("0", generator.Generate("0"));
            Assert.Equal("GND", generator.Generate("gnd"));
            Assert.Equal("GND", generator.Generate("Gnd"));
            Assert.Equal("GND", generator.Generate("GND"));

            // ordinary nodes
            Assert.Equal("a", generator.Generate("a"));
            Assert.Equal("Ab", generator.Generate("Ab"));
        }

        [Fact]
        public void GenerateWithSubcircuitTest()
        {
            var subcircuit = new Model.SpiceObjects.SubCircuit();
            subcircuit.Pins = new System.Collections.Generic.List<string>() { "IN", "OUT" };
            subcircuit.DefaultParameters =
                new System.Collections.Generic.List<Model.SpiceObjects.Parameters.AssignmentParameter>() {
                    new Model.SpiceObjects.Parameters.AssignmentParameter() { Name = "L", Value = "100" },
                    new Model.SpiceObjects.Parameters.AssignmentParameter() { Name = "C", Value = "10" } };

            var generator = new NodeNameGenerator("x1", subcircuit, new System.Collections.Generic.List<string>() { "net2", "net3" });

            // ground nodes
            Assert.Equal("0", generator.Generate("0"));
            Assert.Equal("GND", generator.Generate("gnd"));
            Assert.Equal("GND", generator.Generate("Gnd"));
            Assert.Equal("GND", generator.Generate("GND"));

            // ordinary nodes
            Assert.Equal("x1.a", generator.Generate("a"));
            Assert.Equal("x1.Ab", generator.Generate("Ab"));

            // subcircuit named nodes
            Assert.Equal("net2", generator.Generate("IN"));
            Assert.Equal("net3", generator.Generate("OUT"));
        }
    }
}