using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;

namespace BlockInfrastructure.Common.Test.Shared.Integrations.Fixtures;

public static class TestConfiguration
{
    public static IConfiguration Create(ContainerFixture containerFixture)
    {
        using var rsa = RSA.Create();
        var privateXml = rsa.ToXmlString(true);
        var postgreSqlPort = containerFixture.PostgreSqlTestcontainer.GetMappedPublicPort(5432);
        var configurationBuilder = new ConfigurationBuilder();
        var memoryConfiguration = new Dictionary<string, string>
        {
            ["Auth:JwtSigningKey"] = privateXml,
            ["Auth:GoogleOAuthClientID"] = "",
            ["Auth:GoogleOAuthClientSecret"] = "",
            ["ConnectionStrings:DatabaseConnection"] =
                $"Server=localhost;Database={Ulid.NewUlid().ToString()};Port={postgreSqlPort};User Id=admin;Password=testPassword@;",
            ["ConnectionStrings:RedisConnection"] = containerFixture.RedisTestcontainer.GetConnectionString(),
            ["RabbitMq:Host"] = "localhost",
            ["RabbitMq:Port"] = containerFixture.RabbitMqTestcontainer.GetMappedPublicPort(5672).ToString(),
            ["RabbitMq:Username"] = "admin",
            ["RabbitMq:Password"] = "testPassword@",
            ["loki"] = "http://localhost:3150",
            ["otlp"] = "http://localhost:3150"
        };

        return configurationBuilder.AddInMemoryCollection(memoryConfiguration).Build();
    }
}