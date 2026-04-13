using Avalonia.Controls;
using StreamArch.Desktop.Services;
using StreamArch.Desktop.ViewModels;

namespace StreamArch.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        var importer = new AvaloniaPlaylistImporter(this.StorageProvider);
        
        InitializeComponent();
        DataContext = new MainWindowViewModel(importer);
    }
}