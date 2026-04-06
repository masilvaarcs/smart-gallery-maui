using SmartGallery.Maui.ViewModels;

namespace SmartGallery.Maui.Views;

public partial class DetailPage : ContentPage
{
    public DetailPage(DetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
