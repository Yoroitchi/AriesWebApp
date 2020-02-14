using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace AriesWebApp
{
    public class Program
    {
        public static Task Main(string[] args) => CreateHostBuilder(args).Build().RunAsync();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                    /*
                        Uncomment the line below if you want the application to run in a docker container.
                        In this case, when running the container, make sure the url is like following : http://[::1]:7000
                     */
                    webBuilder.UseUrls("http://+:7000");
                    });
    }
}
