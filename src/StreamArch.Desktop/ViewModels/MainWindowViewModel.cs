using CommunityToolkit.Mvvm.ComponentModel;
using StreamArch.Core.Interfaces;

namespace StreamArch.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentPage;

    private readonly IPlaylistImporter _importer;
    
    public MainWindowViewModel(IPlaylistImporter importer)
    {
        _importer = importer;
        CurrentPage = new HomeViewModel(this, _importer); 
    }

    public void NavigateTo(ViewModelBase viewModel)
    {
        CurrentPage = viewModel;
    }
}