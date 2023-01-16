using Newtonsoft.Json;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using ServerPrototype.Actors.Grains;
using ServerPrototype.App.Configs;
using ServerPrototype.App.Services;

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
                    var mongoDbConfig = hosting.Configuration.GetSection("Mongo").Get<MongoDbConfig>();

                    //builder.AddMemoryGrainStorage("OrleansStorage")
                    //    .AddMemoryGrainStorageAsDefault();
                    builder.UseLocalhostClustering()
                         .UseDashboard(options =>
                         {
                             options.Port = 8082;
                         })
                         .UseMongoDBClient(mongoDbConfig.ConnectionUri)
                         .UseMongoDBClustering(options =>
                         {
                             options.DatabaseName = mongoDbConfig.DbName;
                         })
                         .AddMongoDBGrainStorageAsDefault(optionsBuilder =>
                         {
                             optionsBuilder.Configure(options =>
                             {
                                 options.DatabaseName = mongoDbConfig.DbName;
                                 options.CollectionPrefix = "Orleans-"; 
                                 
                                 options.ConfigureJsonSerializerSettings = settings =>
                                 {
                                     settings.NullValueHandling = NullValueHandling.Include;
                                     settings.ObjectCreationHandling = ObjectCreationHandling.Replace;
                                     settings.DefaultValueHandling = DefaultValueHandling.Populate;
                                 };
                             });
                         })
                         .Configure<ClusterOptions>(options =>
                         {
                             options.ClusterId = "my-first-cluster";
                             options.ServiceId = "AspNetSampleApp";
                         })
                         .ConfigureApplicationParts(parts => {
                             parts.AddApplicationPart(typeof(AccountGrain).Assembly).WithReferences();
                         });
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