namespace ZMachineConsole
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    class Program
    {
        static void Main(string[] args)
        {
            var storyFilePath = @"C:\shane\projects\zork1.z5";

            var services = ConfigureServices();
            using (ServiceProvider serviceProvider = services.BuildServiceProvider())
            {
                serviceProvider.GetService<ConsoleApplication>().Run(storyFilePath);
            }
        }

        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();
            //services.AddTransient<ITestService, TestService>();
            services.AddLogging(configure => configure.AddDebug());
            services.AddSingleton<ConsoleApplication>();
            return services;
        }
    }
}
