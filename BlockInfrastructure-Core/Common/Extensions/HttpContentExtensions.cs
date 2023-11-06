using Newtonsoft.Json;

namespace BlockInfrastructure.Core.Common.Extensions;

public static class HttpContentExtensions
{
    public async static Task<T> Deserialize<T>(this HttpContent httpContent)
    {
        return JsonConvert.DeserializeObject<T>(await httpContent.ReadAsStringAsync())!;
    }
}