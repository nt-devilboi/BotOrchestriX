using BotOrchestriX.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace BotOrchestriX.BuilderContext;

public class BuilderContextFlow<TState> where TState : struct, Enum
{
    private readonly IServiceCollection _collection;
    private readonly FlowComponents<TState> freeFlowComponent;
    internal readonly List<StateEvent> Steps = [];
    private readonly List<FlowNode<TState>> _nodes = new();

    internal BuilderContextFlow(FlowComponents<TState> freeFlowComponent, IServiceCollection collection,
        List<StateEvent>? steps = null)
    {
        _collection = collection;
        this.freeFlowComponent = freeFlowComponent;
        Steps = steps ?? [];
    }


    public BuilderContextFlow<TState> AddHandler<TContextHandler>()
        where TContextHandler : class, IContextHandler
    {
        if (freeFlowComponent.Empty) throw new ArgumentException("capacity for handler is exhausted");
        var start = freeFlowComponent.FreeState;
        freeFlowComponent.Next();

        var node = new HandlerNode<TState>
        {
            State = start,
            HandlerType = typeof(TContextHandler)
        };

        _nodes.Add(node);
        freeFlowComponent.PrevHandler = start;
        return this;
    }


    public BuilderContextFlow<TState> AddSwitch<TContextHandler>(
        params (Action<BuilderContextFlow<TState>> action, string name)[] events)
        where TContextHandler : class, IContextHandler
    {
        if (freeFlowComponent.Empty) throw new ArgumentException("capacity for handler is exhausted");
        var start = freeFlowComponent.FreeState;
        freeFlowComponent.Next();

        var switchNode = new SwitchNode<TState>
        {
            HandlerType = typeof(TContextHandler),
            State = start
        };

        _nodes.Add(switchNode);
        foreach (var action1 in events)
        {
            var subTaskBuilder = new BuilderContextFlow<TState>(freeFlowComponent, _collection, Steps);
            action1.action(subTaskBuilder);

            switchNode.Branches.Add(action1.name, subTaskBuilder._nodes[0]);
        }

        return this;
    }

    internal void Build()
    {
        var serviceVisitor = new ServiceRegistrationVisitor<TState>(_collection);
        foreach (var node in _nodes)
            node.Accept(serviceVisitor);

        var eventVisitor = new StateEventGeneratorVisitor<TState>();
        foreach (var node in _nodes)
            node.Accept(eventVisitor);

        Steps.Clear();
        Steps.AddRange(eventVisitor.Events);
    }
}