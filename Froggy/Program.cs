using Discord;
using Discord.WebSocket;
using Froggy;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Newtonsoft.Json;

public class Program
{
	public static Task Main(string[] args) => new Program().MainAsync();

	private AppSettings? appSettings;

	private DiscordSocketClient _client;

	private IList<PlaylistItem> _playlist;

	private static Random _random = new Random();

	public async Task MainAsync()
	{
		appSettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText("appsettings.json"));
		await PopulatePlaylistAsync();
		Console.WriteLine(_playlist.Count);

		_client = new DiscordSocketClient();

		_client.Log += Log;
		_client.ChannelCreated += Womba;
		_client.MessageReceived += MessageReceived;
		_client.Ready += Ready;
		_client.UserVoiceStateUpdated += UserVoiceStateUpdated;
		_client.MessageReceived += Message;


		var token = appSettings.DiscordToken;

		await _client.LoginAsync(TokenType.Bot, token);
		await _client.StartAsync();

		await Task.Delay(-1);
	}

	private async Task UserVoiceStateUpdated(SocketUser User, SocketVoiceState Leaving, SocketVoiceState Joining)
	{
		if (Leaving.VoiceChannel is not null)
		{
			if (!Leaving.VoiceChannel.ConnectedUsers.Any())
			{
				await Leaving.VoiceChannel.Guild.CreateVoiceChannelAsync(Leaving.VoiceChannel.Name, tcp => tcp.CategoryId = Leaving.VoiceChannel.CategoryId);
				await PurgeTextChannel(Leaving.VoiceChannel);
			}
		}
	}

	private async Task PurgeTextChannel(ITextChannel textChannel)
	{
		IEnumerable<IMessage> messages;
		do
		{
			messages = await textChannel.GetMessagesAsync().FlattenAsync();
			await textChannel.DeleteMessagesAsync(messages);
		} while (messages.Any());
	}

	private async Task Ready()
	{
		IUser foo = await _client.GetUserAsync(66356975771852800);
		//await foo.SendMessageAsync("Hoi!");
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
		_playlist = playlistResponse.Items;
	}

	private async Task MessageReceived(SocketMessage arg)
	{
		if (arg.Author == _client.CurrentUser) return;

		if (arg.MentionedUsers.Any(o => o.Id == _client.CurrentUser.Id))
		{
			if (DateTime.UtcNow.DayOfWeek == DayOfWeek.Wednesday)
			{
				//await arg.DeleteAsync();
				await arg.Channel.SendMessageAsync($"https://youtu.be/{_playlist[_random.Next(0, _playlist.Count)].Snippet.ResourceId.VideoId}");
			}
		}
	}

	private Task Womba(SocketChannel arg)
	{
		Console.WriteLine(arg);
		return Task.CompletedTask;
	}

	private Task Log(LogMessage msg)
	{
		Console.WriteLine(msg.ToString());
		return Task.CompletedTask;
	}

	private Task Message(SocketMessage msg)
	{
		Console.WriteLine(msg.ToString());
		return Task.CompletedTask;
	}
}
