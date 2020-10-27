using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace PaymentGatewayAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            Microsoft.AspNetCore.WebHost.CreateDefaultBuilder(args)
           .UseStartup<Startup>();

    }
}
