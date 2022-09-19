using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Timer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;
using ServerPrototype.App.Configs;
using ServerPrototype.App.Infrastructure;
using ServerPrototype.DAL.Api;
using ServerPrototype.DAL.Configs;
using ServerPrototype.DAL.Implementation;
using System.Text.Json.Serialization;

namespace ServerPrototype.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


//            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new JsonKnownTypeConverter());


            //BsonClassMap.RegisterClassMap<PlayerFarmGrain.PlayerFramState>(cm =>
            //{
            //    cm.AutoMap();
            //    //cm.SetDictionaryRepresentation(x => x.InventoryContainer.Items, DictionaryRepresentation.Document);
            //    cm.SetDictionaryRepresentation(x => x., DictionaryRepresentation.Document);
            //});
            //BsonClassMap.RegisterClassMap<PlayerState>(cm =>
            //{
            //    cm.AutoMap();
            //    //cm.SetDictionaryRepresentation(x => x.InventoryContainer.Items, DictionaryRepresentation.Document);
            //    cm.SetDictionaryRepresentation(x => x.Stats, DictionaryRepresentation.Document);
            //});

            services.AddSwaggerGen(c =>
            {
                //// Set the comments path for the Swagger JSON and UI.
                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //c.IncludeXmlComments(xmlPath);
            });

            // METRICS
            var metrics = AppMetrics.CreateDefaultBuilder()
                .Build();
            services.AddMetrics(metrics);
            services.AddMetricsEndpoints();

            // CONTROLLERS
            services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.AddSignalR();

            services.AddSingleton<IMetricsUpdater, AppMetricsProxy>();
            services.AddSingleton<IApiGameServer, ApiGameServer>();
            services.AddSingleton<IPlayerInventoryDb, PlayerInventoryDb>();
            services.AddSingleton<IPlayerEffectsDb, PlayerEffectsDb>();

            //services.AddSingleton<IMongoDatabase>(provider =>
            //{
            //    var config = Configuration.GetSection("Mongo").Get<MongoDbConfig>();
            //    var client = provider.GetRequiredService<IMongoClient>();
            //    return client.GetDatabase(config.DbName);
            //});


            //services.AddSingleton<IPlayerRepository, PlayerRepository>();
            //services.AddSingleton<ICharacterRepository, CharacterRepository>();
            //services.AddSingleton<IBattleRepository, BattleRepository>();
            //services.AddSingleton<IMetricsUpdater, AppMetricsProxy>();
            //services.AddSingleton<IMessageSerializer, MessagePackSerializerProxy>();

            services.TryAddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            services.TryAddSingleton(serviceProvider =>
            {
                var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
                return provider.Create<CounterOptions>();
            });
            services.TryAddSingleton(serviceProvider =>
            {
                var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
                return provider.Create<TimerOptions>();
            });
            services.TryAddSingleton(serviceProvider =>
            {
                var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
                return provider.Create<UserSession>();
            });


            services.Configure<ApiServerOptions>(Configuration.GetSection("Server"));
            services.Configure<PlayerInventoryDbConfig>(Configuration.GetSection("PlayerInventoryDb"));
            services.Configure<PlayerEffectDbConfig>(Configuration.GetSection("PlayerEffectDb"));

            //add here new profiles from AutoMapper 
            services.AddAutoMapper(config =>
            {
                //config.AddProfile<PacketProfile>();
                //config.AddProfile<ConfigProfile>();
                //config.AddProfile<ServerProfile>();
                //config.AddProfile<ControllerProfile>();
            });

            services.AddHttpClient();

            //services.Configure<HttpClientsConfig>(Configuration.GetSection("HttpClients"));
            //services.AddHttpClient(Constants.HTTP_CLIENT_BOTS_KEY, (serviceProvider, config) =>
            //{
            //    config.BaseAddress = new Uri(serviceProvider.GetRequiredService<IOptions<HttpClientsConfig>>().Value.BotsManagerBasePath);
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "BTA PROTOTYPE API");
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
