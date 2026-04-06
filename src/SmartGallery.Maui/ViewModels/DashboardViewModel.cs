using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartGallery.Maui.Models;
using SmartGallery.Maui.Services;

namespace SmartGallery.Maui.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly GalleryApiService _api;

    public DashboardViewModel(GalleryApiService api)
    {
        _api = api;
    }

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private int _totalImagens;

    [ObservableProperty]
    private string _totalArmazenamento = "0 B";

    [ObservableProperty]
    private int _totalFormatos;

    [ObservableProperty]
    private List<FormatoInfo> _formatos = [];

    [ObservableProperty]
    private List<TagInfo> _tagsPopulares = [];

    [ObservableProperty]
    private string _statusText = string.Empty;

    [RelayCommand]
    private async Task CarregarAsync()
    {
        if (IsLoading) return;
        IsLoading = true;
        StatusText = "Carregando estatísticas...";

        try
        {
            var stats = await _api.ObterStatsAsync();
            if (stats is null)
            {
                StatusText = "Sem dados disponíveis";
                return;
            }

            TotalImagens = stats.TotalImagens;
            TotalArmazenamento = FormatarBytes(stats.TotalBytes);
            TotalFormatos = stats.PorFormato.Count;

            var totalPorFormato = stats.PorFormato.Values.Sum();
            Formatos = stats.PorFormato
                .OrderByDescending(f => f.Value)
                .Select(f => new FormatoInfo(
                    f.Key.ToUpperInvariant(),
                    f.Value,
                    totalPorFormato > 0 ? (double)f.Value / totalPorFormato : 0,
                    ObterCorFormato(f.Key)))
                .ToList();

            TagsPopulares = stats.TagsPopulares
                .Select((t, i) => new TagInfo(t, i + 1))
                .ToList();

            StatusText = $"Atualizado em {DateTime.Now:HH:mm}";
        }
        catch (Exception ex)
        {
            StatusText = $"Erro: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private static string FormatarBytes(long bytes)
    {
        string[] unidades = ["B", "KB", "MB", "GB"];
        double tamanho = bytes;
        int i = 0;
        while (tamanho >= 1024 && i < unidades.Length - 1)
        {
            tamanho /= 1024;
            i++;
        }
        return $"{tamanho:F1} {unidades[i]}";
    }

    private static Color ObterCorFormato(string formato) => formato.ToLowerInvariant() switch
    {
        "jpeg" or "jpg" => Color.FromArgb("#4CAF50"),
        "png" => Color.FromArgb("#2196F3"),
        "gif" => Color.FromArgb("#FF9800"),
        "webp" => Color.FromArgb("#9C27B0"),
        _ => Color.FromArgb("#607D8B")
    };
}

public record FormatoInfo(string Nome, int Quantidade, double Percentual, Color Cor);
public record TagInfo(string Nome, int Posicao);
