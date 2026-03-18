using Avalonia.Controls;
using StreamArch.Desktop.Services;
using StreamArch.Desktop.ViewModels;

namespace StreamArch.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // 1. Cria o importador e passa o StorageProvider (Explorador de Arquivos do Linux)
        var importer = new AvaloniaPlaylistImporter(this.StorageProvider);
        
        // 2. Cria a ViewModel (com o nome do template) e injeta o importador
        var viewModel = new MainWindowViewModel(importer);

        // 3. Conecta a tela à lógica
        DataContext = viewModel;
    }
}