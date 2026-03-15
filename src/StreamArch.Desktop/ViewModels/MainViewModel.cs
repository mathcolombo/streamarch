using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using StreamArch.Core.Interfaces;
using StreamArch.Core.Models;

namespace StreamArch.Desktop.ViewModels;

public partial class MainViewModel : ObservableObject, IDisposable
{
    private readonly LibVLC _libVlc;
    private readonly IPlaylistImporter _importer; // O Serviço injetado

    // CONSTRUTOR: Agora exigimos um IPlaylistImporter
    public MainViewModel(IPlaylistImporter importer)
    {
        _importer = importer; // Guardamos para usar depois

        LibVLCSharp.Shared.Core.Initialize();
        _libVlc = new LibVLC();
        MediaPlayer = new MediaPlayer(_libVlc);
    }

    public MediaPlayer MediaPlayer { get; }

    [ObservableProperty]
    private ObservableCollection<Channel> _channels = [];

    [ObservableProperty]
    private Channel? _selectedChannel;

    partial void OnSelectedChannelChanged(Channel? value)
    {
        if (value != null) PlayChannel(value);
    }

    [RelayCommand]
    private void LoadPlaylist()
    {
        // A ViewModel não sabe que isso abre uma janela. 
        // Ela só pede: "Importador, me dê a lista".
        var loadedChannels = _importer.LoadPlaylist();

        if (loadedChannels != null)
        {
            Channels.Clear();
            foreach (var channel in loadedChannels)
            {
                Channels.Add(channel);
            }
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