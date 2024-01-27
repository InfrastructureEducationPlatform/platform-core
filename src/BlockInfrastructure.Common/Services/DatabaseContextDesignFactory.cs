using System.CommandLine;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

namespace BlockInfrastructure.Common.Services;

public class DatabaseContextDesignFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        var rootCommand = new RootCommand("Common Database Context Factory");

        // Database Information
        var hostOption = new Option<string>("--host", "Database Host")
        {
            IsRequired = true
        };
        rootCommand.AddOption(hostOption);
        var portOption = new Option<int>("--port", "Database Port")
        {
            IsRequired = true
        };
        rootCommand.AddOption(portOption);
        var databaseOption = new Option<string>("--database", "Database Name")
        {
            IsRequired = true
        };
        rootCommand.AddOption(databaseOption);
        var usernameOption = new Option<string>("--username", "Database Username")
        {
            IsRequired = true
        };
        rootCommand.AddOption(usernameOption);
        var passwordOption = new Option<string>("--password", "Database Password")
        {
            IsRequired = true
        };
        rootCommand.AddOption(passwordOption);

        DatabaseConfiguration? databaseConfiguration = null;
        rootCommand.SetHandler((host, port, database, username, password) =>
        {
            databaseConfiguration = new DatabaseConfiguration
            {
                Host = host,
                Port = port,
                DatabaseName = database,
                Username = username,
                Password = password
            };
        }, hostOption, portOption, databaseOption, usernameOption, passwordOption);
        rootCommand.Invoke(args);

        var connectionConfiguration = new NpgsqlConnectionStringBuilder
        {
            Host = databaseConfiguration!.Host,
            Port = databaseConfiguration.Port,
            Database = databaseConfiguration.DatabaseName,
            Username = databaseConfiguration.Username,
            Password = databaseConfiguration.Password
        };
        return new DatabaseContext(new DbContextOptionsBuilder<DatabaseContext>()
                                   .UseNpgsql(connectionConfiguration.ToString())
                                   .EnableDetailedErrors()
                                   .EnableSensitiveDataLogging()
                                   .Options);
    }

    private class DatabaseConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string DatabaseName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}