using SmartGallery.Maui.ViewModels;

namespace SmartGallery.Maui.Views;

public partial class GalleryPage : ContentPage
{
    public GalleryPage(GalleryViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is GalleryViewModel vm)
            vm.CarregarCommand.Execute(null);
    }
}
