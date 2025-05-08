using MakeWish.Desktop.ViewModels;

namespace MakeWish.Desktop.Views;

public partial class MainWindow
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}