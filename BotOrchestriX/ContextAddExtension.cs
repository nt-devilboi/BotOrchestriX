using Microsoft.Extensions.DependencyInjection;
using BotOrchestriX.Abstract;
using BotOrchestriX.BuilderContext;

namespace BotOrchestriX;

public static class ContextAddExtension
{
    public static void AddFlow(this IServiceCollection serviceCollection, string trigger,
        Action<BuilderContextFlow> builderFunc, IServiceRegistryFlow registryFlow)
    {
        var flowComponents = new FlowComponents();
        var builder = new BuilderContextFlow(flowComponents, serviceCollection);

        serviceCollection.AddScoped<Command>(_ => new Router(trigger, flowComponents));
        if (serviceCollection.HasDuplicate(trigger))
            throw new InvalidOperationException(
                $"Trigger: {trigger} already registered.");

        serviceCollection.AddSingleton(new RouterTriggerDescriptor(trigger));
        builderFunc(builder);

        builder.Build();

        registryFlow.AddFlow(builder.Steps);
    }

    private static bool HasDuplicate(this IServiceCollection serviceCollection, string trigger)
    {
        return serviceCollection.Any(d => d.ServiceType == typeof(RouterTriggerDescriptor)
                                          && d.ImplementationInstance is RouterTriggerDescriptor r
                                          && r.Trigger == trigger);
    }

    public static IServiceCollection AddTriggerProvider(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<TriggerProvider>();
        return serviceCollection;
    }
}