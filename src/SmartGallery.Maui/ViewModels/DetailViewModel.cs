using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartGallery.Maui.Models;
using SmartGallery.Maui.Services;

namespace SmartGallery.Maui.ViewModels;

[QueryProperty(nameof(ImagemId), "id")]
public partial class DetailViewModel : ObservableObject
{
    private readonly GalleryApiService _api;

    public DetailViewModel(GalleryApiService api)
    {
        _api = api;
    }

    [ObservableProperty]
    private string _imagemId = string.Empty;

    [ObservableProperty]
    private ImagemDetalhe? _imagem;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _tamanhoFormatado = string.Empty;

    [ObservableProperty]
    private string _resolucao = string.Empty;

    [ObservableProperty]
    private string _tagsTexto = string.Empty;

    partial void OnImagemIdChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
            CarregarCommand.Execute(null);
    }

    [RelayCommand]
    private async Task CarregarAsync()
    {
        if (string.IsNullOrEmpty(ImagemId)) return;
        IsLoading = true;

        try
        {
            Imagem = await _api.ObterImagemAsync(ImagemId);
            if (Imagem is not null)
            {
                TamanhoFormatado = FormatarBytes(Imagem.TamanhoBytes);
                Resolucao = (Imagem.Largura > 0 && Imagem.Altura > 0)
                    ? $"{Imagem.Largura} × {Imagem.Altura} px"
                    : "N/D";
                TagsTexto = Imagem.Tags.Count > 0
                    ? string.Join(", ", Imagem.Tags)
                    : "Sem tags";
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erro", ex.Message, "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DeletarAsync()
    {
        if (Imagem is null) return;

        bool confirmar = await Shell.Current.DisplayAlert(
            "Confirmar exclusão",
            $"Deseja excluir \"{Imagem.Titulo}\"?",
            "Excluir", "Cancelar");

        if (!confirmar) return;

        try
        {
            var ok = await _api.DeletarAsync(Imagem.Id);
            if (ok)
            {
                await Shell.Current.DisplayAlert("Sucesso", "Imagem excluída.", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erro", ex.Message, "OK");
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
}
