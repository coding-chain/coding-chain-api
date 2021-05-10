using System.Reflection;
using System.Text;
using Application;
using Application.Common.Interceptors;
using Application.Contracts.IService;
using Application.Read.Contracts;
using Application.Write.Contracts;
using CodingChainApi.Infrastructure.Common.Exceptions;
using CodingChainApi.Infrastructure.Contexts;
using CodingChainApi.Infrastructure.MessageBroker.RabbitMQ;
using CodingChainApi.Infrastructure.MessageBroker.RabbitMQ.Code.CodeExecution;
using CodingChainApi.Infrastructure.Repositories.AggregateRepositories;
using CodingChainApi.Infrastructure.Repositories.ReadRepositories;
using CodingChainApi.Infrastructure.Services;
using CodingChainApi.Infrastructure.Services.Processes;
using CodingChainApi.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CodingChainApi.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            ConfigureSqlServer(services, configuration);
            ConfigureJwt(services, configuration);
            ConfigureBcrypt(services, configuration);
            ConfigureProcess(services, configuration);
            ConfigureAppData(services, configuration);
            ConfigureLanguages(services, configuration);
            ConfigureRabbitMQ(services, configuration);
            //
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ITimeService, TimeService>();
            services.AddScoped<IParticipationExecutionService, ParticipationExecutionService>();
            RegisterAggregateRepositories(services);
            RegisterReadRepositories(services);
            return services;
        }

        private static void RegisterAggregateRepositories(IServiceCollection services)
        {
            services.AddProxiedScoped<IUserRepository, UserRepository>(typeof(EventPublisherInterceptor));
            services.AddProxiedScoped<IProgrammingLanguageRepository, ProgrammingLanguageRepository>(
                typeof(EventPublisherInterceptor));
            services.AddProxiedScoped<ITeamRepository, TeamRepository>(typeof(EventPublisherInterceptor));
            services.AddProxiedScoped<ITournamentRepository, TournamentRepository>(typeof(EventPublisherInterceptor));
        }

        private static void RegisterReadRepositories(IServiceCollection services)
        {
            services.AddScoped<IReadProgrammingLanguageRepository, ReadProgrammingLanguageRepository>();
            services.AddScoped<IReadUserRepository, ReadUserRepository>();
            services.AddScoped<IReadTeamRepository, ReadTeamRepository>();
            services.AddScoped<IReadTournamentRepository, ReadTournamentRepository>();
        }

        private static void ConfigureProcess(IServiceCollection services, IConfiguration configuration)
        {
            ConfigureInjectableSettings<IProcessSettings, IProcessSettings>(services, configuration);
        }

        private static void ConfigureAppData(IServiceCollection services, IConfiguration configuration)
        {
            ConfigureInjectableSettings<IAppDataSettings, AppDataSettings>(services, configuration);
        }

        private static void ConfigureLanguages(IServiceCollection services, IConfiguration configuration)
        {
            ConfigureInjectableSettings<ILanguagesSettings, LanguagesSettings>(services, configuration);
        }

        private static TImplementation ConfigureInjectableSettings<TInterface, TImplementation>(
            IServiceCollection services,
            IConfiguration configuration) where TImplementation : class, TInterface where TInterface : class
        {
            var settingsName = typeof(TImplementation).Name;
            var settings = configuration.GetSection(settingsName).Get<TImplementation>();
            services.Configure<TImplementation>(configuration.GetSection(settingsName));
            services.AddSingleton<TInterface>(sp =>
                sp.GetRequiredService<IOptions<TImplementation>>().Value);
            return settings;
        }

        private static void ConfigureJwt(IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = ConfigureInjectableSettings<IJwtSettings, JwtSettings>(services, configuration);
            if (jwtSettings.Key is null)
                throw new InfrastructureException("JWt Key is null please check your JwtSettings");
            if (jwtSettings.MinutesDuration is null)
                throw new InfrastructureException("JWt MinutesDuration is null please check your JwtSettings");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                    };
                });
        }

        private static void ConfigureBcrypt(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<BcryptSettings>(configuration.GetSection(nameof(BcryptSettings)));
            services.AddSingleton<IBcryptSettings>(sp =>
                sp.GetRequiredService<IOptions<BcryptSettings>>().Value);
        }

        private static void ConfigureSqlServer(IServiceCollection services, IConfiguration configuration)
        {
            var dbSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
            if (dbSettings.ConnectionString is null)
            {
                throw new InfrastructureException("Please provide connection string");
            }

            services.AddDbContext<CodingChainContext>(o => o.UseSqlServer(dbSettings.ConnectionString));
        }

        private static void ConfigureRabbitMQ(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            // RabbitMQ
            serviceCollection.AddHostedService<ParticipationExecutionService>();
            serviceCollection.AddSingleton<IRabbitMqPublisher, RabbitMQPublisher>();
            ConfigureInjectableSettings<IRabbitMQSettings, RabbitMQSettings>(serviceCollection, configuration);
            // End RabbitMQ Configuration
        }
    }
}