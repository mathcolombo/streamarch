using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using StreamArch.Core.Interfaces;
using StreamArch.Core.Models;

namespace StreamArch.Desktop.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _router;
    private readonly IPlaylistImporter _importer;

    public HomeViewModel(
        MainWindowViewModel router,
        IPlaylistImporter importer)
    {
        _router = router;
        _importer = importer;
    }

    [RelayCommand]
    private async Task LoadFileAsync()
    {
        List<Channel>? channels = await _importer.LoadPlaylistAsync();

        if (channels == null || channels.Count == 0)
        {
            Console.WriteLine("Nenhum canal foi carregado!");
            return;
        }
        
        Console.WriteLine($"Sucesso! {channels.Count} canais carregados.");
        _router.NavigateTo(new PlayerViewModel(_router, _importer, channels));
    }
}