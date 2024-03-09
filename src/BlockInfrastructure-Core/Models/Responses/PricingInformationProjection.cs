using System.ComponentModel.DataAnnotations;
using BlockInfrastructure.Common.Models.Data;

namespace BlockInfrastructure.Core.Models.Responses;

public class PricingInformationProjection
{
    [Required]
    public string Id { get; set; }

    [Required]
    public PricingMachineType MachineType { get; set; }

    [Required]
    public string Tier { get; set; }

    [Required]
    public List<PriceInfoPerVendorProjection> PriceInfoPerVendors { get; set; }

    public static PricingInformationProjection FromPricingInformation(PricingInformation pricingInformation)
    {
        return new PricingInformationProjection
        {
            Id = pricingInformation.Id,
            MachineType = pricingInformation.MachineType,
            Tier = pricingInformation.Tier,
            PriceInfoPerVendors = pricingInformation.PriceInfoPerVendors
                                                    .Select(PriceInfoPerVendorProjection.FromPriceInfoPerVendor).ToList()
        };
    }
}

public class PriceInfoPerVendorProjection
{
    [Required]
    public string PricingInformationId { get; set; }

    [Required]
    public VendorType Vendor { get; set; }

    [Required]
    public decimal PricePerHour { get; set; }

    [Required]
    public string TierInformation { get; set; }

    public static PriceInfoPerVendorProjection FromPriceInfoPerVendor(PriceInfoPerVendor priceInfoPerVendor)
    {
        return new PriceInfoPerVendorProjection
        {
            PricingInformationId = priceInfoPerVendor.PricingInformationId,
            Vendor = priceInfoPerVendor.Vendor,
            PricePerHour = priceInfoPerVendor.PricePerHour,
            TierInformation = priceInfoPerVendor.TierInformation
        };
    }
}