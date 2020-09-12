namespace ZMachineConsole
{
    using Zmpp.Core.IO;
    using System;

    public class ConsoleOutput : IOutputStream
    {
        private bool _isSelected;

        public void close()
        {
            Console.Out.Close();
        }

        public void flush()
        {
            Console.Out.Flush();
        }

        public bool isSelected()
        {
            return _isSelected;
        }

        public void print(char zchar)
        {
            if (zchar == '\n' || zchar == '\r')
                Console.Out.WriteLine();
            else
                Console.Out.Write(zchar);
        }

        public void select(bool flag)
        {
            _isSelected = flag;
        }
    }
}
