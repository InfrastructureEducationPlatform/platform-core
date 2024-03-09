using BlockInfrastructure.Common.Services;
using BlockInfrastructure.Core.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace BlockInfrastructure.Core.Services;

public class PricingService(DatabaseContext databaseContext)
{
    public async Task<List<PricingInformationProjection>> GetAllPricingInformationAsync()
    {
        var data = await databaseContext.PricingInformations
                                        .Include(a => a.PriceInfoPerVendors)
                                        .ToListAsync();
        return data.Select(PricingInformationProjection.FromPricingInformation).ToList();
    }
}