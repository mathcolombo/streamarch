using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using StreamArch.Core.Interfaces;
using StreamArch.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace StreamArch.Desktop.ViewModels;

public class ChannelGroup
{
    public string Name { get; set; } = string.Empty;
    public ObservableCollection<Channel> Channels { get; set; } = [];
}

public partial class PlayerViewModel : ViewModelBase, IDisposable
{
    private readonly MainWindowViewModel _router;
    private readonly IPlaylistImporter _importer;
    private readonly LibVLC _libVlc;

    public MediaPlayer MediaPlayer { get; }

    public ObservableCollection<ChannelGroup> GroupedChannels { get; } = [];

    [ObservableProperty] private bool _isSidebarVisible = true;
    [ObservableProperty] private bool _isLoading = false;
    [ObservableProperty] private string _currentChannelName = "Nenhum canal selecionado";

    public PlayerViewModel(MainWindowViewModel router, IPlaylistImporter importer, List<Channel> rawChannels)
    {
        _router = router;
        _importer = importer;

        MakeGroupedChannels(rawChannels);

        LibVLCSharp.Shared.Core.Initialize();
        _libVlc = new LibVLC("--avcodec-hw=none", "--vout=x11");
        MediaPlayer = new MediaPlayer(_libVlc);

        PlayReadyObserver();
    }

    private void MakeGroupedChannels(List<Channel> channels)
    {
        IOrderedEnumerable<IGrouping<string, Channel>> groups = GroupChannels(channels);
        AddGroupedChannels(groups);
    }

    private IOrderedEnumerable<IGrouping<string, Channel>> GroupChannels(List<Channel> channels) => channels
        .GroupBy(c => string.IsNullOrWhiteSpace(c.Group) ? "Sem Categoria" : c.Group)
        .OrderBy(g => g.Key);

    private void AddGroupedChannels(IOrderedEnumerable<IGrouping<string, Channel>> groups)
    {
        foreach (IGrouping<string, Channel> group in groups)
        {
            GroupedChannels.Add(new ChannelGroup
            {
                Name = group.Key,
                Channels = new ObservableCollection<Channel>(group)
            });
        }
    }

    private void PlayReadyObserver() =>
        MediaPlayer.Playing += (sender, args) =>
        {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => 
            {
                IsLoading = false;
            });
        };

    [RelayCommand]
    private void PlayChannel(Channel channel)
    {
        // 3. Ao clicar no canal, ativa o Loading e atualiza o título
        IsLoading = true;
        CurrentChannelName = $"Assistindo: {channel.Name}";

        using var media = new Media(_libVlc, new Uri(channel.Url));
        media.AddOption(":network-caching=2000"); // Aumentei o cache para 2 segundos para estabilizar
        MediaPlayer.Play(media);
    }

    [RelayCommand]
    private void ToggleSidebar() => IsSidebarVisible = !IsSidebarVisible;

    [RelayCommand]
    private void GoBack()
    {
        _router.NavigateTo(new HomeViewModel(_router, _importer)); // Volta à Home
        Dispose(); // Para o vídeo e liberta a memória RAM
    }

    public void Dispose()
    {
        MediaPlayer.Stop();
        MediaPlayer.Dispose();
        _libVlc.Dispose();
    }
}