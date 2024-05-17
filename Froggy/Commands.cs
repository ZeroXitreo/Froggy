using Discord.Interactions;

namespace Froggy;
public class Commands : InteractionModuleBase
{
	private static readonly HttpClient httpClient = new()
	{
		BaseAddress = new Uri("https://inspirobot.me/")
	};

	[SlashCommand("inspire", "Get an inspirational quote")]
	public async Task Inspire()
	{
		await DeferAsync();
		using HttpResponseMessage response = await httpClient.GetAsync("api?generate=true");
		string responseBody = await response.Content.ReadAsStringAsync();
		using HttpResponseMessage response2 = await httpClient.GetAsync(responseBody);
		await FollowupWithFileAsync(response2.Content.ReadAsStream(), "inspiration.jpg");
	}
}
