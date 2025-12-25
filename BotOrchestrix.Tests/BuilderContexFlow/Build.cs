using BotOrchestriX;
using BotOrchestriX.Abstract;
using BotOrchestriX.BuilderContext;
using BotOrchestriX.Entity;
using BotOrchestrix.Tests.ContextAddExtension;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BotOrchestrix.Tests.StateEventGeneratorVisitor;

public class Build
{
    private BuilderContextFlow builderContextFlow;

    [SetUp]
    public void Setup()
    {
        builderContextFlow = new BuilderContextFlow(new FlowComponents(), new Mock<IServiceCollection>().Object);
    }

    [Test]
    public void CorrectStateEvent_IF_UseOnlyHandler()
    {
        builderContextFlow.AddHandler<AddFlow.FakeHandler>().AddHandler<AddFlow.FakeHandler2>();
        builderContextFlow.Build();


        builderContextFlow.Steps.Count.Should().Be(2);
        builderContextFlow.Steps.Should()
            .BeEquivalentTo([
                new StateEvent(Trigger.UserWantToContinue,
                    typeof(AddFlow.FakeHandler).FullName,
                    typeof(AddFlow.FakeHandler2).FullName),
                new StateEvent(Trigger.UserWantToContinue,
                    typeof(AddFlow.FakeHandler2).FullName,
                    nameof(BaseContextState.Menu))
            ]);
    }
    
    //todo: а если будет пусто?
}