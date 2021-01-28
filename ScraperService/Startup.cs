using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ScraperService.Helpers;
using ScraperService.Repositories;
using System.Net.Http;

namespace ScraperService
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
                       
            services.Configure<AppSettings>(options =>
            {
                Configuration.Bind(options);
            });

            services.AddScoped<IShowsRepositroy, ShowsRepositroy>()
                .AddTransient<ITvMazeWebAccessor, TvMazeWebAccessor>()
                .AddTransient<IWebClientHelper, WebClientHelper>()
                .AddTransient<HttpMessageHandler>(x => new HttpClientHandler())
                .AddSingleton<IMongoClient>(x => new MongoClient(Configuration.GetConnectionString("ShowsDatabaseConnectionString")))
                .AddSingleton<IStateMonitor, StateMonitor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
