using System.Net;
using System.Text.Json;

namespace SmartGallery.Maui.Tests;

#region Models (replicated for test isolation - same contract as API)
public record ImagemResumo(string Id, string Titulo, string Formato, long TamanhoBytes, string UrlThumbnail, DateTime DataUpload, List<string> Tags);
public record ImagemDetalhe(string Id, string Titulo, string Descricao, List<string> Tags, string Formato, long TamanhoBytes, int Largura, int Altura, string UrlAssinada, string UrlThumbnail, DateTime DataUpload, bool Publica);
public record UploadResponse(string Id, string Titulo, string Url, string Formato, long TamanhoBytes, DateTime DataUpload, List<string> TagsIa);
public record ListagemResponse(List<ImagemResumo> Imagens, int Total, string? ProximoToken);
public record GaleriaStats(int TotalImagens, long TotalBytes, Dictionary<string, int> PorFormato, List<string> TagsPopulares);
#endregion

public class MockHttpHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

    public MockHttpHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
    {
        _handler = handler;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        => Task.FromResult(_handler(request));
}

public class GalleryApiServiceTests
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private static HttpClient CreateMockClient(string responseJson, HttpStatusCode status = HttpStatusCode.OK)
    {
        var handler = new MockHttpHandler(_ => new HttpResponseMessage(status)
        {
            Content = new StringContent(responseJson, System.Text.Encoding.UTF8, "application/json")
        });
        return new HttpClient(handler) { BaseAddress = new Uri("https://test.api.com/") };
    }

    [Fact]
    public async Task ListarImagens_DeveDesserializarCorretamente()
    {
        var expected = new ListagemResponse(
            [new("1", "Teste", "jpeg", 1024, "https://thumb.jpg", DateTime.UtcNow, ["tag1"])],
            1, null);

        var json = JsonSerializer.Serialize(expected);
        var http = CreateMockClient(json);

        var result = await http.GetAsync("api/imagens?limite=50");
        var body = await result.Content.ReadAsStringAsync();
        var parsed = JsonSerializer.Deserialize<ListagemResponse>(body, JsonOptions);

        Assert.NotNull(parsed);
        Assert.Equal(1, parsed.Total);
        Assert.Single(parsed.Imagens);
        Assert.Equal("Teste", parsed.Imagens[0].Titulo);
    }

    [Fact]
    public async Task ObterDetalhe_DeveTerTodasAsPropriedades()
    {
        var detalhe = new ImagemDetalhe("1", "Foto", "Uma foto", ["sol", "praia"],
            "jpeg", 2048, 1920, 1080, "https://signed.jpg", "https://thumb.jpg",
            DateTime.UtcNow, true);

        var json = JsonSerializer.Serialize(detalhe);
        var http = CreateMockClient(json);

        var result = await http.GetAsync("api/imagens/1");
        var body = await result.Content.ReadAsStringAsync();
        var parsed = JsonSerializer.Deserialize<ImagemDetalhe>(body, JsonOptions);

        Assert.NotNull(parsed);
        Assert.Equal(1920, parsed.Largura);
        Assert.Equal(1080, parsed.Altura);
        Assert.Contains("sol", parsed.Tags);
        Assert.True(parsed.Publica);
    }

    [Fact]
    public async Task Upload_DeveRetornarTagsIa()
    {
        var response = new UploadResponse("1", "Novo", "https://img.jpg", "png",
            4096, DateTime.UtcNow, ["landscape", "mountain", "sky"]);

        var json = JsonSerializer.Serialize(response);
        var http = CreateMockClient(json);

        var result = await http.PostAsync("api/imagens", new StringContent("fake"));
        var body = await result.Content.ReadAsStringAsync();
        var parsed = JsonSerializer.Deserialize<UploadResponse>(body, JsonOptions);

        Assert.NotNull(parsed);
        Assert.Equal(3, parsed.TagsIa.Count);
        Assert.Contains("landscape", parsed.TagsIa);
    }

    [Fact]
    public async Task Stats_DeveTerFormatosETags()
    {
        var stats = new GaleriaStats(10, 50000,
            new Dictionary<string, int> { ["jpeg"] = 6, ["png"] = 4 },
            ["natureza", "cidade", "retrato"]);

        var json = JsonSerializer.Serialize(stats);
        var http = CreateMockClient(json);

        var result = await http.GetAsync("api/imagens/stats");
        var body = await result.Content.ReadAsStringAsync();
        var parsed = JsonSerializer.Deserialize<GaleriaStats>(body, JsonOptions);

        Assert.NotNull(parsed);
        Assert.Equal(10, parsed.TotalImagens);
        Assert.Equal(2, parsed.PorFormato.Count);
        Assert.Equal(3, parsed.TagsPopulares.Count);
    }

    [Fact]
    public void ListagemVazia_DeveRetornarListaVazia()
    {
        var json = """{"imagens":[],"total":0,"proximoToken":null}""";
        var parsed = JsonSerializer.Deserialize<ListagemResponse>(json, JsonOptions);

        Assert.NotNull(parsed);
        Assert.Empty(parsed.Imagens);
        Assert.Equal(0, parsed.Total);
        Assert.Null(parsed.ProximoToken);
    }

    [Fact]
    public void TagsIa_PodeSerListaVazia()
    {
        var json = """{"id":"1","titulo":"Sem IA","url":"https://x.jpg","formato":"jpeg","tamanhoBytes":100,"dataUpload":"2026-01-01T00:00:00Z","tagsIa":[]}""";
        var parsed = JsonSerializer.Deserialize<UploadResponse>(json, JsonOptions);

        Assert.NotNull(parsed);
        Assert.Empty(parsed.TagsIa);
    }

    [Fact]
    public void FormatarBytes_DeveConverterCorretamente()
    {
        Assert.EndsWith("B", FormatarBytes(0));
        Assert.Contains("KB", FormatarBytes(1024));
        Assert.Contains("MB", FormatarBytes(1572864));
        Assert.Contains("GB", FormatarBytes(2147483648));
    }

    private static string FormatarBytes(long bytes)
    {
        string[] unidades = ["B", "KB", "MB", "GB"];
        double tamanho = bytes;
        int i = 0;
        while (tamanho >= 1024 && i < unidades.Length - 1) { tamanho /= 1024; i++; }
        return $"{tamanho:F1} {unidades[i]}";
    }

    [Fact]
    public void Paginacao_DeveConterToken()
    {
        var json = """{"imagens":[{"id":"1","titulo":"A","formato":"jpg","tamanhoBytes":100,"urlThumbnail":"x","dataUpload":"2026-01-01T00:00:00Z","tags":[]}],"total":50,"proximoToken":"abc123"}""";
        var parsed = JsonSerializer.Deserialize<ListagemResponse>(json, JsonOptions);

        Assert.NotNull(parsed);
        Assert.Equal("abc123", parsed.ProximoToken);
        Assert.Equal(50, parsed.Total);
    }

    [Fact]
    public void DetalheImagem_TagsDeveSerMergeManualEIa()
    {
        var tags = new List<string> { "manual-tag", "ia-landscape", "ia-sky" };
        var detalhe = new ImagemDetalhe("1", "X", "", tags, "png", 100, 800, 600,
            "url", "thumb", DateTime.UtcNow, true);

        Assert.Equal(3, detalhe.Tags.Count);
        Assert.Contains("manual-tag", detalhe.Tags);
        Assert.Contains("ia-landscape", detalhe.Tags);
    }

    [Fact]
    public void Stats_PorFormato_DeveCalcularPercentuais()
    {
        var stats = new GaleriaStats(100, 1000000,
            new Dictionary<string, int> { ["jpeg"] = 60, ["png"] = 30, ["gif"] = 10 },
            []);

        var totalPorFormato = stats.PorFormato.Values.Sum();
        var jpegPercent = (double)stats.PorFormato["jpeg"] / totalPorFormato;

        Assert.Equal(0.6, jpegPercent, 2);
        Assert.Equal(100, totalPorFormato);
    }
}
