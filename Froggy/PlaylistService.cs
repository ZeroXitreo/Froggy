using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace Froggy;

public class PlaylistService(AppSettings appSettings)
{
    private List<PlaylistItem>? Playlist;
    internal async Task<List<PlaylistItem>> GetPlaylist()
    {
        if (Playlist == null)
        {
            YouTubeService service = new(new BaseClientService.Initializer()
            {
                ApiKey = appSettings.YtApiKey,
            });

            PlaylistItemsResource.ListRequest playlistRequest = service.PlaylistItems.List("snippet");
            playlistRequest.PlaylistId = appSettings.YtPlaylist;
            playlistRequest.MaxResults = 50;
            PlaylistItemListResponse playlistResponse = await playlistRequest.ExecuteAsync();

            Playlist = [.. playlistResponse.Items];
        }

        return Playlist;
    }
}
