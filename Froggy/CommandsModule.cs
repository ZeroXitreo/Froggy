using Discord.Interactions;

namespace Froggy;

public class CommandsModule(PlaylistService playlistService) : InteractionModuleBase<SocketInteractionContext>
{
    private static readonly HttpClient httpClient = new()
    {
        BaseAddress = new Uri("https://inspirobot.me/")
    };

    private static readonly Random random = new();

    [SlashCommand("inspire", "Get an inspirational quote")]
    public async Task Inspire()
    {
        Console.WriteLine($"{Context.User.Username} used {nameof(Inspire)}");
        await DeferAsync();

        using HttpResponseMessage response = await httpClient.GetAsync("api?generate=true");
        string responseBody = await response.Content.ReadAsStringAsync();
        using HttpResponseMessage response2 = await httpClient.GetAsync(responseBody);
        await FollowupWithFileAsync(response2.Content.ReadAsStream(), "inspiration.jpg");
    }

    [SlashCommand("wednesday", "Is it wednesday yet?")]
    public async Task Wednesday()
    {
        Console.WriteLine($"{Context.User.Username} used {nameof(Inspire)}");
        await DeferAsync();

        if (DateTime.UtcNow.DayOfWeek == DayOfWeek.Wednesday)
        {
            var playlist = await playlistService.GetPlaylist();
            await FollowupAsync($"https://youtu.be/{playlist[random.Next(0, playlist.Count)].Snippet.ResourceId.VideoId}");
        }
        else
        {
            await FollowupAsync($"Not wednesday, sorry <@{Context.User.Id}>", ephemeral: true);
        }
    }

    //[SlashCommand("setmeup", "Set me up for a channel")]
    //public async Task SetMeUp()
    //{
    //    Console.WriteLine($"{Context.User.Username} used {nameof(SetMeUp)}");
    //}
}
