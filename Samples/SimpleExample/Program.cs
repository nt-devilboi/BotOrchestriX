using BotOrchestriX;
using BotOrchestriX.Abstract;
using BotOrchestriX.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SimpleExample.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();

//Settings

builder.Services.AddBaseTelegramCommands();
builder.Services.AddTelegramBotWithController<MenuHandler>(
    Environment.GetEnvironmentVariable("HOST_FOR_TG") ?? "https://160c81149caf11.lhr.life",
    Environment.GetEnvironmentVariable("TG_TOKEN") ??
    throw new ArgumentException("NOT HAVE TOKEN FOR BOT TG"));

builder.Services.AddTelegramDbContext<ChatTelegramDb>();

//addFlow
var registerFlow = new ServiceRegistryFlow();
builder.Services.AddFlow<HelloFlow>("Hello", x =>
    x.AddHandler<HiHandler>()
        .AddHandler<HowAreYouHandler>(), registerFlow);

builder.Services.AddSingleton<IServiceRegistryFlow>(registerFlow);

var app = builder.Build();


app.UseHttpsRedirection();

app.MapControllers();

app.Run();


public class ChatTelegramDb : ChatDb
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("FakeDbContext");
    }
}