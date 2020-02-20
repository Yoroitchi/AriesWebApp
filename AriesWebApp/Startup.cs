using System;
using Jdenticon.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Hyperledger.Aries.Storage;
using Hyperledger.Indy.DidApi;


namespace AriesWebApp
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
            services.AddMvc();
            services.AddLogging();

            //Register agent framework dependy service and handlers
            services.AddAriesFramework(builder =>
            {
                builder.RegisterAgent(option =>
                {

                    option.AgentName = "Holder";
                    option.WalletConfiguration = new WalletConfiguration { Id = "Holder" };
                    option.WalletCredentials = new WalletCredentials { Key = "HolderKey" };
                    option.GenesisFilename = "AriesTest.txn";
                    option.PoolName = "Aries";
                    option.EndpointUri = "http://127.0.0.1:5000";
                    option.ProtocolVersion = 2;
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            app.UseAriesFramework();
            app.UseJdenticon();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(routes =>
            {
                routes.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
