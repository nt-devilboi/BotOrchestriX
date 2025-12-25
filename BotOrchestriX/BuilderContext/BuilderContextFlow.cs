using BotOrchestriX.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace BotOrchestriX.BuilderContext;

public class BuilderContextFlow
{
    private readonly IServiceCollection _collection;
    private readonly FlowComponents _flowComponents;
    internal readonly List<StateEvent> Steps = [];
    private readonly List<FlowNode> _nodes = new();

    internal BuilderContextFlow(FlowComponents flowComponents, IServiceCollection collection,
        List<StateEvent>? steps = null)
    {
        _collection = collection;
        this._flowComponents = flowComponents;
        Steps = steps ?? [];
    }


    public BuilderContextFlow AddHandler<TContextHandler>()
        where TContextHandler : class, IContextHandler
    {
        var node = new HandlerNode
        {
            State = typeof(TContextHandler).FullName,
            HandlerType = typeof(TContextHandler)
        };

        _flowComponents.Add(node.State);
        _nodes.Add(node);
        return this;
    }


    public BuilderContextFlow AddSwitch<TContextHandler>(
        params (Action<BuilderContextFlow> action, string name)[] events)
        where TContextHandler : class, IContextHandler
    {
        var switchNode = new SwitchNode
        {
            HandlerType = typeof(TContextHandler),
            State = typeof(TContextHandler).FullName
        };

        _flowComponents.Add(switchNode.State);
        _nodes.Add(switchNode);
        foreach (var action1 in events)
        {
            var subTaskBuilder = new BuilderContextFlow(_flowComponents, _collection, Steps);
            action1.action(subTaskBuilder);

            switchNode.Branches.Add(action1.name, subTaskBuilder._nodes[0]);
        }

        return this;
    }

    internal void Build()
    {
        var serviceVisitor = new ServiceRegistrationVisitor(_collection);
        foreach (var node in _nodes)
            node.Accept(serviceVisitor);

        var eventVisitor = new StateEventGeneratorVisitor();
        foreach (var node in _nodes)
            node.Accept(eventVisitor);

        eventVisitor.ConnectWithMenu();
        Steps.Clear();
        Steps.AddRange(eventVisitor.Events);
    }
}