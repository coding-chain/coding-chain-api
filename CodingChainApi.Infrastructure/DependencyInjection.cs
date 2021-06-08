using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Application;
using Application.Common.Interceptors;
using Application.Contracts.Dtos;
using Application.Contracts.IService;
using Application.Read.Contracts;
using Application.Write.Contracts;
using CodingChainApi.Infrastructure.Common.Exceptions;
using CodingChainApi.Infrastructure.Contexts;
using CodingChainApi.Infrastructure.Hubs;
using CodingChainApi.Infrastructure.Repositories.AggregateRepositories;
using CodingChainApi.Infrastructure.Repositories.ReadRepositories;
using CodingChainApi.Infrastructure.Services;
using CodingChainApi.Infrastructure.Services.Cache;
using CodingChainApi.Infrastructure.Services.Messaging;
using CodingChainApi.Infrastructure.Services.Parser;
using CodingChainApi.Infrastructure.Settings;
using MediatR;
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
            services.AddMediatR(typeof(DependencyInjection).GetTypeInfo().Assembly);
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            ConfigureCache(services, configuration);
            ConfigureSqlServer(services, configuration);
            ConfigureJwt(services, configuration);
            ConfigureBcrypt(services, configuration);
            ConfigureProcess(services, configuration);
            ConfigureAppData(services, configuration);
            ConfigureRabbitMq(services, configuration);
            //
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ITimeService, TimeService>();
            services.AddScoped<IFunctionTypeParserService, FunctionTypeParserService>();
            services.AddScoped<ICache, Cache>();
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
            services.AddProxiedScoped<IStepEditionRepository, StepEditionRepository>(typeof(EventPublisherInterceptor));
            services.AddProxiedScoped<IParticipationRepository, ParticipationRepository>(
                typeof(EventPublisherInterceptor));
            services.AddProxiedScoped<IParticipationsSessionsRepository, ParticipationsSessionRepository>(
                typeof(EventPublisherInterceptor));
            services.AddProxiedScoped<IPlagiarizedFunctionRepository, PlagiarizedFunctionRepository>(
                typeof(EventPublisherInterceptor));
        }

        private static void RegisterReadRepositories(IServiceCollection services)
        {
            services.AddScoped<IReadProgrammingLanguageRepository, ReadProgrammingLanguageRepository>();
            services.AddScoped<IReadUserRepository, ReadUserRepository>();
            services.AddScoped<IReadTeamRepository, ReadTeamRepository>();
            services.AddScoped<IReadTournamentRepository, ReadTournamentRepository>();
            services.AddScoped<IReadStepRepository, ReadStepRepository>();
            services.AddScoped<IReadTestRepository, ReadTestRepository>();
            services.AddScoped<IReadParticipationRepository, ReadParticipationRepository>();
            services.AddScoped<IReadRightRepository, ReadRightRepository>();
            services.AddScoped<IReadParticipationSessionRepository, ReadParticipationSessionRepository>();
            services.AddScoped<IReadFunctionSessionRepository, ReadFunctionSessionRepository>();
            services.AddScoped<IReadUserSessionRepository, ReadUserSessionRepository>();
        }

        private static void ConfigureCache(IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();
            ConfigureInjectableSettings<ICacheSettings, CacheSettings>(services, configuration);
        }

        private static void ConfigureProcess(IServiceCollection services, IConfiguration configuration)
        {
            ConfigureInjectableSettings<IProcessSettings, ProcessSettings>(services, configuration);
        }

        private static void ConfigureAppData(IServiceCollection services, IConfiguration configuration)
        {
            ConfigureInjectableSettings<IAppDataSettings, AppDataSettings>(services, configuration);
        }


        private static TImplementation ConfigureInjectableSettings<TInterface, TImplementation>(
            IServiceCollection services,
            IConfiguration configuration, bool singleton = true) where TImplementation : class, TInterface
            where TInterface : class
        {
            var settingsName = typeof(TImplementation).Name;
            var settings = configuration.GetSection(settingsName).Get<TImplementation>();
            services.Configure<TImplementation>(configuration.GetSection(settingsName));
            if (singleton)
                services.AddSingleton<TInterface>(sp =>
                    sp.GetRequiredService<IOptions<TImplementation>>().Value);
            else
                services.AddScoped<TInterface>(sp =>
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

            services.AddAuthentication(opt =>
                {
                    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                path.StartsWithSegments(ParticipationSessionsHub.Route))
                                // Read the token out of the query string
                                context.Token = accessToken;

                            return Task.CompletedTask;
                        }
                    };
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
                throw new InfrastructureException("Please provide connection string");

            services.AddDbContext<CodingChainContext>(options =>
                options.UseSqlServer(dbSettings.ConnectionString,
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
            );
        }

        private static void ConfigureRabbitMq(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            // RabbitMQ
            serviceCollection.AddScoped<IDispatcher<RunParticipationTestsDto>, ParticipationPendingExecutionService>();
            serviceCollection
                .AddScoped<IDispatcher<PlagiarismAnalyzeExecutionDto>, PlagiarismPendingExecutionService>();
            serviceCollection
                .AddScoped<IDispatcher<CleanParticipationExecutionDto>, CleanParticipationExecutionService>();
            serviceCollection
                .AddScoped<IDispatcher<PrepareParticipationExecutionDto>, PrepareParticipationExecutionService>();
            ConfigureInjectableSettings<IRabbitMqSettings, RabbitMqSettings>(serviceCollection, configuration);
            // End RabbitMQ Configuration
        }
    }
}