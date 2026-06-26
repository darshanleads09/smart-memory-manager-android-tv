using Microsoft.Extensions.DependencyInjection;

namespace SmartMemoryManager.UI;

public partial class App : Microsoft.Maui.Controls.Application
{
    private readonly IServiceProvider serviceProvider;

    public App(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(serviceProvider.GetRequiredService<MainPage>());
    }
}
