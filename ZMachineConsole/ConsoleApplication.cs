namespace ZMachineConsole
{
    using Microsoft.Extensions.Logging;
    using System;

    /// <summary>
    /// Represents a simple console Z-machine.
    /// </summary>
    public class ConsoleApplication
    {
        private readonly ILogger logger;

        public ConsoleApplication(ILogger<ConsoleApplication> logger)
        {
            this.logger = logger;
        }

        internal void Run(string storyFilePath)
        {
            try
            {
                var console = new ConsoleViewModel(storyFilePath);
                var interpreter = new Interpreter(logger, console);

                interpreter.Open(storyFilePath);

                interpreter.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"The application gasped, \"{ex.Message}\" before terminating unexpectedly.\n");
                Console.WriteLine($"\"{ex.StackTrace}\"");
            }
        }
    }
}
