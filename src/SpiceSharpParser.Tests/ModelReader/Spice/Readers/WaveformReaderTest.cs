using NSubstitute;
using SpiceSharpParser.ModelReader.Netlist.Spice.Context;
using SpiceSharpParser.ModelReader.Netlist.Spice.Readers;
using SpiceSharpParser.ModelReader.Netlist.Spice.Readers.Waveforms;
using SpiceSharpParser.ModelReader.Netlist.Spice.Registries;
using SpiceSharpParser.Model.Netlist.Spice.Objects.Parameters;
using SpiceSharp.Components;
using System;
using Xunit;

namespace SpiceSharpParser.Tests.ModelReader.Spice.Readers
{
    public class WaveformReaderTest
    {
        [Fact]
        public void GenerateTest()
        {
            // arrange
            var waveFormGenerator = Substitute.For<WaveformGenerator>();
            waveFormGenerator.Generate(Arg.Any<BracketParameter>(), Arg.Any<IReadingContext>()).Returns(new Sine());

            var waveFormRegistry = Substitute.For<IWaveformRegistry>();
            waveFormRegistry.Supports("func").Returns(true);
            waveFormRegistry.Get(Arg.Any<string>()).Returns(waveFormGenerator);

            var bracketParameter = new Model.Netlist.Spice.Objects.Parameters.BracketParameter();
            bracketParameter.Name = "FUNc";
            var readingContext = Substitute.For<IReadingContext>();

            // act
            WaveformReader waveformReader = new WaveformReader(waveFormRegistry);
            var waveForm = waveformReader.Generate(bracketParameter, readingContext);

            // assert
            Assert.IsType<Sine>(waveForm);
        }

        [Fact]
        public void NotSupportedGenerateTest()
        {
            // arrange
            var waveFormGenerator = Substitute.For<WaveformGenerator>();
            waveFormGenerator.Generate(Arg.Any<BracketParameter>(), Arg.Any<IReadingContext>()).Returns(new Sine());

            var waveFormRegistry = Substitute.For<IWaveformRegistry>();
            waveFormRegistry.Supports("func").Returns(true);
            waveFormRegistry.Get(Arg.Any<string>()).Returns(waveFormGenerator);

            var bracketParameter = new Model.Netlist.Spice.Objects.Parameters.BracketParameter();
            bracketParameter.Name = "func2";
            var readingContext = Substitute.For<IReadingContext>();

            // act + assert
            WaveformReader waveformReader = new WaveformReader(waveFormRegistry);
            Assert.Throws<Exception>(() => waveformReader.Generate(bracketParameter, readingContext));
        }
    }
}