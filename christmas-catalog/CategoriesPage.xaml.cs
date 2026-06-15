using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Christmas;
public class Category
{
    public string ImageSource { get; set; } // Nome do arquivo (ex: "christmas_balls.png")
    public string Name { get; set; }       // Nome da categoria
    public int ItemCount { get; set; }     // Contagem de itens
    public string DisplayCount => $"{ItemCount} items"; // Exibe "20 items"
}

public partial class CategoriesPage : ContentPage
{
    public ObservableCollection<Category> Categories { get; set; }
    
    public CategoriesPage()
    {
        InitializeComponent();

        Categories = new ObservableCollection<Category>
        {
            new Category { Name = "Muffins", ImageSource = "muffins.png", ItemCount = 20 },
            new Category { Name = "Christmas sweaters", ImageSource = "christmas_sweaters.png", ItemCount = 8 },
            new Category { Name = "Christmas sweets", ImageSource = "christmas_sweets.png", ItemCount = 20 },
            new Category { Name = "Christmas socks", ImageSource = "christmas_socks.png", ItemCount = 17 },
            new Category { Name = "Christmas socks", ImageSource = "christmas_socks.png", ItemCount = 17 }
        };
        this.BindingContext = this;
    }

    private async void Button_OnClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("GiftOptionsPage"); 
    }

    private async void CategoriaTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("GiftDetailsPage");
    }
}