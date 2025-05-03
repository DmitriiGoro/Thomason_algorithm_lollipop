using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace ThomasonAlgorithm.Demo;

public static class DependencyInjection
{
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