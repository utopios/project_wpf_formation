using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ConfigDemo.Settings;

namespace ConfigDemo.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IConfiguration _configuration;
    private readonly AppSettings _appSettings;
    private readonly ApiSettings _apiSettings;
    private readonly FeatureFlags _featureFlags;

    public MainViewModel(
        IConfiguration configuration,
        IOptions<AppSettings> appSettings,
        IOptions<ApiSettings> apiSettings,
        IOptions<FeatureFlags> featureFlags)
    {
        _configuration = configuration;
        _appSettings = appSettings.Value;
        _apiSettings = apiSettings.Value;
        _featureFlags = featureFlags.Value;

        // Démonstration de lecture directe
        ConnectionString = _configuration.GetConnectionString("DefaultConnection") ?? "Non définie";

        // Lecture par index
        DirectValue = _configuration["AppSettings:ApplicationName"] ?? "Non trouvé";
    }

    #region AppSettings

    public string ApplicationName => _appSettings.ApplicationName;
    public string Version => _appSettings.Version;
    public int MaxItems => _appSettings.MaxItems;
    public bool EnableLogging => _appSettings.EnableLogging;

    #endregion

    #region ApiSettings

    public string ApiBaseUrl => _apiSettings.BaseUrl;
    public int ApiTimeout => _apiSettings.Timeout;
    public int ApiRetryCount => _apiSettings.RetryCount;

    #endregion

    #region FeatureFlags

    public bool EnableDarkMode => _featureFlags.EnableDarkMode;
    public bool EnableExport => _featureFlags.EnableExport;
    public bool EnableNotifications => _featureFlags.EnableNotifications;

    #endregion

    #region Accès direct

    [ObservableProperty]
    private string _connectionString;

    [ObservableProperty]
    private string _directValue;

    #endregion

    #region Environnement

    public string Environment => System.Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
    public bool IsDevelopment => Environment.Equals("Development", StringComparison.OrdinalIgnoreCase);

    #endregion
}
