using System;
using Jdenticon.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Hyperledger.Aries.Storage;


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

                    option.AgentName = "MyEpicAgent";
                    option.WalletConfiguration = new WalletConfiguration { Id = "MyEpicWallet" };
                    option.WalletCredentials = new WalletCredentials { Key = "MyEpicKey" };
                    option.IssuerDid = "Th7MpTaRZVRYnPiabds81Y";
                    option.IssuerKeySeed = "000000000000000000000000Steward1";
                    option.GenesisFilename = "AriesTest.txn";
                    option.PoolName = "AriesTest";
                    option.EndpointUri = "http://localhost:7000";
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
