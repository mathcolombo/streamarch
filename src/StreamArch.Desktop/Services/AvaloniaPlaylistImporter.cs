using Avalonia.Platform.Storage;
using StreamArch.Core.Interfaces;
using StreamArch.Core.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using StreamArch.Core.Services;

namespace StreamArch.Desktop.Services;

public class AvaloniaPlaylistImporter : IPlaylistImporter
{
    private readonly IStorageProvider _storageProvider;

    public AvaloniaPlaylistImporter(IStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
    }

    public async Task<List<Channel>?> LoadPlaylistAsync()
    {
        var files = await _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Selecione sua lista M3U",
            AllowMultiple = false
        });

        if (files.Count <= 0) return null;
        
        await using Stream stream = await files[0].OpenReadAsync();
        using var reader = new StreamReader(stream);
            
        string content = await reader.ReadToEndAsync();
            
        return M3UParser.Parse(content);
    }
}