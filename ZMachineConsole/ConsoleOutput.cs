namespace ZMachineConsole
{
    using Zmpp.Core.IO;
    using System;

    public class ConsoleOutput : IOutputStream
    {
        private bool isSelected;

        public void Close()
        {
            Console.Out.Close();
        }

        public void Flush()
        {
            Console.Out.Flush();
        }

        public bool IsSelected()
        {
            return isSelected;
        }

        public void Print(char zchar)
        {
            if (zchar == '\n' || zchar == '\r')
                Console.Out.WriteLine();
            else
                Console.Out.Write(zchar);
        }

        public void Select(bool flag)
        {
            isSelected = flag;
        }
    }
}
