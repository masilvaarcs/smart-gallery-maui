namespace SmartGallery.Maui.Services;

public class SettingsService
{
    private const string ApiUrlKey = "api_url";
    private const string DefaultApiUrl = "https://localhost:5001";

    public string ApiUrl
    {
        get => Preferences.Get(ApiUrlKey, DefaultApiUrl);
        set => Preferences.Set(ApiUrlKey, value);
    }
}
