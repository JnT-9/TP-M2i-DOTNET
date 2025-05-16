using MauiApp1.ViewModels;

namespace MauiApp1.Views;

public partial class NewTaskPage : ContentPage
{
    private readonly NewTaskViewModel _viewModel;

    public NewTaskPage(NewTaskViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
} 