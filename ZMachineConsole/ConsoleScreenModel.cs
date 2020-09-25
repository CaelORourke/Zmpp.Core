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

        public void EraseLine(int value)
        {
            throw new NotImplementedException();
        }

        public void EraseWindow(int window)
        {
            Console.Clear();
        }

        public int ActiveWindow => throw new NotImplementedException();

        public TextAnnotation BottomAnnotation => throw new NotImplementedException();

        public IOutputStream OutputStream => output;

        public ITextCursor TextCursor => throw new NotImplementedException();

        public TextAnnotation TopAnnotation => throw new NotImplementedException();

        public void Reset()
        {
            Console.Clear();
        }

        public void SetBackground(int colornumber, int window)
        {
            // TODO: Support colors!!!
            //Console.BackgroundColor = (ConsoleColor)colornumber;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        public void SetBufferMode(bool flag)
        {
            throw new NotImplementedException();
        }

        public char SetFont(char fontnumber)
        {
            throw new NotImplementedException();
        }

        public void SetForeground(int colornumber, int window)
        {
            // TODO: Support colors!!!
            //Console.ForegroundColor = (ConsoleColor)colornumber;
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void SetTextCursor(int line, int column, int window)
        {
            throw new NotImplementedException();
        }

        public void SetTextStyle(int style)
        {
            // TODO: Support font styles.
            // TODO: Add constants for style. Roman (if 0), Reverse Video (if 1), Bold (if 2), Italic (4), Fixed Pitch (8)
        }

        public void SetWindow(int window)
        {
            throw new NotImplementedException();
        }

        public void SplitWindow(int linesUpperWindow)
        {
            throw new NotImplementedException();
        }
    }
}
