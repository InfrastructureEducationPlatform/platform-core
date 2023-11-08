using Testcontainers.PostgreSql;

namespace BlockInfrastructure.Core.Test.Shared.Integrations.Fixtures;

[CollectionDefinition("Container")]
public class ContainerCollection : ICollectionFixture<ContainerFixture>
{
}

public class ContainerFixture : IDisposable
{
    public readonly PostgreSqlContainer PostgreSqlTestcontainer;


    public ContainerFixture()
    {
        PostgreSqlTestcontainer = new PostgreSqlBuilder()
                                  .WithName($"POSTGRESQL-{Ulid.NewUlid()}")
                                  .WithImage("postgres:15.3")
                                  .WithPortBinding(5432, true)
                                  .WithUsername("admin")
                                  .WithPassword("testPassword@")
                                  .Build();

        var taskList = new List<Task>
        {
            PostgreSqlTestcontainer.StartAsync()
        };
        Task.WhenAll(taskList).Wait();
    }

    public void Dispose()
    {
        PostgreSqlTestcontainer.DisposeAsync()
                               .GetAwaiter().GetResult();
    }
}