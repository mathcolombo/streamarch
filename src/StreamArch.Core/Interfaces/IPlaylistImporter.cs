using StreamArch.Core.Models;

namespace StreamArch.Core.Interfaces;

public interface IPlaylistImporter
{
    Task<List<Channel>?> LoadPlaylistAsync();
}