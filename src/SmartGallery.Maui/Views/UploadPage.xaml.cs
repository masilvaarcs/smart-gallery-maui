using SmartGallery.Maui.ViewModels;

namespace SmartGallery.Maui.Views;

public partial class UploadPage : ContentPage
{
    public UploadPage(UploadViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
