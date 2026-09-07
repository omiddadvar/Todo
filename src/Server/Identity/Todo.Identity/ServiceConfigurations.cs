using MassTransit;
using Todo.Identity.Constants;

namespace Todo.Identity;

public static class ServiceConfigurations
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMasstransitService(configuration);
        return services;
    }
    private static IServiceCollection AddMasstransitService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration[ConfigKeyword.RabbitMQ.Host], "/", h =>
                {
                    h.Username(configuration[ConfigKeyword.RabbitMQ.Username]);
                    h.Password(configuration[ConfigKeyword.RabbitMQ.Password]);
                });

                cfg.ConfigureEndpoints(context);
            });
        });
        return services;
    }
}
