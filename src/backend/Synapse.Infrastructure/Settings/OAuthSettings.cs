namespace Synapse.Infrastructure.Settings;

public class OAuthSettings
{
    public GoogleSettings Google { get; set; } = new();
    public MicrosoftSettings Microsoft { get; set; } = new();
    public FrontendUrlSettings Frontend { get; set; } = new();
}

public class GoogleSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}

public class MicrosoftSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
}

public class FrontendUrlSettings
{
    public string BaseUrl { get; set; } = "http://localhost:3000";
}
