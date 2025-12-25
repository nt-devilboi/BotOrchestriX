using Microsoft.Extensions.DependencyInjection;
using BotOrchestriX.Abstract;
using BotOrchestriX.BuilderContext;

namespace BotOrchestriX;

public static class ContextAddExtension
{
    public static void AddFlow<TEnum>(this IServiceCollection serviceCollection, string trigger,
        Action<BuilderContextFlow> builderFunc, IServiceRegistryFlow registryFlow) where TEnum : struct, Enum
    {
        var flowComponents = new FlowComponents();
        var builder = new BuilderContextFlow(flowComponents, serviceCollection);

        serviceCollection.AddScoped<Command>(_ => new Router(trigger, flowComponents));
        var stateType = typeof(TEnum);
        if (serviceCollection.HasDuplicate(trigger))
            throw new InvalidOperationException(
                $"Trigger descriptor for state '{stateType.FullName}' already registered.");

        serviceCollection.AddSingleton<IRouterTriggerDescriptor>(new RouterTriggerDescriptor(stateType, trigger));
        builderFunc(builder);

        builder.Build();

        registryFlow.AddFlow<TEnum>(builder.Steps);
    }

    private static bool HasDuplicate(this IServiceCollection serviceCollection, string trigger)
    {
        return serviceCollection.Any(d => d.ServiceType == typeof(IRouterTriggerDescriptor)
                                          && d.ImplementationInstance is RouterTriggerDescriptor r
                                          && (r.StateType == typeof(Enum) || r.Trigger == trigger));
    }

    public static IServiceCollection AddTriggerProvider(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<ITriggerProvider, TriggerProvider>();
        return serviceCollection;
    }
}

public class FlowComponents //todo: на сколько этот класс вообще актуальный?
{
    private readonly List<string> States = [];
    public string Start => States[0];
    public string PrevHandler => States[^1];

    public void Add(string state)
    {
        States.Add(state);
    }
}

internal record IHandlerInfo(IContextHandler ContextHandler, string number);