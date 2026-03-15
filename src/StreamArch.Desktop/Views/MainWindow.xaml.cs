using System.Windows;
using StreamArch.Desktop.Services;
using StreamArch.Desktop.ViewModels;

namespace StreamArch.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var importerService = new FilePlaylistImporter();
        var viewModel = new MainViewModel(importerService);
        DataContext = viewModel;
    }
}