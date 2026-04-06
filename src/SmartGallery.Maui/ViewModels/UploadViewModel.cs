using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartGallery.Maui.Models;
using SmartGallery.Maui.Services;

namespace SmartGallery.Maui.ViewModels;

public partial class UploadViewModel : ObservableObject
{
    private readonly GalleryApiService _api;

    public UploadViewModel(GalleryApiService api)
    {
        _api = api;
    }

    [ObservableProperty]
    private string _titulo = string.Empty;

    [ObservableProperty]
    private string _tagsTexto = string.Empty;

    [ObservableProperty]
    private string? _arquivoSelecionado;

    [ObservableProperty]
    private ImageSource? _previewImagem;

    [ObservableProperty]
    private bool _isUploading;

    [ObservableProperty]
    private string _statusText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<string> _tagsIa = [];

    [ObservableProperty]
    private bool _uploadConcluido;

    private FileResult? _fileResult;

    [RelayCommand]
    private async Task SelecionarImagemAsync()
    {
        var result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Selecionar imagem",
            FileTypes = FilePickerFileType.Images
        });

        if (result is null) return;

        _fileResult = result;
        ArquivoSelecionado = result.FileName;
        PreviewImagem = ImageSource.FromFile(result.FullPath);

        if (string.IsNullOrWhiteSpace(Titulo))
            Titulo = Path.GetFileNameWithoutExtension(result.FileName);
    }

    [RelayCommand]
    private async Task TirarFotoAsync()
    {
        if (!MediaPicker.IsCaptureSupported)
        {
            await Shell.Current.DisplayAlert("Erro", "Câmera não suportada.", "OK");
            return;
        }

        var photo = await MediaPicker.CapturePhotoAsync();
        if (photo is null) return;

        _fileResult = photo;
        ArquivoSelecionado = photo.FileName;
        PreviewImagem = ImageSource.FromFile(photo.FullPath);

        if (string.IsNullOrWhiteSpace(Titulo))
            Titulo = $"foto_{DateTime.Now:yyyyMMdd_HHmmss}";
    }

    [RelayCommand]
    private async Task EnviarAsync()
    {
        if (_fileResult is null)
        {
            await Shell.Current.DisplayAlert("Atenção", "Selecione uma imagem.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(Titulo))
        {
            await Shell.Current.DisplayAlert("Atenção", "Informe um título.", "OK");
            return;
        }

        IsUploading = true;
        StatusText = "Enviando imagem...";
        UploadConcluido = false;

        try
        {
            var tags = string.IsNullOrWhiteSpace(TagsTexto)
                ? null
                : TagsTexto.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

            using var stream = await _fileResult.OpenReadAsync();
            var response = await _api.UploadAsync(stream, _fileResult.FileName, Titulo, tags);

            if (response is not null)
            {
                TagsIa = new ObservableCollection<string>(response.TagsIa);
                StatusText = $"Upload concluído! Rekognition detectou {response.TagsIa.Count} tag(s) IA.";
                UploadConcluido = true;
            }
        }
        catch (Exception ex)
        {
            StatusText = $"Erro: {ex.Message}";
        }
        finally
        {
            IsUploading = false;
        }
    }

    [RelayCommand]
    private void LimparFormulario()
    {
        Titulo = string.Empty;
        TagsTexto = string.Empty;
        ArquivoSelecionado = null;
        PreviewImagem = null;
        StatusText = string.Empty;
        TagsIa = [];
        UploadConcluido = false;
        _fileResult = null;
    }
}
