using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace ThomasonAlgorithm.Demo;

public static class DependencyInjection
{
    /// <summary>
    /// Extension method for configuring the PostgreSQL database context and registering it in the DI container.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the DbContext will be added.</param>
    /// <param name="connectionString">The connection string used to connect to the PostgreSQL database.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> with the configured database context.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="connectionString"/> is null.</exception>
    public static IServiceCollection AddDbInfrastructure(this IServiceCollection services, string connectionString)
    {
        if (connectionString == null)
            throw new ArgumentNullException(nameof(connectionString), "Connection string is required");

        var npgSqlDatasourceBuilder = new NpgsqlDataSourceBuilder(connectionString).EnableDynamicJson().Build();
        
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(npgSqlDatasourceBuilder).UseSnakeCaseNamingConvention());

        return services;
    }
}