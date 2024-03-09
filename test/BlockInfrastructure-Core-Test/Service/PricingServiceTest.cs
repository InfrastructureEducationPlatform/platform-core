using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Services;
using BlockInfrastructure.Common.Test.Fixtures;
using BlockInfrastructure.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace BlockInfrastructure.Core.Test.Service;

public class PricingServiceTest
{
    private readonly UnitTestDatabaseContext _databaseContext =
        new(new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Ulid.NewUlid().ToString()).Options);

    private readonly PricingService _pricingService;

    public PricingServiceTest()
    {
        _pricingService = new PricingService(_databaseContext);
    }

    [Fact(DisplayName = "GetAllPricingInformationAsync: GetAllPricingInformationAsync는 만약 호출되었을 때, 리스트를 반환해야 합니다.")]
    public async Task Is_GetAllPricingInformationAsync_ShouldReturnList_WhenCalled()
    {
        // Let
        var pricingInformation = new PricingInformation
        {
            Id = Ulid.NewUlid().ToString(),
            MachineType = PricingMachineType.VirtualMachine,
            Tier = "Standard",
            PriceInfoPerVendors = new List<PriceInfoPerVendor>
            {
                new()
                {
                    Vendor = VendorType.AWS,
                    PricePerHour = 0.5m
                }
            }
        };
        _databaseContext.PricingInformations.Add(pricingInformation);
        await _databaseContext.SaveChangesAsync();

        // Do
        var result = await _pricingService.GetAllPricingInformationAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal(pricingInformation.Id, result.First().Id);
        Assert.Equal(pricingInformation.MachineType, result.First().MachineType);
        Assert.Equal(pricingInformation.Tier, result.First().Tier);
        Assert.Single(result.First().PriceInfoPerVendors);
        Assert.Equal(pricingInformation.PriceInfoPerVendors.First().Vendor, result.First().PriceInfoPerVendors.First().Vendor);
        Assert.Equal(pricingInformation.PriceInfoPerVendors.First().PricePerHour,
            result.First().PriceInfoPerVendors.First().PricePerHour);
    }
}