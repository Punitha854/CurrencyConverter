using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyConverter.Models;
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

namespace CurrencyConverter
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
            services.AddDbContext<CurrencyDBContext>(opts => opts.UseInMemoryDatabase(Configuration["ConnectionString"]));

            services.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds(x => x.FullName);


                c.IncludeXmlComments("CurrencyConverter.xml", true);
                
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "CurrencyConverter API",
                    Description = "REST APIs used to convert between different currencies",
                    TermsOfService = new Uri("https://openexchangerates.org/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Punitha Guruswamy",
                        Email = string.Empty,
                        Url = new Uri("https://twitter.com/pg"),
                    }
                   
                });
            });

            services.AddCors(c => c.AddPolicy("AllowOrigin", option => option.AllowAnyOrigin()));
            services.AddCors(options =>
            { 

#if DEBUG
            options.AddPolicy(
              "CorsPolicy",
              builder => builder.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
        });
#else
                options.AddPolicy(
                  "CorsPolicy",
                  builder => builder.WithOrigins("http://localhost:4200") 
                  //http://localhost:4200")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials());
                 });
#endif

        }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseCors("CorsPolicy");

            app.UseCors(options => options.AllowAnyOrigin());
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Currency OPS API V1");
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
