using StreamArch.Core.Models;

namespace StreamArch.Core.Interfaces;

public interface IPlaylistImporter
{
    List<Channel>? LoadPlaylist();
}