namespace BlockInfrastructure.Common.Models.Internal.PluginConfigs;

public class AzureStaticProviderConfig
{
    public string SubscriptionId { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string TenantId { get; set; }
}