using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using IllnessesRecordingSystem.ViewModels;

namespace IllnessesRecordingSystem.Views;

public partial class AddWindow : Window
{
    public AddWindow()
    {
        InitializeComponent();
        DataContext = new AddWindowViewModel();
    }
}