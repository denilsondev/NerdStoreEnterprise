using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using NSE.Identidade.API.Data;
using Microsoft.OpenApi.Models;
using System;

namespace NSE.Identidade.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IHostEnvironment hostEnvironment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(hostEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{hostEnvironment.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables();

            if (hostEnvironment.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddControllers();

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo 
                {
                    Title = "NerdStore Enterprise Identity API",
                    Description = "Esta API faz parte do curso ASP .NET Core Enterprise Application",
                    Contact = new OpenApiContact() {Name = "Denilson", Email = "contato@dev.io"},
                    License = new OpenApiLicense() {Name = "MIT", Url = new Uri("http://opensource.org")}
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            });

            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
