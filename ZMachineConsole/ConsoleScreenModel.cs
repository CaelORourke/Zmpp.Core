namespace ZMachineConsole
{
    using Zmpp.Core.IO;
    using Zmpp.Core.UI;
    using System;

    /// <summary>
    /// Represents a <see cref="System.Console"/> screen model.
    /// </summary>
    public class ConsoleScreenModel : IScreenModel
    {
        private readonly IOutputStream output;

        public ConsoleScreenModel()
        {
            output = new ConsoleOutput();
        }

        public void EraseWindow(int window)
        {
            Console.Clear();
        }

        public IOutputStream OutputStream => output;

        public void Reset()
        {
            Console.Clear();
        }

        public void SetBackground(int colornumber, int window)
        {
            switch(colornumber){
                case 3:
                    Console.BackgroundColor = ConsoleColor.Red;
                    break;
                case 4:
                    Console.BackgroundColor = ConsoleColor.Green;
                    break;
                case 5:
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    break;
                case 6:
                    Console.BackgroundColor = ConsoleColor.Blue;
                    break;
                case 7:
                    Console.BackgroundColor = ConsoleColor.Magenta;
                    break;
                case 8:
                    Console.BackgroundColor = ConsoleColor.Cyan;
                    break;
                case 9:
                    Console.BackgroundColor = ConsoleColor.White;
                    break;
                case 10:
                    // 10 = light grey(true $5AD6, $$0101101011010110)
                    Console.BackgroundColor = ConsoleColor.Gray;
                    break;
                case 11:
                    // 11 = medium grey(true $4631, $$0100011000110001)
                    Console.BackgroundColor = ConsoleColor.Gray;
                    break;
                case 12:
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    break;
                default:
                    Console.BackgroundColor = ConsoleColor.Black;
                    break;
            }
        }

        public void SetForeground(int colornumber, int window)
        {
            switch (colornumber)
            {
                case 3:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case 4:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case 5:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case 6:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case 7:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case 8:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case 9:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case 10:
                    // 10 = light grey(true $5AD6, $$0101101011010110)
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case 11:
                    // 11 = medium grey(true $4631, $$0100011000110001)
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case 12:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
        }

        public void SetTextStyle(int style)
        {
            // TODO: Support font styles.
            // TODO: Add constants for style. Roman (if 0), Reverse Video (if 1), Bold (if 2), Italic (4), Fixed Pitch (8)
        }

        #region Not Implemented

        public int ActiveWindow => throw new NotImplementedException();

        public TextAnnotation BottomAnnotation => throw new NotImplementedException();

        public void EraseLine(int value) => throw new NotImplementedException();

        public void SetBufferMode(bool flag) => throw new NotImplementedException();

        public char SetFont(char fontnumber) => throw new NotImplementedException();

        public void SetTextCursor(int line, int column, int window) => throw new NotImplementedException();

        public void SetWindow(int window) => throw new NotImplementedException();

        public void SplitWindow(int linesUpperWindow) => throw new NotImplementedException();

        public ITextCursor TextCursor => throw new NotImplementedException();

        public TextAnnotation TopAnnotation => throw new NotImplementedException();

        #endregion
    }
}
