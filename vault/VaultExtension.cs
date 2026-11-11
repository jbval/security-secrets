using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

public static class VaultExtensions
{
    public static IConfigurationBuilder AddVault(this IConfigurationBuilder builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        var vaultUrl = Environment.GetEnvironmentVariable("HASHICORP_VAULT_URL");
        var token = Environment.GetEnvironmentVariable("HASHICORP_VAULT_TOKEN");
        var path = Environment.GetEnvironmentVariable("HASHICORP_VAULT_PATH");
        var mountPoint = Environment.GetEnvironmentVariable("HASHICORP_VAULT_MOUNTPOINT");

        ArgumentNullException.ThrowIfNull(vaultUrl);
        ArgumentNullException.ThrowIfNull(token);
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(mountPoint);

        builder.Add(new VaultConfigurationSource
        {
            VaultUrl = vaultUrl,
            Token = token,
            Path = path,
            MountPoint = mountPoint,
        });

        return builder;
    }
}

internal class VaultConfigurationSource : IConfigurationSource
{
    public required string VaultUrl { get; init; }
    public required string Token { get; init; }
    public required string Path { get; init; }
    public required string MountPoint { get; init; }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new VaultConfigurationProvider(this);
    }
}

internal class VaultConfigurationProvider : ConfigurationProvider
{
    private readonly VaultConfigurationSource source;
    private readonly IVaultClient vaultClient;

    public VaultConfigurationProvider(VaultConfigurationSource source)
    {
        this.source = source ?? throw new ArgumentNullException(nameof(source));
        var settings = new VaultClientSettings(source.VaultUrl, new TokenAuthMethodInfo(source.Token));
        vaultClient = new VaultClient(settings);
    }

    public override void Load()
    {
        LoadAsync()
            .GetAwaiter()
            .GetResult();
    }

    private async Task LoadAsync()
    {
        var readSecretAsync = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(
            path: source.Path,
            mountPoint: source.MountPoint);

        foreach (KeyValuePair<string, object> kvp in readSecretAsync.Data.Data) // Acessa secret.Data.Data para KV v2
        {
            this.Data.Add(new KeyValuePair<string, string?>(kvp.Key, kvp.Value?.ToString() ?? string.Empty));
        }
        
        OnReload();
    }
}


public class ApplicationSettings
{
   public ConnectString ConnectString { get; set; }    
}

public class ConnectString
{
    public string Default { get; set; }
}