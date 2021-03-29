using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyCaching.Core.Configurations;
using HybridProvider.Services;

namespace HybridProvider
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HybridProvider", Version = "v1" });
            });


            services.AddMvc();

            services.AddEasyCaching(option =>
            {
                // local
                option.UseInMemory("c1");
                // distributed
                option.UseRedis(config =>
                {
                    config.DBConfig.Endpoints.Add(new ServerEndPoint("127.0.0.1", 6379));
                    config.DBConfig.Database = 5;
                }, "c2");

                // combine local and distributed
                option.UseHybrid(config =>
                    {
                        config.TopicName = "test-topic";
                        config.EnableLogging = false;

                        // specify the local cache provider name after v0.5.4
                        config.LocalCacheProviderName = "c1";
                        // specify the distributed cache provider name after v0.5.4
                        config.DistributedCacheProviderName = "c2";
                    })
                    // use redis bus
                    .WithRedisBus(busConf =>
                    {
                        busConf.Endpoints.Add(new ServerEndPoint("127.0.0.1", 6379));
                    });
            });


            //Register Custom Services
            services.AddSingleton<ICacheService, CacheService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HybridProvider v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
