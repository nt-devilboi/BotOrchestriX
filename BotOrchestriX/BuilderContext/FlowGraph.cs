using BotOrchestriX.Abstract;
using BotOrchestriX.Entity;
using Microsoft.Extensions.DependencyInjection;

namespace BotOrchestriX.BuilderContext;

internal interface IFlowNodeVisitor
{
    void Visit(HandlerNode node);
    void Visit(SwitchNode node);
}

internal abstract class FlowNode
{
    public required string State { get; init; }
    public abstract void Accept(IFlowNodeVisitor visitor);
}

internal sealed class HandlerNode : FlowNode
{
    public required Type HandlerType { get; init; }

    public List<FlowNode> SubTasks { get; } =
        new(); // чувак давай потом сделаем просто Next, есть свич какие здесь та subTask?

    public override void Accept(IFlowNodeVisitor visitor) => visitor.Visit(this);
}

internal sealed class SwitchNode : FlowNode
{
    public required Type HandlerType { get; init; }
    public Dictionary<string, FlowNode> Branches { get; } = new();

    public override void Accept(IFlowNodeVisitor visitor) => visitor.Visit(this);
}

internal sealed class StateEventGeneratorVisitor : IFlowNodeVisitor
{
    private readonly List<StateEvent> _events = new();
    private (string state, bool inSwitch) PreviousNode;

    public void ConnectWithMenu()
    {
        if (PreviousNode.state == BaseContextState.Menu.ToString()) return;

        _events.Add(new StateEvent(Trigger.UserWantToContinue, PreviousNode.state, BaseContextState.Menu.ToString()));
        PreviousNode = (BaseContextState.Menu.ToString(), false);
    }

    public IReadOnlyList<StateEvent> Events => _events;

    public void Visit(HandlerNode node)
    {
        if (PreviousNode is { state: not null, inSwitch: false })
        {
            _events.Add(new StateEvent(Trigger.UserWantToContinue, PreviousNode.state,
                node.State));
        }

        PreviousNode = (node.State, PreviousNode.inSwitch);
    }

    public void Visit(SwitchNode node)
    {
        if (!string.IsNullOrEmpty(PreviousNode.state))
        {
            _events.Add(new StateEvent(Trigger.UserWantToContinue, PreviousNode.state,
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

internal sealed class ServiceRegistrationVisitor(IServiceCollection collection) : IFlowNodeVisitor
{
    public void Visit(HandlerNode node)
    {
        collection.AddScoped(node.HandlerType);
        collection.AddScoped<IContextHandlerDescriptor>(sp =>
            new IContextHandlerDescriptor((IContextHandler)sp.GetRequiredService(node.HandlerType), node.State));

        foreach (var sub in node.SubTasks)
            sub.Accept(this);
    }

    public void Visit(SwitchNode node)
    {
        collection.AddScoped(node.HandlerType);
        collection.AddScoped<IContextHandlerDescriptor>(sp =>
            new IContextHandlerDescriptor((IContextHandler)sp.GetRequiredService(node.HandlerType), node.State));

        foreach (var branch in node.Branches.Values)
        {
            branch.Accept(this);
        }
    }
}