using System.Threading.Tasks;
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
		           	webBuilder.UseStartup<Startup>();
                   	webBuilder.UseUrls("http://127.0.0.1:8000");
		        });
    }
}
