using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartGallery.Maui.Models;
using SmartGallery.Maui.Services;

namespace SmartGallery.Maui.ViewModels;

public partial class GalleryViewModel : ObservableObject
{
    private readonly GalleryApiService _api;

    public GalleryViewModel(GalleryApiService api)
    {
        _api = api;
    }

    [ObservableProperty]
    private ObservableCollection<ImagemResumo> _imagens = [];

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private string _statusText = string.Empty;

    private string? _proximoToken;

    [RelayCommand]
    private async Task CarregarAsync()
    {
        if (IsLoading) return;
        IsLoading = true;
        StatusText = "Carregando...";

        try
        {
            var result = await _api.ListarImagensAsync(limite: 50);
            Imagens = new ObservableCollection<ImagemResumo>(result.Imagens);
            _proximoToken = result.ProximoToken;
            IsEmpty = Imagens.Count == 0;
            StatusText = $"{result.Total} imagens encontradas";
        }
        catch
        {
            StatusText = "Nao foi possivel carregar a galeria agora.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task BuscarAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            await CarregarAsync();
            return;
        }

        IsLoading = true;
        StatusText = "Buscando...";

        try
        {
            var result = await _api.BuscarPorTagsAsync(SearchText);
            Imagens = new ObservableCollection<ImagemResumo>(result);
            IsEmpty = Imagens.Count == 0;
            StatusText = $"{Imagens.Count} resultado(s) para \"{SearchText}\"";
        }
        catch
        {
            StatusText = "Nao foi possivel concluir a busca.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task CarregarMaisAsync()
    {
        if (IsLoading || string.IsNullOrEmpty(_proximoToken)) return;
        IsLoading = true;

        try
        {
            var result = await _api.ListarImagensAsync(limite: 20, token: _proximoToken);
            foreach (var img in result.Imagens)
                Imagens.Add(img);
            _proximoToken = result.ProximoToken;
            StatusText = $"{Imagens.Count} de {result.Total} imagens";
        }
        catch
        {
            StatusText = "Nao foi possivel carregar mais itens.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task AbrirDetalheAsync(ImagemResumo imagem)
    {
        await Shell.Current.GoToAsync($"detalhe?id={imagem.Id}");
    }
}
