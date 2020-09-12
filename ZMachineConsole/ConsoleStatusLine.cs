namespace ZMachineConsole
{
    using Zmpp.Core.UI;
    using System;

    public class ConsoleStatusLine : IStatusLine
    {
        public void updateStatusScore(string objectName, int score, int steps)
        {
            //// capture the original values
            //var left = Console.CursorLeft;
            //var top = Console.CursorTop;
            //var foreground = Console.ForegroundColor;
            //var background = Console.BackgroundColor;
            var padding = Console.WindowWidth - 3 - objectName.Length - score.ToString().Length - steps.ToString().Length;

            // display the score
            //Console.CursorLeft = 0;
            //Console.CursorTop = 0;
            //Console.ForegroundColor = background;
            //Console.BackgroundColor = foreground;
            //Console.Write(" {0}{1}{2}/{3} ", objectName, new String(' ', padding), score, steps);
            Console.Title = String.Format(" {0}{1}{2}/{3} ", objectName, new String(' ', padding), score, steps);

            //// move the cursor back to its original position
            //Console.CursorLeft = left;
            //Console.CursorTop = top;
            //Console.ForegroundColor = foreground;
            //Console.BackgroundColor = background;
        }

        public void updateStatusTime(string objectName, int hours, int minutes)
        {
            var padding = Console.WindowWidth - 3 - objectName.Length - hours.ToString().Length - minutes.ToString().Length;
            Console.Title = String.Format(" {0}{1}{2}:{3} ", objectName, new String(' ', padding), hours, minutes);
        }
    }
}
