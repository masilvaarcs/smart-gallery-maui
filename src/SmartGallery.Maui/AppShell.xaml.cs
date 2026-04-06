using SmartGallery.Maui.Views;

namespace SmartGallery.Maui;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute("detalhe", typeof(DetailPage));
	}
}
