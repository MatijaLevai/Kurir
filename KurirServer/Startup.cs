using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using KurirServer.Helpers;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Swashbuckle.AspNetCore.Swagger;

namespace KurirServer
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
           
            services.AddDbContext<KurirDbContext>();
            services.AddAutoMapper(typeof(Startup));
            services.AddScopedServices();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "v1",
                    Version = "v1",
                    Description = "Eko kurir ASP.NET Core Web API"

                });

                c.OperationFilter<SwaggerDefaultValues>();
                //set 
                // Name of xml file in properties
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath, true);
            });
            services.AddOData();
            //services.AddSignalR();
            //  services.AddMvc(action => action.EnableEndpointRouting = true).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddMvc(options => {
                foreach (var outputFormatter in options.OutputFormatters.OfType<ODataOutputFormatter>().Where(_ =>_.SupportedMediaTypes.Count == 0))
                {
                    outputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }
                foreach (var inputFormatter in options.InputFormatters.OfType<ODataInputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
                {
                    inputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
          
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
           
            //app.UseSignalR(routes =>
            //{
            //    //routes.MapHub<DeliveryHub>("/deliveryHub");
            //}
            //);
            app.UseSwagger();

            //// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            //// specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
               
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                //c.RoutePrefix = "api/swagger" ;
            });

            app.UseMvc(routeBuilder => {
                routeBuilder.EnableDependencyInjection();
                routeBuilder.Expand().Select().OrderBy().Filter().Count();
                //routeBuilder.MapODataServiceRoute("oData", "oData", OdataModelBuilder.GetEdmModel());
            });
        }



    }
}
