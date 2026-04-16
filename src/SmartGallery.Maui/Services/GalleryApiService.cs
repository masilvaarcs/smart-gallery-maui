using System.Net.Http.Json;
using System.Text.Json;
using SmartGallery.Maui.Models;

namespace SmartGallery.Maui.Services;

public class GalleryApiService
{
    private readonly HttpClient _http;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public GalleryApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<ListagemResponse> ListarImagensAsync(int limite = 20, string? token = null, CancellationToken ct = default)
    {
        var url = $"api/imagens?limite={limite}";
        if (!string.IsNullOrEmpty(token))
            url += $"&token={Uri.EscapeDataString(token)}";

        var response = await _http.GetFromJsonAsync<ListagemResponse>(url, JsonOptions, ct);
        return response ?? new ListagemResponse([], 0, null);
    }

    public async Task<ImagemDetalhe?> ObterImagemAsync(string id, CancellationToken ct = default)
    {
        return await _http.GetFromJsonAsync<ImagemDetalhe>($"api/imagens/{Uri.EscapeDataString(id)}", JsonOptions, ct);
    }

    public async Task<List<ImagemResumo>> BuscarPorTagsAsync(string termo, CancellationToken ct = default)
    {
        var url = $"api/imagens/busca?termo={Uri.EscapeDataString(termo)}";
        var result = await _http.GetFromJsonAsync<ListagemResponse>(url, JsonOptions, ct);
        return result?.Imagens ?? [];
    }

    public async Task<GaleriaStats?> ObterStatsAsync(CancellationToken ct = default)
    {
        return await _http.GetFromJsonAsync<GaleriaStats>("api/imagens/stats", JsonOptions, ct);
    }

    public async Task<UploadResponse?> UploadAsync(Stream arquivo, string nomeArquivo, string titulo, List<string>? tags = null, bool publica = false, CancellationToken ct = default)
    {
        using var content = new MultipartFormDataContent();
        using var streamContent = new StreamContent(arquivo);
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
            ObterMimeType(nomeArquivo));

        content.Add(streamContent, "arquivo", nomeArquivo);
        content.Add(new StringContent(titulo), "titulo");
        content.Add(new StringContent(publica ? "true" : "false"), "publica");

        if (tags is { Count: > 0 })
        {
            foreach (var tag in tags)
                content.Add(new StringContent(tag), "tags");
        }

        var response = await _http.PostAsync("api/imagens", content, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<UploadResponse>(JsonOptions, ct);
    }

    public async Task<bool> DeletarAsync(string id, CancellationToken ct = default)
    {
        var response = await _http.DeleteAsync($"api/imagens/{Uri.EscapeDataString(id)}", ct);
        return response.IsSuccessStatusCode;
    }

    private static string ObterMimeType(string nomeArquivo)
    {
        var ext = Path.GetExtension(nomeArquivo).ToLowerInvariant();
        return ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".bmp" => "image/bmp",
            _ => "application/octet-stream"
        };
    }
}
