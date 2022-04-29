namespace ZMachineConsole
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("I need the path or URL for a story file.");
                Environment.Exit(-1);
            }

            Console.Clear();

            var services = ConfigureServices();
            using (var serviceProvider = services.BuildServiceProvider())
            {
                serviceProvider.GetService<ConsoleApplication>().Run(args[0]);
            }
        }

        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddLogging(configure => configure.AddDebug());
            services.AddSingleton<ConsoleApplication>();
            return services;
        }
    }
}
