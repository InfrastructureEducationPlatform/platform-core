using DotNet.Testcontainers.Builders;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace BlockInfrastructure.Common.Test.Shared.Integrations.Fixtures;

public class ContainerFixture : IDisposable
{
    public readonly PostgreSqlContainer PostgreSqlTestcontainer;
    public readonly RabbitMqContainer RabbitMqTestcontainer;
    public readonly RedisContainer RedisTestcontainer;

    public ContainerFixture()
    {
        PostgreSqlTestcontainer = new PostgreSqlBuilder()
                                  .WithName($"POSTGRESQL-{Ulid.NewUlid()}")
                                  .WithImage("postgres:15.3")
                                  .WithPortBinding(5432, true)
                                  .WithUsername("admin")
                                  .WithPassword("testPassword@")
                                  .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
                                  .Build();
        RedisTestcontainer = new RedisBuilder()
                             .WithName($"REDIS-{Ulid.NewUlid().ToString()}")
                             .WithPortBinding(6379, true)
                             .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(6379))
                             .Build();
        RabbitMqTestcontainer = new RabbitMqBuilder()
                                .WithName($"RABBITMQ-{Ulid.NewUlid().ToString()}")
                                .WithPortBinding(5672, true)
                                .WithPortBinding(15672, true)
                                .WithUsername("admin")
                                .WithPassword("testPassword@")
                                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5672))
                                .Build();

        var taskList = new List<Task>
        {
            PostgreSqlTestcontainer.StartAsync(),
            RedisTestcontainer.StartAsync(),
            RabbitMqTestcontainer.StartAsync()
        };
        Task.WhenAll(taskList).Wait();
    }

    public void Dispose()
    {
        PostgreSqlTestcontainer.DisposeAsync()
                               .GetAwaiter().GetResult();
        RedisTestcontainer.DisposeAsync()
                          .GetAwaiter().GetResult();
        RabbitMqTestcontainer.DisposeAsync()
                             .GetAwaiter().GetResult();
    }
}