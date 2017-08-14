using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace MemoryLibrary
{
    public class MemoryFile
    {
        private List<string> memoryStrings;

        public List<string> MemoryStrings
        {
            get { return memoryStrings; }
            set { memoryStrings = value; }
        }

        private List<float> memoryStringsGrades;

        public List<float> MemoryStringsGrades
        {
            get { return memoryStringsGrades; }
            set { memoryStringsGrades = value; }
        }

        public MemoryFile()
        {
            this.MemoryStrings = new List<string>();
            this.MemoryStringsGrades = new List<float>();
        }

        //public static global::Memory.MemoryFile Open(string selectedMemoryFile)
        public static MemoryFile Open(string selectedMemoryFile)
        {
            MemoryFile memoryFile = new MemoryFile();
            memoryFile.ReadMemoryFile(selectedMemoryFile);
            memoryFile.ReadMemoryFileGrades(Path.GetFileNameWithoutExtension(selectedMemoryFile) + ".grades");

            return memoryFile;
        }

        private void ReadMemoryFile(string path)
        {
            using (TextReader textReader = new StreamReader(path))
            {
                bool firstMemoryString = true;
                string line = null;
                StringBuilder sb = new StringBuilder();
                while (null != (line = textReader.ReadLine()))
                {
                    if (firstMemoryString)
                    {
                        sb.AppendLine(line);
                        firstMemoryString = false;
                    }
                    else
                    {
                        string[] brokenLine = line.Split('.');
                        int lineCounter;
                        if (brokenLine.Length>0 && int.TryParse(brokenLine[0], out lineCounter) && lineCounter == this.MemoryStrings.Count + 2)
                        {
                            memoryStrings.Add(sb.ToString());
                            sb.Remove(0, sb.Length);
                        }
                        sb.AppendLine(line);
                    }
                }
                //Add last MemoryString from MemoryFile
                if (sb.Length > 0)
                {
                    memoryStrings.Add(sb.ToString());
                }

                //Close handle to MemoryFile
                textReader.Close();
            }
        }

        private void ReadMemoryFileGrades(string filename)
        {
            int totalGrades = 0;
            string gradesfilepath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), filename);

            using (TextReader textReader = new StreamReader(gradesfilepath))
            {
                string line = null;
                while (null != (line = textReader.ReadLine()))
                {
                    switch (line)
                    {
                        case "":
                            //Eat empty lines
                            break;

                        default:
                            string[] brokenLine = line.Split(' ');
                            if (2 != brokenLine.Length)
                            {
                                throw new MemoryException(String.Format("[{0}] MemoryStrings Grade File paring error", gradesfilepath));
                            }

                            int memoryStringNo;
                            if (false == int.TryParse(brokenLine[0], out memoryStringNo))
                            {
                                throw new MemoryException(String.Format("[{0}] MemoryStrings Grade File paring error", gradesfilepath));
                            }
                            if (memoryStringNo != totalGrades + 1)
                            {
                                //There must be a strinct sequence in grades
                                throw new MemoryException(String.Format("[{0}] MemoryStrings Grade File paring error", gradesfilepath));
                            }

                            float memoryStringGrade;
                            if (false == float.TryParse(brokenLine[1], out memoryStringGrade))
                            {
                                throw new MemoryException(String.Format("[{0}] MemoryStrings Grade File paring error", gradesfilepath));
                            }

                            //this.MemoryStringsGrades[memoryStringNo] = memoryStringGrade;
                            this.memoryStringsGrades.Add(memoryStringGrade);
                            totalGrades++;

                            break;
                    }
                }

                //Close handle to MemoryString Grades file
                textReader.Close();
            }
        }

        public static float CalcAvgMemoryFileGrade(string path)
        {
            float avgGrade;

            try
            {
                avgGrade = MemoryFile.CalcAvgGradeFromMemoryFile(path);
            }
            catch (FileNotFoundException) { avgGrade = GenerateMemoryFileGrades(path); }
            catch (MemoryException e) { Console.WriteLine(e.Message); avgGrade = GenerateMemoryFileGrades(path); }

            return avgGrade;
        }

        private static float CalcAvgGradeFromMemoryFile(string path)
        {
            float avgGrade = 0.0f;

            int totalMemoryStrings = MemoryFile.CountMemoryStrings(path);

            //Read grades if counts!= then append grades with 0.0
            int totalGrades = 0;
            string bareFilename = Path.GetFileNameWithoutExtension(Path.GetFileName(path));
            string gradesfilepath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), bareFilename + ".grades");

            using (TextReader textReader = new StreamReader(gradesfilepath))
            {
                string line = null;
                while (null != (line = textReader.ReadLine()))
                {
                    switch (line)
                    {
                        case "":
                            //Eat empty lines
                            break;

                        default:
                            string[] brokenLine = line.Split(' ');
                            if (2 != brokenLine.Length)
                            {
                                throw new MemoryException(String.Format("[{0}] MemoryStrings Grade File paring error", gradesfilepath));
                            }

                            int memoryStringNo;
                            if (false == int.TryParse(brokenLine[0], out memoryStringNo))
                            {
                                throw new MemoryException(String.Format("[{0}] MemoryStrings Grade File paring error", gradesfilepath));
                            }
                            if (memoryStringNo != totalGrades + 1)
                            {
                                //There must be a strinct sequence in grades
                                throw new MemoryException(String.Format("[{0}] MemoryStrings Grade File paring error", gradesfilepath));
                            }

                            float memoryStringGrade;
                            if (false == float.TryParse(brokenLine[1], out memoryStringGrade))
                            {
                                throw new MemoryException(String.Format("[{0}] MemoryStrings Grade File paring error", gradesfilepath));
                            }

                            avgGrade += memoryStringGrade;
                            totalGrades++;

                            break;
                    }
                }

                //Close handle to MemoryString Grades file
                textReader.Close();
            }

            if (totalGrades != totalMemoryStrings)
            {
                //Append 0.0 grades for new MemoryStrings
                using (TextWriter textWriter = new StreamWriter(gradesfilepath, true))
                {
                    for (int i = totalGrades + 1; i <= totalMemoryStrings; i++)
                    {
                        textWriter.WriteLine(String.Format("{0} {1}", i, 0.0f));
                    }

                    //Close handle to MemoryString Grades file
                    textWriter.Close();
                }
            }

            return avgGrade / totalMemoryStrings;
        }

        private static float GenerateMemoryFileGrades(string path)
        {
            int totalMemoryStrings = MemoryFile.CountMemoryStrings(path);

            string bareFilename = Path.GetFileNameWithoutExtension(Path.GetFileName(path));
            string gradesfilepath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), bareFilename + ".grades");

            using (TextWriter textWriter = new StreamWriter(gradesfilepath))
            {
                for (int i = 1; i <= totalMemoryStrings; i++)
                {
                    textWriter.WriteLine(String.Format("{0} {1}", i, 0.0f));
                }

                //Close handle to MemoryString Grades file
                textWriter.Close();
            }

            return 0.0f;
        }

        public static int CountMemoryStrings(string path)
        {
            int totalMemoryStrings = 0;

            using (TextReader textReader = new StreamReader(path))
            {
                string line = null;
                while (null != (line = textReader.ReadLine()))
                {
                    int lineCounter;
                    string[] brokenLine = line.Split('.');
                    if(brokenLine.Length>0 && int.TryParse(brokenLine[0], out lineCounter) && lineCounter == totalMemoryStrings + 1)
                    {
                        totalMemoryStrings++;
                    }
                }

                //Close hadle to MemoryString file
                textReader.Close();
            }

            return totalMemoryStrings;
        }


        public static int GetNumberOutOfMemoryString(string memoryString)
        {
            int memoryStringNumber = 0;
            string[] brokenMemoryString = memoryString.Split('.');
            
            if(!int.TryParse(brokenMemoryString[0], out memoryStringNumber))
            {
                throw new MemoryException("Unable to parse memory string number out of memory string");
            }

            return memoryStringNumber;
        }

        public static string GetMemoryStringWithoutNumber(string memoryString)
        {
            int dotPosition = memoryString.IndexOf('.');
            if (dotPosition < 0)
            {
                throw new MemoryException("Unable to parse memory string number out of memory string");
            }
            string memoryStringWithoutNumber = memoryString.Substring(dotPosition + 1);
            if (string.IsNullOrEmpty(memoryStringWithoutNumber))
            {
                throw new MemoryException("Unable to parse memory string number out of memory string");
            }

            return memoryStringWithoutNumber;
        }



        public static float GradeMemoryString(string memoryFilename, int memoryFileToGrade, int gradeValue, out float prevGradeValue, out float currentGradeValue)
        {
            float newAvgGrade = 0.0f;
            string bareFilename = Path.GetFileNameWithoutExtension(Path.GetFileName(memoryFilename));

            MemoryFile memoryFile = new MemoryFile();
            memoryFile.ReadMemoryFileGrades(bareFilename + ".grades");
            prevGradeValue = memoryFile.MemoryStringsGrades[memoryFileToGrade - 1];
            if (0 == prevGradeValue)
            {
                memoryFile.MemoryStringsGrades[memoryFileToGrade - 1] = gradeValue;
            }
            else
            {
                memoryFile.MemoryStringsGrades[memoryFileToGrade - 1] = (gradeValue + prevGradeValue) / 2.0f;
            }
            currentGradeValue = memoryFile.MemoryStringsGrades[memoryFileToGrade - 1];
            newAvgGrade = memoryFile.SaveGradesFile(bareFilename + ".grades");

            return newAvgGrade;
        }


        private float SaveGradesFile(string gradesFilename)
        {
            float newAvgGrade = 0.0f;
            string gradesfilepath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), gradesFilename);

            using (TextWriter textWriter = new StreamWriter(gradesfilepath))
            {
                for (int i = 0; i < this.MemoryStringsGrades.Count; i++)
                {
                    textWriter.WriteLine(String.Format("{0} {1}", i+1, this.MemoryStringsGrades[i]));
                    newAvgGrade += this.MemoryStringsGrades[i];
                }

                //Close handle to MemoryString Grades file
                textWriter.Close();
            }

            return newAvgGrade/this.MemoryStringsGrades.Count;
        }
    }
}
