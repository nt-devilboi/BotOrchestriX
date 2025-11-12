using BotOrchestriX;
using BotOrchestriX.Abstract;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Stateless;
using Telegram.Bot.Types;

namespace BotOrchestrix.Tests.ContextAddExtension;

public class Tests
{
    private IServiceCollection collection;

    [SetUp]
    public void Setup()
    {
        collection = new ServiceCollection();
        collection.AddSingleton<IServiceRegistryFlow, ServiceRegistryFlow>();
    }

    [Test]
    public void CorrectWork_IF_UseAddHandlerAndAddSubHandle()
    {
        var serviceRegistry = collection.BuildServiceProvider().GetService<IServiceRegistryFlow>();
        collection.AddFlow<FakeFlow>("test",
            x => x.AddSwitch<FakeSwitch>(
                    (x => x.AddHandler<FakeHandler>(), "whoap"),
                    (x => x.AddHandler<FakeHandler>(), "lol"))
                .AddHandler<FakeHandler>(),
            serviceRegistry);

        var stateMachine = new StateMachine<FakeFlow, Trigger>(FakeFlow.Authorization);
        serviceRegistry.Wraps(stateMachine);

        var enums = Enum.GetValues<FakeFlow>();
        var states = stateMachine.GetInfo().States.ToArray();
        states.Length.Should().Be(enums.Length);
        for (var i = 0; i < enums.Length; i++)
        {
            states[i].UnderlyingState.Should().Be(enums[i]);
        }


        var userGoToSubTask =
            new StateMachine<FakeFlow, Trigger>.TriggerWithParameters<string>(Trigger.UserGoToSubTask);


        stateMachine.Fire(userGoToSubTask, FakeFlow.AddSecondName.ToString());
        stateMachine.State.Should().Be(FakeFlow.AddSecondName);


        stateMachine = new StateMachine<FakeFlow, Trigger>(FakeFlow.Authorization);
        serviceRegistry.Wraps(stateMachine);
        stateMachine.Fire(userGoToSubTask, FakeFlow.AddOld.ToString());
        stateMachine.State.Should().Be(FakeFlow.AddOld);

        stateMachine = new StateMachine<FakeFlow, Trigger>(FakeFlow.Authorization);
        serviceRegistry.Wraps(stateMachine);
        
        stateMachine.Fire(Trigger.UserWantToContinue);
        stateMachine.State.Should().Be(FakeFlow.AddName);
    }

    [Test]
    public void CorrectWork_IF_UseAddHandler()
    {
        var serviceRegistry = collection.BuildServiceProvider().GetService<IServiceRegistryFlow>();
        collection.AddFlow<FakeFlow>("test",
            x => x.AddHandler<FakeHandler>()
                .AddHandler<FakeHandler2>()
                .AddHandler<FakeHandler2>()
                .AddHandler<FakeHandler>(),
            serviceRegistry);

        var stateMachine = new StateMachine<FakeFlow, Trigger>(FakeFlow.Authorization);
        serviceRegistry.Wraps(stateMachine);

        var enums = Enum.GetValues<FakeFlow>();
        var states = stateMachine.GetInfo().States.ToArray();
        enums.Length.Should().Be(states.Length);
        for (var i = 0; i < enums.Length; i++)
        {
            states[i].UnderlyingState.Should().Be(enums[i]);
        }


        stateMachine.State.Should().Be(FakeFlow.Authorization);
        stateMachine.Fire(Trigger.UserWantToContinue);

        stateMachine.State.Should().Be(FakeFlow.AddSecondName);
        stateMachine.Fire(Trigger.UserWantToContinue);

        stateMachine.State.Should().Be(FakeFlow.AddOld);
        stateMachine.Fire(Trigger.UserWantToContinue);

        stateMachine.State.Should().Be(FakeFlow.AddName);
    }

    [Test]
    public void CorrectWork_IF_MoveToSwitch()
    {
        var serviceRegistry = collection.BuildServiceProvider().GetService<IServiceRegistryFlow>();
        collection.AddFlow<FakeFlow>("test",
            x => x.AddHandler<FakeHandler2>().AddSwitch<FakeSwitch>(
                (x => x.AddHandler<FakeHandler>(), "whoap")),
            serviceRegistry);

        var stateMachine = new StateMachine<FakeFlow, Trigger>(FakeFlow.Authorization);
        serviceRegistry.Wraps(stateMachine);

        stateMachine.Fire(Trigger.UserWantToContinue);
        stateMachine.State.Should().Be(FakeFlow.AddSecondName);
    }
}

public class FakeHandler : ContextHandler<BasePayload, FakeFlow>
{
    protected override async Task Handle(Update update, DetailContext<BasePayload, FakeFlow> context)
    {
        context.State.Continue();
    }

    protected override Task Enter(DetailContext<BasePayload, FakeFlow> context)
    {
        throw new NotImplementedException();
    }
}

public class FakeSwitch : ContextHandler<BasePayload, FakeFlow>
{
    protected override async Task Handle(Update update, DetailContext<BasePayload, FakeFlow> context)
    {
        context.State.Continue();
    }

    protected override Task Enter(DetailContext<BasePayload, FakeFlow> context)
    {
        throw new NotImplementedException();
    }
}

public class FakeHandler2 : ContextHandler<BasePayload, FakeFlow>
{
    protected override async Task Handle(Update update, DetailContext<BasePayload, FakeFlow> context)
    {
        context.State.Continue();
    }

    protected override Task Enter(DetailContext<BasePayload, FakeFlow> context)
    {
        throw new NotImplementedException();
    }
}