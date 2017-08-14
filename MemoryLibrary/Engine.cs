using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryLibrary
{
    public class Engine
    {
        private Random rand;

        public Engine()
        {
            this.rand = new Random((int)DateTime.Now.ToBinary());
        }

        /// <summary>
        /// TODO: change boundary conditions, check overflow situations (int overflow), check does each memoryFile can be selected fairly
        /// </summary>
        /// <param name="memoryFiles"></param>
        /// <returns></returns>
        public string PickMemoryFile(Dictionary<string, Grade> memoryFiles)
        {
            string selectedMemoryFile = string.Empty;

            //Multiplay by 10 to move comma - increase learning process acuracy
            int highestGrade = 0;
            foreach (KeyValuePair<string, Grade> kv in memoryFiles)
            {
                memoryFiles[kv.Key].GradeValue *= 10;
                if (kv.Value.GradeValue > highestGrade)
                {
                    highestGrade = (int)Math.Ceiling(kv.Value.GradeValue);
                }
            }
            highestGrade++;

            //Revert grades and calc total sum
            int totalSum = 0;
            foreach (KeyValuePair<string, Grade> kv in memoryFiles)
            {
                int revertedGrade = (int)(highestGrade - kv.Value.GradeValue);
                memoryFiles[kv.Key].GradeValue = revertedGrade;
                totalSum += revertedGrade;
            }

            int currentSum = 0;
            int randValue = rand.Next(1, totalSum + 1);
            foreach (KeyValuePair<string, Grade> kv in memoryFiles)
            {
                currentSum += (int)kv.Value.GradeValue;

                if (currentSum >= randValue)
                {
                    selectedMemoryFile = kv.Key;
                    break;
                }
            }

            if (string.Empty == selectedMemoryFile)
            {
                throw new MemoryException("Engine selection error");
            }

            return selectedMemoryFile;
        }

        public string PickMemoryString(MemoryFile memoryFile)
        {
            string selectedMemoryString = string.Empty;

            //Multiplay by 10 to move comma - increase learning process acuracy
            int highestGrade = 0;
            for (int i = 0; i < memoryFile.MemoryStringsGrades.Count; i++)
            {
                memoryFile.MemoryStringsGrades[i] *= 10;
                if (memoryFile.MemoryStringsGrades[i] > highestGrade)
                {
                    highestGrade = (int)Math.Ceiling(memoryFile.MemoryStringsGrades[i]);
                }
            }
            highestGrade++;

            //Revert grades and calc total sum
            int totalSum = 0;
            for (int i = 0; i < memoryFile.MemoryStringsGrades.Count; i++)
            {
                int revertedGrade = (int)(highestGrade - memoryFile.MemoryStringsGrades[i]);
                memoryFile.MemoryStringsGrades[i] = revertedGrade;
                totalSum += revertedGrade;
            }

            int currentSum = 0;
            int randValue = rand.Next(1, totalSum + 1);
            for (int i = 0; i < memoryFile.MemoryStringsGrades.Count; i++)
            {
                currentSum += (int)memoryFile.MemoryStringsGrades[i];

                if (currentSum >= randValue)
                {
                    selectedMemoryString = memoryFile.MemoryStrings[i];
                    break;
                }
            }

            if (string.Empty == selectedMemoryString)
            {
                throw new MemoryException("Engine selection error");
            }

            return selectedMemoryString;
        }
    }
}
