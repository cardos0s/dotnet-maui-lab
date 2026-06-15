namespace Christmas;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(CategoriesPage), typeof(CategoriesPage));
        Routing.RegisterRoute(nameof(GiftOptionsPage), typeof(GiftOptionsPage));         
        Routing.RegisterRoute(nameof(GiftDetailsPage), typeof(GiftDetailsPage));         

    }
}