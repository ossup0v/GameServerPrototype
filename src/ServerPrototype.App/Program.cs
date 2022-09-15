using Orleans;
using Orleans.ApplicationParts;
using Orleans.Hosting;
using Orleans.Runtime.Development;
using ServerPrototype.Actors.Grains;
using ServerPrototype.App.Services;
using ServerPrototype.Interfaces.Grains;

namespace ServerPrototype.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseOrleans((hosting, builder) =>
                {
                    builder.ConfigureApplicationParts(parts => {
                        parts.AddApplicationPart(typeof(AccountGrain).Assembly).WithReferences();
                    });

                    builder.AddMemoryGrainStorage("OrleansStorage")
                        .AddMemoryGrainStorageAsDefault();
                    builder.UseLocalhostClustering();
                    //builder.UseLocalhostClustering()
                    //.ConfigureApplicationParts(parts=> parts
                    //    .AddApplicationPart(new AssemblyPart(typeof(IAccountGrain).Assembly))
                    //    .AddApplicationPart(new AssemblyPart(typeof(AccountGrain).Assembly)
                    //    
                    //));
                    //builder
                    //    .UseLocalhostClustering()
                    //    .UseInMemoryLeaseProvider()
                    //    .ConfigureApplicationParts(parts => parts
                    //        .AddApplicationPart(new AssemblyPart(typeof(IAccountGrain).Assembly))
                    //        .AddApplicationPart(new AssemblyPart(typeof(AccountGrain).Assembly)))
                    //    .UseDashboard();

                    //.ConfigureLogging(builder => builder.AddFilter()

                    //ar mongoDbConfig = hosting.Configuration.GetSection("Mongo").Get<MongoDbConfig>();
                    //var redisDbConfig = hosting.Configuration.GetSection("Redis").Get<RedisDbConfig>();

                    //builder
                    //.UseDashboard(options =>
                    //{
                    //    options.Port = 8082;
                    //})
                    //.UseMongoDBClient(mongoDbConfig.ConnectionUri)
                    //.UseMongoDBClustering(options =>
                    //{
                    //    options.DatabaseName = mongoDbConfig.DbName;
                    //})
                    //.AddMongoDBGrainStorageAsDefault(optionsBuilder =>
                    //{
                    //    optionsBuilder.Configure(options =>
                    //    {
                    //        options.DatabaseName = mongoDbConfig.DbName;
                    //        options.ConfigureJsonSerializerSettings = settings =>
                    //        {
                    //            settings.NullValueHandling = NullValueHandling.Include;
                    //            settings.ObjectCreationHandling = ObjectCreationHandling.Replace;
                    //            settings.DefaultValueHandling = DefaultValueHandling.Populate;
                    //        };
                    //    });
                    //})
                    //.Configure<ClusterOptions>(options =>
                    //{
                    //    options.ClusterId = "my-first-cluster";
                    //    options.ServiceId = "AspNetSampleApp";
                    //})
                    //.AddStartupTask<ConfigsUpdaterWorker>()
                    //.UseInMemoryReminderService()
                    //.AddRedisStreams(Constants.STORAGE_STREAM_PROVIDER, c => c.ConfigureRedis(options => options.ConnectionString = redisDbConfig.ConnectionUri))
                    //.AddMemoryGrainStorage("PubSubStore")
                    //.AddSimpleMessageStreamProvider(Constants.STORAGE_PROVIDER)
                    //.Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                    //.ConfigureApplicationParts(parts =>
                    //    parts.AddApplicationPart(typeof(PlayerGrain).Assembly).WithReferences());
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<GameServerHostedService>();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    //.UseUrls("http://*:8081");
                });
    }
}