using NSubstitute;
using SpiceNetlist.SpiceObjects;
using SpiceNetlist.SpiceObjects.Parameters;
using SpiceNetlist.SpiceSharpConnector.Context;
using SpiceNetlist.SpiceSharpConnector.Processors;
using SpiceNetlist.SpiceSharpConnector.Processors.EntityGenerators.Models;
using SpiceNetlist.SpiceSharpConnector.Processors.Waveforms;
using SpiceNetlist.SpiceSharpConnector.Registries;
using SpiceSharp;
using SpiceSharp.Circuits;
using SpiceSharp.Components;
using System;
using Xunit;

namespace SpiceNetlist.SpiceSharpConnector.Tests.Processors
{
    public class ComponentProcessorTest
    {
        [Fact]
        public void GenerateTest()
        {
            // arrange
            var generator = Substitute.For<EntityGenerator>();
            generator.Generate(
               Arg.Any<Identifier>(),
               Arg.Any<string>(),
               Arg.Any<string>(),
               Arg.Any<ParameterCollection>(),
               Arg.Any<IProcessingContext>()).Returns(x => new Resistor((Identifier)x[0]));

            var registry = Substitute.For<IEntityGeneratorRegistry>();
            registry.Supports("r").Returns(true);
            registry.Get("r").Returns(generator);

            var processingContext = Substitute.For<IProcessingContext>();
            processingContext.NodeNameGenerator.Returns(new NodeNameGenerator());
            processingContext.ObjectNameGenerator.Returns(new ObjectNameGenerator(string.Empty));

            var resultService = Substitute.For<IResultService>();
            processingContext.Result.Returns(resultService);

            // act
            ComponentProcessor processor = new ComponentProcessor(registry);
            var component = new SpiceObjects.Component() { Name = "Ra1", PinsAndParameters = new ParameterCollection() { new ValueParameter("0"), new ValueParameter("1"), new ValueParameter("12.3") } };
            processor.Process(component, processingContext);

            // assert
            generator.Received().Generate(new Identifier("Ra1"), "Ra1", "r", Arg.Any<ParameterCollection>(), Arg.Any<IProcessingContext>());
            resultService.Received().AddEntity(Arg.Is<Entity>((Entity e) => e.Name.Name == "Ra1"));
        }
    }
}