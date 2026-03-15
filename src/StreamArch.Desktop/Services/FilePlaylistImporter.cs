using System.IO;
using Microsoft.Win32;
using StreamArch.Core.Interfaces;
using StreamArch.Core.Models;
using StreamArch.Core.Services;

namespace StreamArch.Desktop.Services;

public class FilePlaylistImporter : IPlaylistImporter
{
    public List<Channel>? LoadPlaylist()
    {
        var dialog = new OpenFileDialog()
        {
            Filter = "Listas IPTV (*.m3u;*.m3u8)|*.m3u;*.m3u8",
            Title = "Selecione sua lista de canais"
        };

        if (dialog.ShowDialog() != true)
            return null;
        
        string content = File.ReadAllText(dialog.FileName);
        return M3uParser.Parse(content);
    }
}