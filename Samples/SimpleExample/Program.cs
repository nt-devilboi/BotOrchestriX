using BotOrchestriX;
using BotOrchestriX.Abstract;
using BotOrchestriX.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SimpleExample.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();

//Settings

builder.Services.AddBaseTelegramCommands();
await builder.Services.AddTelegramBot<MenuHandler>(
    Environment.GetEnvironmentVariable("HOST_FOR_TG") ?? "https://efd7f4bbf379ce.lhr.life", "api/update/message",
    Environment.GetEnvironmentVariable("TG_TOKEN", EnvironmentVariableTarget.User) ??
    throw new ArgumentException("NOT HAVE TOKEN FOR BOT TG"));

builder.Services.AddTelegramDbContext<ChatTelegramDb>();

//addFlow
var registerFlow = new ServiceRegistryFlow();
builder.Services.AddFlow("Hello", x =>
    x.AddHandler<HiHandler>()
        .AddHandler<HowAreYouHandler>(), registerFlow);

builder.Services.AddSingleton<IServiceRegistryFlow>(registerFlow);

var app = builder.Build();


app.UseHttpsRedirection();

app.MapControllers();
app.MapTelegram();

app.Run();


public class ChatTelegramDb : ChatDb
{
    public ChatTelegramDb(DbContextOptions options) : base(options)
    {
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("FakeDbContext");
    }
}