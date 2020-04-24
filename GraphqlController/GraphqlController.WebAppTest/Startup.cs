using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using GraphqlController.Services;
using GraphiQl;
using GraphqlController.WebAppTest.Repositories;
using GraphqlController.AspNetCore;
using GraphqlController.WebAppTest.Types;

namespace GraphqlController.WebAppTest
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

            services.AddGraphQlController()
                    .AddCurrentAssembly();
                       
            services.AddGraphQlEndpoint();

            services.AddScoped<TeacherRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseGraphiQl("/graphi", "/graphql/root");

            app.UseHttpsRedirection();

            app.UseRouting();
                        
            app.UseAuthorization();

            app.UseGraphQLController();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQLEnpoint<Root>("/graphql/root");
                endpoints.MapGraphQLEnpoint<Root2>("/graphql/root2");
                endpoints.MapControllers();
            });
        }
    }
}
