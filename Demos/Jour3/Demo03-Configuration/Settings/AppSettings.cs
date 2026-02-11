namespace ConfigDemo.Settings;

/// <summary>
/// Classes fortement typées pour la configuration
/// Utilisées avec IOptions<T> pattern
/// </summary>
public class AppSettings
{
    public const string SectionName = "AppSettings";

    public string ApplicationName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public int MaxItems { get; set; }
    public bool EnableLogging { get; set; }
}

public class ApiSettings
{
    public const string SectionName = "ApiSettings";

    public string BaseUrl { get; set; } = string.Empty;
    public int Timeout { get; set; }
    public int RetryCount { get; set; }
}

public class FeatureFlags
{
    public const string SectionName = "FeatureFlags";

    public bool EnableDarkMode { get; set; }
    public bool EnableExport { get; set; }
    public bool EnableNotifications { get; set; }
}
