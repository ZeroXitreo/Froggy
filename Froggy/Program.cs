using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Froggy;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Newtonsoft.Json;
using System.Reflection;

public class Program
{
	public static Task Main() => new Program().MainAsync();

	private readonly AppSettings appSettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText("appsettings.json"))!;

	private readonly DiscordSocketClient client = new(new()
	{
		GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
	});

	private IList<PlaylistItem> playlist = [];

	private static readonly Random random = new();

	public async Task MainAsync()
	{
		await PopulatePlaylistAsync();

		client.MessageReceived += MessageReceived;
		client.Ready += Ready;
		await client.LoginAsync(TokenType.Bot, appSettings.DiscordToken);
		await client.StartAsync();
		await Task.Delay(-1);
	}

	private async Task Ready()
	{
		InteractionService interactionService = new(client);
		await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), null);
		await interactionService.RegisterCommandsGloballyAsync();

		client.InteractionCreated += async interaction =>
		{
			var ctx = new SocketInteractionContext(client, interaction);
			await interactionService.ExecuteCommandAsync(ctx, null);
		};
	}

	private async Task PopulatePlaylistAsync()
	{
		YouTubeService service = new(new BaseClientService.Initializer()
		{
			ApiKey = appSettings.YtApiKey,
		});

		PlaylistItemsResource.ListRequest playlistRequest = service.PlaylistItems.List("snippet");
		playlistRequest.PlaylistId = appSettings.YtPlaylist;
		playlistRequest.MaxResults = 50;
		PlaylistItemListResponse playlistResponse = await playlistRequest.ExecuteAsync();
		playlist = playlistResponse.Items;
	}

	private async Task MessageReceived(SocketMessage message)
	{
		Console.WriteLine(message.Content);
		if (message.Author == client.CurrentUser) return;

		if (message.Content.Contains($"<@{client.CurrentUser.Id}>"))
		{
			if (DateTime.UtcNow.DayOfWeek == DayOfWeek.Wednesday)
			{
				await message.Channel.SendMessageAsync($"https://youtu.be/{playlist[random.Next(0, playlist.Count)].Snippet.ResourceId.VideoId}");
			}
			else
			{
				await message.Channel.SendMessageAsync($"Not wednesday, sorry <@{message.Author.Id}>");
			}
		}
	}
}
