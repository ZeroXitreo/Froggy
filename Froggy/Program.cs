using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Froggy;

public class Program
{
    private static readonly AppSettings appSettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText("appsettings.json"))!;

    private static IServiceProvider CreateProvider()
    {
        var services = new ServiceCollection();

        services.AddSingleton(appSettings);

        services
            .AddSingleton(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            })
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(sp => new InteractionService(sp.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<PlaylistService>()
            .AddSingleton<InteractionHandler>();

        return services.BuildServiceProvider();
    }

    static async Task Main(string[] args)
    {
        var services = CreateProvider();

        var client = services.GetRequiredService<DiscordSocketClient>();

        await services.GetRequiredService<InteractionHandler>().InitializeAsync();

        client.Log += async (msg) =>
        {
            await Task.CompletedTask;
            Console.WriteLine(msg);
        };

        await client.LoginAsync(TokenType.Bot, appSettings.DiscordToken);
        await client.StartAsync();

        await Task.Delay(Timeout.Infinite);
    }
}