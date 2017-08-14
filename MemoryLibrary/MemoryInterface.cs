using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryLibrary
{
    public class MemoryInterface
    {
        public static string GetMemoryString(string applicationPrefix)
        {
            //Access configuration file
            ConfigurationFile confFile = ConfigurationFile.Open();

            //Pick memoryFile from which we will pick memory string to learn
            Engine memoryEngine = new Engine();
            string selectedMemoryFile = memoryEngine.PickMemoryFile(confFile.MemoryFiles);

            //Read all strings and grades for selected file
            MemoryFile memoryFile = MemoryFile.Open(selectedMemoryFile);

            //Calculate memory string to learn
            string selectedMemoryString = memoryEngine.PickMemoryString(memoryFile);

            //Write down this memory string, so it can be graded
            int selectedMemoryStringNumber = MemoryFile.GetNumberOutOfMemoryString(selectedMemoryString);
            ConfigurationFile.WriteDownLatestMemoryString(selectedMemoryFile, selectedMemoryStringNumber, applicationPrefix);

            //Display selected string with selected color in conf file
            //Display display = new Display(confFile.SelectedColor);

            return MemoryFile.GetMemoryStringWithoutNumber(selectedMemoryString);
        }
    }
}
