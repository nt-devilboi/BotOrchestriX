# BotOrchestriX
A lightweight framework that simplifies bot development by encapsulating all the infrastructure so you can focus directly on your **business logic**.

Now it's work with **Telegram**
## 🚀 Why It Exists
Instead of spending time wiring up controllers, handlers, and message routing, **BotOrchestriX** lets you jump straight into defining how your bot should behave.  
It hides infrastructure complexity and gives you a clean entry point for working with logic and flows.


# Get started

Define a handler class that implements `IStrategyMenu`:
```csharp
public class MenuHandler(
    ITelegramBotClient botClient) : IStrategyMenu
{
    public async Task Handle(ChatContext context)
    {
        await botClient.SendTextMessageAsync(context.ChatId, "Hello User");
    }
}
```

Add Configuration
```csharp
builder.Services.AddBaseTelegramCommands();
builder.Services.AddTelegramBotWithController<MenuHandler>("HOST","TG_TOKEN");

builder.Services.AddTelegramDbContext<ChatTelegramDb>();
```
Can use `localhost.run/docs/` for webhoock

Define a handler class with you business logic that implements `ContextHandler<BasePayload, YourEnum>`
```csharp
public class YourClass(ITelegramBotClient botClient) : ContextHandler<BasePayload, HelloFlow>
```



Add Flow
```csharp
var registerFlow = new ServiceRegistryFlow();
builder.Services.AddFlow<YourEnum>("trigger", x =>
    x.AddHandler<YourClass>()
        .AddHandler<YourClass>(), registerFlow);

builder.Services.AddSingleton<IServiceRegistryFlow>(registerFlow);
```
YourClasses.Count() == YourEnum.Count()

