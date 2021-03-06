using System.Linq;
using System.Reflection;
using Application;
using Application.Contracts.IService;
using CodingChainApi.Infrastructure;
using CodingChainApi.Infrastructure.Hubs;
using CodingChainApi.Infrastructure.Settings;
using CodingChainApi.Messaging;
using CodingChainApi.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using ZymLabs.NSwag.FluentValidation;
using DependencyInjection = Application.DependencyInjection;

namespace CodingChainApi
{
    public class Startup
    {
        private const string PolicyName = "AllowAll";


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddOptions(); // enable Configuration Services


            services.AddInfrastructure(Configuration);
            AddCors(services);
            services.AddApplication();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IPropertyCheckerService, PropertyCheckerService>();
            services.AddHttpContextAccessor();
            AddSignalR(services);
            services.AddHostedService<ParticipationPreparedListener>();
            services.AddHostedService<ParticipationDoneExecutionListener>();
            services.AddHostedService<PlagiarismExecutionDoneListener>();

            services.AddCors();

            ConfigureControllers(services);

            services.AddSingleton<FluentValidationSchemaProcessor>();
            services.AddVersionedApiExplorer(setupAction => { setupAction.GroupNameFormat = "'v'VV"; });

            var apiVersionDescriptionProvider = ConfigureVersioning(services);

            ConfigureSwagger(services, apiVersionDescriptionProvider);

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
        }

        private void AddSignalR(IServiceCollection services)
        {
            var signalRBuilder = services.AddSignalR();
            var signalRSettings =
                services.ConfigureInjectableSettings<ISignalRSettings, SignalRSettings>(Configuration);
            if (!string.IsNullOrWhiteSpace(signalRSettings.ConnectionString))
            {
                signalRBuilder.AddAzureSignalR(options => options.ConnectionString = signalRSettings.ConnectionString);
            }
        }

        private void AddCors(IServiceCollection services)
        {
            var corsSettings = services.ConfigureInjectableSettings<ICorsSettings, CorsSettings>(Configuration);
            services.AddCors(options =>
            {
                options.AddPolicy(PolicyName, builder =>
                {
                    builder.WithOrigins(corsSettings.FrontEndUrl)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .WithExposedHeaders("Location");
                });
            });
        }

        private static void ConfigureControllers(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                })
                .AddFluentValidation(options =>
                {
                    options.RegisterValidatorsFromAssemblies(new[]
                        { Assembly.GetAssembly(typeof(DependencyInjection)), Assembly.GetExecutingAssembly() });
                    // options.ValidatorFactoryType = typeof(HttpContextServiceProviderValidatorFactory);
                })
                .AddMvcOptions(options =>
                {
                    // Clear the default MVC model validations, as we are registering all model validators using FluentValidation
                    options.ModelMetadataDetailsProviders.Clear();
                    options.ModelValidatorProviders.Clear();
                });
        }

        private static IApiVersionDescriptionProvider? ConfigureVersioning(IServiceCollection services)
        {
            services.AddApiVersioning(setupAction =>
            {
                setupAction.AssumeDefaultVersionWhenUnspecified = true;
                setupAction.DefaultApiVersion = new ApiVersion(1, 0);
                setupAction.ReportApiVersions = true;
            });

            var apiVersionDescriptionProvider =
                services.BuildServiceProvider().GetService<IApiVersionDescriptionProvider>();
            return apiVersionDescriptionProvider;
        }

        private static void ConfigureSwagger(IServiceCollection services,
            IApiVersionDescriptionProvider? apiVersionDescriptionProvider)
        {
            foreach (var apiVersionDescription in apiVersionDescriptionProvider.ApiVersionDescriptions)
                services.AddSwaggerDocument((settings, serviceProvider) =>
                {
                    var fluentValidationSchemaProcessor = serviceProvider.GetService<FluentValidationSchemaProcessor>();
                    // Add the fluent validations schema processor
                    settings.SchemaProcessors.Add(fluentValidationSchemaProcessor);
                    settings.PostProcess = document =>
                    {
                        document.Info.Version = apiVersionDescription.ApiVersion.ToString();
                        document.Info.Title = "CodingChain API";
                        document.Info.Description = "REST API for example.";
                    };
                });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();


            app.UseRouting();

            app.UseCors(PolicyName);
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ParticipationSessionsHub>(ParticipationSessionsHub.Route,
                    options => { options.Transports = HttpTransportType.ServerSentEvents; });
                endpoints.MapHub<PlagiarismHub>(PlagiarismHub.Route,
                    options => { options.Transports = HttpTransportType.ServerSentEvents; });
            });

            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}