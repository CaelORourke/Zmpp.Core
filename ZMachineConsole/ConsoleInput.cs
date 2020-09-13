namespace ZMachineConsole
{
    using Zmpp.Core.IO;
    using Zmpp.Core.Vm;
    using System;
    using System.Globalization;

    public class ConsoleInput : IInputStream
    {
        private IMachine machine;

        public void Init(IMachine machine)
        {
            this.machine = machine;
        }

        public void close()
        {
            Console.In.Close();
        }

        private String convertToZsciiInputLine(String input)
        {
            return machine.ToZsciiString(input.ToLower(CultureInfo.DefaultThreadCurrentCulture)) + "\r";
        }

        public string readLine()
        {
            return convertToZsciiInputLine(Console.In.ReadLine());
        }
    }
}
