using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;
using API.Data;
using API.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;           
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            // var serviceProvider = services.BuildServiceProvider();
            // var logger = serviceProvider.GetService<ILogger<EmployeesController>>();
            // services.AddSingleton(typeof(ILogger), logger);
            services.AddControllers(o => o.InputFormatters.Insert(o.InputFormatters.Count, new TextPlainInputFormatter()));
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(_config.GetConnectionString("DefaultConnection"));
            });
            services.AddControllers();
            services.AddCors();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });
        }

        public void ConfigureProductionServices(IServiceCollection services)
        {
            // var serviceProvider = services.BuildServiceProvider();
            // var logger = serviceProvider.GetService<ILogger<EmployeesController>>();
            // services.AddSingleton(typeof(ILogger), logger);
            services.AddControllers(o => o.InputFormatters.Insert(o.InputFormatters.Count, new TextPlainInputFormatter()));
            services.AddDbContext<DataContext>(options =>
            {
                options.UseMySql(_config.GetConnectionString("DefaultConnection"), new MariaDbServerVersion(new Version(10, 3, 15)));
            });
            services.AddControllers();
            services.AddCors();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));
            //app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseAuthorization();

            app.UseDefaultFiles();
            app.UseStaticFiles();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToController("Index", "Fallback");
            });
        }
    }
}


