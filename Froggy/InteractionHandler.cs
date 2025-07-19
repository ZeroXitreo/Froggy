using System.Reflection;
using Discord.Interactions;
using Discord.WebSocket;

namespace Froggy;

public class InteractionHandler(DiscordSocketClient client, InteractionService commands, IServiceProvider services)
{
    public async Task InitializeAsync()
    {
        client.Ready += ReadyAsync;
        client.InteractionCreated += HandleInteraction;

        await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
    }

    private async Task ReadyAsync()
    {
        await commands.RegisterCommandsGloballyAsync(true);
        Console.WriteLine("Registered commands:");
        foreach (var command in commands.SlashCommands)
            Console.WriteLine(command.Name);
    }

    private async Task HandleInteraction(SocketInteraction arg)
    {
        var ctx = new SocketInteractionContext(client, arg);
        await commands.ExecuteCommandAsync(ctx, services);
    }
}
