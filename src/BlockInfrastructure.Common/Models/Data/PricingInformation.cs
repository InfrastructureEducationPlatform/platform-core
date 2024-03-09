using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlockInfrastructure.Common.Models.Data;

public class PricingInformation
{
    [Key]
    public string Id { get; set; }

    public PricingMachineType MachineType { get; set; }

    public string Tier { get; set; }

    public List<PriceInfoPerVendor> PriceInfoPerVendors { get; set; }
}

public class PriceInfoPerVendor
{
    public string PricingInformationId { get; set; }
    public PricingInformation PricingInformation { get; set; }

    public VendorType Vendor { get; set; }
    public decimal PricePerHour { get; set; }

    /// <summary>
    ///     Platform Specific Information
    /// </summary>
    public string TierInformation { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PricingMachineType
{
    VirtualMachine,
    WebServer,
    DatabaseServer
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum VendorType
{
    AWS
}