using System;
using System.Collections.Generic;
using System.Text;

namespace Memory
{
    class Display
    {
        private int selectedColor;

        public Display()
        {

        }

        public Display(int selectedColor)
        {
            this.selectedColor = selectedColor;
        }

        //TODO: load win32 color changer and use specified color 
        public void ConsoleOut(string memoryString)
        {
            //Save console colors
            ConsoleColor consoleBgColor = Console.BackgroundColor;
            ConsoleColor consoleFgColor = Console.ForegroundColor;

            //Pick visible color for string
            if (ConsoleColor.White == consoleBgColor)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
            //Write on output
            Console.Write(memoryString);

            //Restore colors
            Console.BackgroundColor = consoleBgColor;
            Console.ForegroundColor = consoleFgColor;
        }

        public void ConsoleOutError(string message, int level)
        {
            Console.WriteLine(message);
        }
    }
}
