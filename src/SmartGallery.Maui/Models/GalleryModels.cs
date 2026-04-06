namespace SmartGallery.Maui.Models;

public record ImagemResumo(
    string Id,
    string Titulo,
    string Formato,
    long TamanhoBytes,
    string UrlThumbnail,
    DateTime DataUpload,
    List<string> Tags
);

public record ImagemDetalhe(
    string Id,
    string Titulo,
    string Descricao,
    List<string> Tags,
    string Formato,
    long TamanhoBytes,
    int Largura,
    int Altura,
    string UrlAssinada,
    string UrlThumbnail,
    string UsuarioId,
    DateTime DataUpload,
    bool Publica
);

public record UploadResponse(
    string Id,
    string Titulo,
    string Url,
    string Formato,
    long TamanhoBytes,
    DateTime DataUpload,
    List<string> TagsIa
);

public record ListagemResponse(
    List<ImagemResumo> Imagens,
    int Total,
    string? ProximoToken
);

public record GaleriaStats(
    int TotalImagens,
    long TotalBytes,
    Dictionary<string, int> PorFormato,
    List<string> TagsPopulares
);
