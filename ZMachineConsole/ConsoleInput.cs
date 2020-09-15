namespace ZMachineConsole
{
    using System;
    using Zmpp.Core.IO;

    public class ConsoleInput : IInputStream
    {
        public void Close()
        {
            Console.In.Close();
        }

        public string ReadLine()
        {
            return Console.In.ReadLine();
        }
    }
}
