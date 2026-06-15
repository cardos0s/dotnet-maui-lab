namespace TaskApp.Maui;

public partial class App : Application
{
    private readonly TaskListPage _home;

    // A página inicial é INJETADA pelo container (já com a ViewModel dentro).
    // Demonstra a cadeia de DI: Repository -> ViewModel -> Page -> App.
    public App(Views.TaskListPage home)
    {
        InitializeComponent();
        _home = home;
    }

    protected override Window CreateWindow(IActivationState? activationState)
        => new(new NavigationPage(_home) { Title = "TaskApp" });
}
