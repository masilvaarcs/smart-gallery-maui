using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartGallery.Maui.Services;

namespace SmartGallery.Maui.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly SettingsService _settings;
    private readonly GalleryApiService _api;

    public SettingsViewModel(SettingsService settings, GalleryApiService api)
    {
        _settings = settings;
        _api = api;
        _apiUrl = settings.ApiUrl;
    }

    [ObservableProperty]
    private string _apiUrl;

    [ObservableProperty]
    private string _statusText = string.Empty;

    [ObservableProperty]
    private bool _isTesting;

    [ObservableProperty]
    private bool _isConnected;

    [RelayCommand]
    private async Task TestarConexaoAsync()
    {
        IsTesting = true;
        StatusText = "Testando conexão...";
        IsConnected = false;

        try
        {
            var stats = await _api.ObterStatsAsync();
            IsConnected = stats is not null;
            StatusText = IsConnected
                ? $"Conectado! {stats!.TotalImagens} imagens na galeria."
                : "Falha: resposta vazia.";
        }
        catch (Exception ex)
        {
            StatusText = $"Falha: {ex.Message}";
        }
        finally
        {
            IsTesting = false;
        }
    }

    [RelayCommand]
    private async Task SalvarAsync()
    {
        if (string.IsNullOrWhiteSpace(ApiUrl))
        {
            await Shell.Current.DisplayAlert("Atenção", "Informe a URL da API.", "OK");
            return;
        }

        // Validate URL format
        if (!Uri.TryCreate(ApiUrl.TrimEnd('/'), UriKind.Absolute, out var uri)
            || (uri.Scheme != "https" && uri.Scheme != "http"))
        {
            await Shell.Current.DisplayAlert("Atenção", "URL inválida. Use https://...", "OK");
            return;
        }

        _settings.ApiUrl = ApiUrl.TrimEnd('/');
        StatusText = "Salvo! Reinicie o app para aplicar.";
    }
}
