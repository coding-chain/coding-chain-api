using System;
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
using CodingChainApi.Infrastructure.CronManagement;
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
using StackExchange.Redis;
using Quartz;

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
            ConfigureCronManagement(services, configuration);
            ConfigureRabbitMq(services, configuration);
            //
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ITimeService, TimeService>();
            services.AddScoped<IFunctionTypeParserService, FunctionTypeParserService>();
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
            services.AddProxiedScoped<ICronRepository, CronRepository>(typeof(EventPublisherInterceptor));
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
            services.AddScoped<IReadFunctionRepository, ReadFunctionRepository>();
            services.AddScoped<IReadCronRepository, ReadCronRepository>();
        }

        private static void ConfigureCache(this IServiceCollection services, IConfiguration configuration)
        {
            ConfigureInjectableSettings<ICacheSettings, CacheSettings>(services, configuration);
            // services.AddMemoryCache();
            // services.AddScoped<ICache, Cache>();
            var redisSettings =
                ConfigureInjectableSettings<IRedisCacheSettings, RedisCacheSettings>(services, configuration);
            services.AddStackExchangeRedisCache(options => { options.Configuration = redisSettings.ConnectionString; });
            services.AddScoped<ICache, RedisCache>();
        }

        private static void ConfigureProcess(IServiceCollection services, IConfiguration configuration)
        {
            ConfigureInjectableSettings<IProcessSettings, ProcessSettings>(services, configuration);
        }

        private static void ConfigureAppData(IServiceCollection services, IConfiguration configuration)
        {
            ConfigureInjectableSettings<IAppDataSettings, AppDataSettings>(services, configuration);
        }


        public static TImplementation ConfigureInjectableSettings<TInterface, TImplementation>(
            this IServiceCollection services,
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

        private static void ConfigureCronManagement(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var settings =
                ConfigureInjectableSettings<IQuartzSettings, QuartzSettings>(serviceCollection, configuration);
            serviceCollection.AddQuartz(q =>
            {
                serviceCollection.AddQuartz(q =>
                {
                    q.UseMicrosoftDependencyInjectionScopedJobFactory();

                    // Register the job, loading the schedule from configuration
                    q.AddJobAndTrigger<PlagiarismAnalysisCronJob>(nameof(settings.PlagiarismAnalysisCronJob),
                        settings.PlagiarismAnalysisCronJob);
                });

                serviceCollection.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
            });
            serviceCollection.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        }

        private static void AddJobAndTrigger<T>(
            this IServiceCollectionQuartzConfigurator quartz,
            string configKey, string cronSchedule)
            where T : IJob
        {
            // Use the name of the IJob as the appsettings.json key
            string jobName = typeof(T).Name;

            // Try and load the schedule from configuration

            // Some minor validation
            if (string.IsNullOrEmpty(cronSchedule))
            {
                throw new Exception($"No Quartz.NET Cron schedule found for job in configuration at {configKey}");
            }

            // register the job as before
            var jobKey = new JobKey(jobName);
            quartz.AddJob<T>(opts => opts.WithIdentity(jobKey));

            quartz.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity(jobName + "-trigger")
                .WithCronSchedule(cronSchedule)); // use the schedule from configuration
        }
    }
}

