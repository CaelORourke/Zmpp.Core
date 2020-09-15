namespace ZMachineConsole
{
    using Zmpp.Core.IO;
    using Zmpp.Core.UI;
    using System;

    public class ConsoleScreenModel : IScreenModel
    {
        private readonly IOutputStream output;

        public ConsoleScreenModel()
        {
            output = new ConsoleOutput();
        }

        public void eraseLine(int value)
        {
            throw new NotImplementedException();
        }

        public void eraseWindow(int window)
        {
            throw new NotImplementedException();
        }

        public int getActiveWindow()
        {
            throw new NotImplementedException();
        }

        public TextAnnotation getBottomAnnotation()
        {
            throw new NotImplementedException();
        }

        public IOutputStream getOutputStream()
        {
            return output;
        }

        public ITextCursor getTextCursor()
        {
            throw new NotImplementedException();
        }

        public TextAnnotation getTopAnnotation()
        {
            throw new NotImplementedException();
        }

        public void reset()
        {
            throw new NotImplementedException();
        }

        public void setBackground(int colornumber, int window)
        {
            Console.BackgroundColor = (ConsoleColor)colornumber;
        }

        public void setBufferMode(bool flag)
        {
            throw new NotImplementedException();
        }

        public char setFont(char fontnumber)
        {
            throw new NotImplementedException();
        }

        public void setForeground(int colornumber, int window)
        {
            // TODO: Seems like the same colornumber is being sent for both foreground and background color?
            //Console.ForegroundColor = (ConsoleColor)colornumber;
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void setTextCursor(int line, int column, int window)
        {
            throw new NotImplementedException();
        }

        public void setTextStyle(int style)
        {
            throw new NotImplementedException();
        }

        public void setWindow(int window)
        {
            throw new NotImplementedException();
        }

        public void splitWindow(int linesUpperWindow)
        {
            throw new NotImplementedException();
        }
    }
}
