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
using EasyCaching.InMemory;
using InMemoryProvider.Services;

namespace InMemoryProvider
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "InMemoryProvider", Version = "v1" });
            });


            //Important step for In-Memory Caching
            services.AddEasyCaching(options =>
            {
                // use memory cache with a simple way
                options.UseInMemory();

                // use memory cache with your own configuration
                //options.UseInMemory(config =>
                //{
                //    config.DBConfig = new InMemoryCachingOptions
                //    {
                //        // scan time, default value is 60s
                //        ExpirationScanFrequency = 60,
                //        // total count of cache items, default value is 10000
                //        SizeLimit = 100,

                //        // below two settings are added in v0.8.0
                //        // enable deep clone when reading object from cache or not, default value is true.
                //        EnableReadDeepClone = true,
                //        // enable deep clone when writing object to cache or not, default value is false.
                //        EnableWriteDeepClone = false,
                //    };
                //    // the max random second will be added to cache's expiration, default value is 120
                //    config.MaxRdSecond = 120;
                //    // whether enable logging, default is false
                //    config.EnableLogging = false;
                //    // mutex key's alive time(ms), default is 5000
                //    config.LockMs = 5000;
                //    // when mutex key alive, it will sleep some time, default is 300
                //    config.SleepMs = 300;
                //}, "default1");

            });

            services.AddSingleton<IDateTimeService, DateTimeService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "InMemoryProvider v1"));
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
