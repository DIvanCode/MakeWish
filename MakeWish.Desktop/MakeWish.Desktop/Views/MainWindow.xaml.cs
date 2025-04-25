using System.Windows;
using MakeWish.Desktop.ViewModels;

namespace MakeWish.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}