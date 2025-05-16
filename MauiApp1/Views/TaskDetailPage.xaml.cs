using MauiApp1.ViewModels;

namespace MauiApp1.Views;

public partial class TaskDetailPage : ContentPage
{
    private readonly TaskDetailViewModel _viewModel;

    public TaskDetailPage(TaskDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadTaskCommand.Execute(null);
    }
} 