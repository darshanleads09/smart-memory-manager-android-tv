using SmartMemoryManager.UI.ViewModels;

namespace SmartMemoryManager.UI;

public partial class MainPage : ContentPage
{
    private readonly DashboardViewModel viewModel;

    public MainPage(DashboardViewModel viewModel)
    {
        this.viewModel = viewModel;
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await viewModel.ScanAsync();
    }
}
