using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Services;
using Microsoft.EntityFrameworkCore;

namespace BlockInfrastructure.Core.Services;

public class PricingService(DatabaseContext databaseContext)
{
    public async Task<List<PricingInformation>> GetAllPricingInformationAsync()
    {
        return await databaseContext.PricingInformations
                                    .Include(a => a.PriceInfoPerVendors)
                                    .ToListAsync();
    }
}