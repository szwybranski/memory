using System;
using System.Collections.Generic;
using System.Text;
using MemoryLibrary;

namespace Memory
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length <= 0)
                {
                    Display display = new Display();
                    display.ConsoleOut(MemoryInterface.GetMemoryString("Console"));
                }
                else
                {
                    Display display = new Display();

                    switch (args[0])
                    {
                        case "-grade":
                        case "-g":
                            if (args.Length < 2)
                            {
                                throw new MemoryException("Missing grade value e.g. Memory -grade 5");
                            }
                            int gradeValue = 0;
                            if (!int.TryParse(args[1], out gradeValue) || gradeValue < 1 || gradeValue > 10)
                            {
                                throw new MemoryException("Unable to parse grade value, it should be integer between 1 - 10");
                            }

                            float prevGradeValue = 0.0f, currentGradeValue = 0.0f;
                            ConfigurationFile.GradeLatestMemoryString(gradeValue, out prevGradeValue, out currentGradeValue, "Console");

                            //Display Previous and Current grade
                            Console.Write("Previous grade: {0:N} Current grade: ", prevGradeValue);
                            ConsoleColor consoleFgColor = Console.ForegroundColor;
                            if (prevGradeValue < currentGradeValue)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("{0:N}", currentGradeValue);
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("{0:N}", currentGradeValue);
                            }
                            Console.ForegroundColor = consoleFgColor;
                            break;

                        case "-info":
                        case "-i":
                            string lastMemoryStringInfo = ConfigurationFile.DisplayLastMemoryStringInfo("Console");
                            display.ConsoleOut(lastMemoryStringInfo);
                            break;

                        case "-help":
                        case "/h":
                        default:
                            display.ConsoleOutError("'" + args[0] + "' is not recognized as a command", 0);
                            break;
                    }
                }
            }
            catch (MemoryException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
