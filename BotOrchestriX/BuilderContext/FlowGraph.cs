using BotOrchestriX.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace BotOrchestriX.BuilderContext;

internal interface IFlowNodeVisitor<TState> where TState : struct, Enum
{
    void Visit(HandlerNode<TState> node);
    void Visit(SwitchNode<TState> node);
}

internal abstract class FlowNode<TState> where TState : struct, Enum
{
    public required TState State { get; init; }
    public abstract void Accept(IFlowNodeVisitor<TState> visitor);
}

internal sealed class HandlerNode<TState> : FlowNode<TState> where TState : struct, Enum
{
    public required Type HandlerType { get; init; }
    public List<FlowNode<TState>> SubTasks { get; } = new();

    public override void Accept(IFlowNodeVisitor<TState> visitor) => visitor.Visit(this);
}

internal sealed class SwitchNode<TState> : FlowNode<TState> where TState : struct, Enum
{
    public required Type HandlerType { get; init; }
    public Dictionary<string, FlowNode<TState>> Branches { get; } = new();

    public override void Accept(IFlowNodeVisitor<TState> visitor) => visitor.Visit(this);
}

internal sealed class StateEventGeneratorVisitor<TState> : IFlowNodeVisitor<TState> where TState : struct, Enum
{
    private readonly List<StateEvent> _events = new();
    private (TState? state, bool inSwitch) PreviousNode;

    public IReadOnlyList<StateEvent> Events => _events;

    public void Visit(HandlerNode<TState> node)
    {
        if (PreviousNode is { state: not null, inSwitch: false })
        {
            _events.Add(new StateEvent(Trigger.UserWantToContinue, PreviousNode.state.Value,
                node.State));
        }

        PreviousNode = (node.State, PreviousNode.inSwitch);
    }

    public void Visit(SwitchNode<TState> node)
    {
        if (PreviousNode.state.HasValue)
        {
            _events.Add(new StateEvent(Trigger.UserWantToContinue, PreviousNode.state.Value,
                node.State));
        }

        PreviousNode = (node.State, true);

        foreach (var branch in node.Branches.Values)
        {
            _events.Add(new StateEvent(Trigger.UserGoToSubTask, node.State,
                branch.State, branch.State.ToString()));
            branch.Accept(this);
        }

        PreviousNode = (node.State, false);
    }
}

internal sealed class ServiceRegistrationVisitor<TState>(IServiceCollection collection) : IFlowNodeVisitor<TState>
    where TState : struct, Enum
{
    public void Visit(HandlerNode<TState> node)
    {
        collection.AddScoped(node.HandlerType);
        collection.AddScoped<IHandlerInfo>(sp =>
            new IHandlerInfo((IContextHandler)sp.GetRequiredService(node.HandlerType), node.State.ToString()));

        foreach (var sub in node.SubTasks)
            sub.Accept(this);
    }

    public void Visit(SwitchNode<TState> node)
    {
        collection.AddScoped(node.HandlerType);
        collection.AddScoped<IHandlerInfo>(sp =>
            new IHandlerInfo((IContextHandler)sp.GetRequiredService(node.HandlerType), node.State.ToString()));

        foreach (var branch in node.Branches.Values)
        {
            branch.Accept(this);
        }
    }
}