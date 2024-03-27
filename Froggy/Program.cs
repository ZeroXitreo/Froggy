using Discord;
using Discord.WebSocket;
using Froggy;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Newtonsoft.Json;

public class Program
{
	public static Task Main() => new Program().MainAsync();

	private AppSettings? appSettings;

	private static readonly DiscordSocketConfig config = new()
	{
		GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
	};

	private readonly DiscordSocketClient client = new(config);

	private IList<PlaylistItem> playlist = [];

	private static readonly Random random = new();

	private static readonly HttpClient httpClient = new() { BaseAddress = new Uri("https://inspirobot.me/") };

	public async Task MainAsync()
	{
		appSettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText("appsettings.json"));
		await PopulatePlaylistAsync();

		client.MessageReceived += MessageReceived;
		await client.LoginAsync(TokenType.Bot, appSettings?.DiscordToken);
		await client.StartAsync();
		await Task.Delay(-1);
	}

	private async Task PopulatePlaylistAsync()
	{
		YouTubeService service = new(new BaseClientService.Initializer()
		{
			ApiKey = appSettings?.YtApiKey,
		});

		PlaylistItemsResource.ListRequest playlistRequest = service.PlaylistItems.List("snippet");
		playlistRequest.PlaylistId = appSettings?.YtPlaylist;
		playlistRequest.MaxResults = 50;
		PlaylistItemListResponse playlistResponse = await playlistRequest.ExecuteAsync();
		playlist = playlistResponse.Items;
	}

	private async Task MessageReceived(SocketMessage message)
	{
		Console.WriteLine(message.Content);
		Console.WriteLine(message.ToString());
		if (message.Author == client.CurrentUser) return;

		await Message(message);

		if (message.MentionedUsers.Any(user => user.Id == client.CurrentUser.Id))
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

	private async Task Message(SocketMessage message)
	{
		if (message.Content == "!inspireme")
		{
			await message.Channel.TriggerTypingAsync();
			using HttpResponseMessage response = await httpClient.GetAsync("api?generate=true");
			string responseBody = await response.Content.ReadAsStringAsync();
			using HttpResponseMessage response2 = await httpClient.GetAsync(responseBody);
			await message.Channel.SendFileAsync(response2.Content.ReadAsStream(), "inspiration.jpg");
		}
	}
}
