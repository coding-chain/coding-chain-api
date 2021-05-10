using System.Linq;
using System.Reflection;
using Application;
using Application.Common.MessageBroker.RabbitMQ;
using Application.Common.MessageBroker.RabbitMQ.Code.CodeExecution;
using Application.Contracts.IService;
using Application.Settings;
using CodingChainApi.Infrastructure;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NeosCodingApi.Controllers;
using NeosCodingApi.Services;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using ZymLabs.NSwag.FluentValidation;
using DependencyInjection = Application.DependencyInjection;

namespace NeosCodingApi
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
            services.AddApplication();
            services.AddSingleton<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IPropertyCheckerService, PropertyCheckerService>();
            services.AddHttpContextAccessor();

            // RabbitMQ
            services.AddHostedService<CodeExecutionListener>();
            services.AddSingleton<RabbitMQPublisher, RabbitMQPublisher>();
            services.Configure<MessageBrokerConfiguration>(options =>
            {
                options.RabbitHost = "localhost";
                options.RabbitPort = 15672;
                options.RabbitUserName = "lapin";
                options.RabbitPassword = "lapin";
            });
            // End RabbitMQ Configuration

            services.AddSignalR();

            services.AddCors();
            // services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = 10;
                options.MemoryBufferThreshold = int.MaxValue;
                options.ValueLengthLimit = int.MaxValue; //TODO Change value
                options.MultipartBodyLengthLimit = long.MaxValue;
            });
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                })
                .AddFluentValidation(options =>
                {
                    options.RegisterValidatorsFromAssemblies(new[]
                        {Assembly.GetAssembly(typeof(DependencyInjection)), Assembly.GetExecutingAssembly()});
                    // options.ValidatorFactoryType = typeof(HttpContextServiceProviderValidatorFactory);
                })
                .AddMvcOptions(options =>
                {
                    // Clear the default MVC model validations, as we are registering all model validators using FluentValidation
                    options.ModelMetadataDetailsProviders.Clear();
                    options.ModelValidatorProviders.Clear();
                });
            ;
            services.AddSingleton<FluentValidationSchemaProcessor>();
            services.AddVersionedApiExplorer(setupAction => { setupAction.GroupNameFormat = "'v'VV"; });

            services.AddApiVersioning(setupAction =>
            {
                setupAction.AssumeDefaultVersionWhenUnspecified = true;
                setupAction.DefaultApiVersion = new ApiVersion(1, 0);
                setupAction.ReportApiVersions = true;
            });

            var apiVersionDescriptionProvider =
                services.BuildServiceProvider().GetService<IApiVersionDescriptionProvider>();

            foreach (var apiVersionDescription in apiVersionDescriptionProvider.ApiVersionDescriptions)
            {
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

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] {"application/octet-stream"});
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseResponseCompression();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
            }


            app.UseRouting();


            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<SutHub>("/sut");
            });

            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}