using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using StreamArch.Core.Interfaces;
using StreamArch.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace StreamArch.Desktop.ViewModels;

// Mantemos o nome padrão do template e adicionamos a interface IDisposable
public partial class MainWindowViewModel : ViewModelBase, IDisposable
{
    private readonly LibVLC _libVlc;
    private readonly IPlaylistImporter _importer;

    // Construtor recebendo o nosso serviço de importação
    public MainWindowViewModel(IPlaylistImporter importer)
    {
        _importer = importer;

        // Inicializa o VLC nativo do Linux
        LibVLCSharp.Shared.Core.Initialize();
        _libVlc = new LibVLC();
        MediaPlayer = new MediaPlayer(_libVlc);
    }

    public MediaPlayer MediaPlayer { get; }

    [ObservableProperty]
    private ObservableCollection<Channel> _channels = new();

    [ObservableProperty]
    private Channel? _selectedChannel;

    partial void OnSelectedChannelChanged(Channel? value)
    {
        if (value != null) PlayChannel(value);
    }

    [RelayCommand]
    private async Task LoadPlaylistAsync()
    {
        var loadedChannels = await _importer.LoadPlaylistAsync();
        
        if (loadedChannels != null)
        {
            Channels.Clear();
            foreach (var channel in loadedChannels)
                Channels.Add(channel);
        }
    }

    private void PlayChannel(Channel channel)
    {
        using var media = new Media(_libVlc, new Uri(channel.Url));
        media.AddOption(":network-caching=1500");
        MediaPlayer.Play(media);
    }

    public void Dispose()
    {
        MediaPlayer.Dispose();
        _libVlc.Dispose();
    }
}