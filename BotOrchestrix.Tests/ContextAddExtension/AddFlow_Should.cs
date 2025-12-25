using BotOrchestriX;
using BotOrchestriX.Abstract;
using BotOrchestriX.Entity;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Stateless;
using Telegram.Bot.Types;

namespace BotOrchestrix.Tests.ContextAddExtension;

public class AddFlow
{
    private IServiceCollection collection;

    [SetUp]
    public void Setup()
    {
        collection = new ServiceCollection();
        collection.AddSingleton<IServiceRegistryFlow, ServiceRegistryFlow>();
    }


    [Test]
    public void CorrectWork_IF_UseAddHandler()
    {
        var serviceRegistry = collection.BuildServiceProvider().GetService<IServiceRegistryFlow>();
        collection.AddFlow("test",
            x => x.AddHandler<FakeHandler>()
                .AddHandler<FakeHandler2>()
                .AddHandler<FakeSwitch>(),
            serviceRegistry);

        var stateMachine = new StateMachine<string, Trigger>(typeof(FakeHandler).FullName);
        serviceRegistry.Wraps(stateMachine);

        var states = stateMachine.GetInfo().States.ToArray();
        states.Length.Should().Be(4);

        stateMachine.State.Should().Be(typeof(FakeHandler).FullName);
        stateMachine.Fire(Trigger.UserWantToContinue);

        stateMachine.State.Should().Be(typeof(FakeHandler2).FullName);
        stateMachine.Fire(Trigger.UserWantToContinue);

        stateMachine.State.Should().Be(typeof(FakeSwitch).FullName);
        stateMachine.Fire(Trigger.UserWantToContinue);

        stateMachine.State.Should().Be(nameof(BaseContextState.Menu));
    }

    [Test]
    public void InvalidOperationException_IF_UseSameTrigger()
    {
        var serviceRegistry = collection.BuildServiceProvider().GetService<IServiceRegistryFlow>();
        collection.AddFlow("test",
            x => x.AddHandler<FakeHandler>(),
            serviceRegistry);

        var act = () => { collection.AddFlow("test", x => x.AddHandler<FakeHandler>(), serviceRegistry); };

        act.Should().Throw<InvalidOperationException>();
    }

    public class FakeHandler : ContextHandler<BasePayload>
    {
        protected override async Task Handle(Update update, DetailContext<BasePayload> context)
        {
            context.State.Continue();
        }

        protected override Task Enter(DetailContext<BasePayload> context)
        {
            throw new NotImplementedException();
        }
    }

    public class FakeSwitch : ContextHandler<BasePayload>
    {
        protected override async Task Handle(Update update, DetailContext<BasePayload> context)
        {
            context.State.Continue();
        }

        protected override Task Enter(DetailContext<BasePayload> context)
        {
            throw new NotImplementedException();
        }
    }

    public class FakeHandler2 : ContextHandler<BasePayload>
    {
        protected override async Task Handle(Update update, DetailContext<BasePayload> context)
        {
            context.State.Continue();
        }

        protected override Task Enter(DetailContext<BasePayload> context)
        {
            throw new NotImplementedException();
        }
    }
}