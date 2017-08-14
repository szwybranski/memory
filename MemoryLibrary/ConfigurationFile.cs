using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace MemoryLibrary
{
    /// <summary>
    /// This is representation of configuration file, dictionary of memoryfiles and it avg grade along with color.
    /// The most interesting is how it loads the configuration file:
    ///     -read configuration file (must exist) - contains memory file list (what user whats to learn) + color for displaing memory string
    ///     -read avg grades configuration file (if doesn't exist then create it) (filename, size, avg grade)
    ///     -foreach file check it size if diffrent then calc new avg grade and create new avg grade file
    /// </summary>
    public class ConfigurationFile
    {
        static string configFilename = "MemoryConfiguration.ini";
        static string latestFilename = "LatestDisplayed.log";

        private Dictionary<string, Grade> memoryFiles;

        public Dictionary<string, Grade> MemoryFiles
        {
            get { return memoryFiles; }
            set { memoryFiles = value; }
        }

        private int selectedColor;

        public int SelectedColor
        {
            get { return selectedColor; }
            set { selectedColor = value; }
        }

        public ConfigurationFile()
        {
            this.memoryFiles = new Dictionary<string, Grade>();
        }

        public static ConfigurationFile Open()
        {
            ConfigurationFile confFile = new ConfigurationFile();

            confFile.ReadConfigurationFile(ConfigurationFile.configFilename);

            try 
            {
                confFile.ReadConfigurationCacheFile(Path.GetFileNameWithoutExtension(ConfigurationFile.configFilename) + ".grades");
            }
            catch (FileNotFoundException) { confFile.GenerateConfigurationCacheFile(Path.GetFileNameWithoutExtension(ConfigurationFile.configFilename) + ".grades"); }
            catch (MemoryException) { confFile.GenerateConfigurationCacheFile(Path.GetFileNameWithoutExtension(ConfigurationFile.configFilename) + ".grades"); }
            
            return confFile;
        }

        private void ReadConfigurationFile(string filename)
        {
            string filepath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), filename);

            using (TextReader textReader = new StreamReader(filepath))
            {
                string line = null;
                bool readMemoryFiles = false;
                bool readColor = false;
                while (null != (line = textReader.ReadLine()))
                {
                    switch (line)
                    {
                        case "":
                            //Eat empty lines
                            break;

                        case "[Display Color (RGB)]":
                            readColor = true;
                            break;

                        case "[List of Memory Files]":
                            readMemoryFiles = true;
                            break;

                        default:
                            if (readColor)
                            {
                                string[] colorValues = line.Split(' ');
                                if (3 != colorValues.Length)
                                {
                                    throw new MemoryException("ConfigurationFile color parsing problem");
                                }
                                int red, green, blue;
                                if (int.TryParse(colorValues[0], out red) && int.TryParse(colorValues[1], out green) && int.TryParse(colorValues[2], out blue))
                                {
                                    this.SelectedColor = red << 16 + green << 8 + blue;
                                }
                                else
                                {
                                    throw new MemoryException("ConfigurationFile color parsing problem");
                                }
                                readColor = false;
                            }
                            else if (readMemoryFiles)
                            {
                                string absolutePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), line);
                                this.MemoryFiles.Add(absolutePath, new Grade(0.0f));
                            }
                            else
                            {
                                throw new MemoryException("ConfigurationFile parsing problem");
                            }
                            break;
                    }
                }

                //Close configuration file handle
                textReader.Close();
            }
        }

        private void ReadConfigurationCacheFile(string filename)
        {
            int totalCachedGrades = 0;
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
                            if (3 != brokenLine.Length)
                            {
                                throw new MemoryException("Configuration Cache File paring error");
                            }
                            DateTime modificationDate = new DateTime();
                            if (false == DateTime.TryParse(brokenLine[1], out modificationDate))
                            {
                                throw new MemoryException("Configuration Cache File paring error");
                            }
                            DateTime currentModificationDate = System.IO.File.GetLastWriteTime(brokenLine[0]);
                            if (modificationDate.ToString("s") != currentModificationDate.ToString("s"))
                            {
                                //TODO FUTURE: incremental grades update, not rereading all files
                                throw new MemoryException("Configuration Cache File is outdated");
                            }
                            else
                            {
                                float avgGrade;
                                if (false == float.TryParse(brokenLine[2], out avgGrade))
                                {
                                    throw new MemoryException("Configuration Cache File paring error");
                                }
                                if (this.MemoryFiles.ContainsKey(brokenLine[0]))
                                {
                                    this.MemoryFiles[brokenLine[0]].GradeValue = avgGrade;
                                    totalCachedGrades++;
                                }
                            }
                            break;
                    }
                }

                //Close handle to configuration cache file
                textReader.Close();
            }

            if (totalCachedGrades != this.MemoryFiles.Count)
            {
                //TODO FUTURE: incremental grades update, not rereading all files
                throw new MemoryException("Configuration Cache File is outdated");
            }
        }

        private void GenerateConfigurationCacheFile(string filename)
        {
            string gradesfilepath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), filename);

            using (TextWriter textWriter = new StreamWriter(gradesfilepath))
            {
                string[] keys = new string[this.MemoryFiles.Keys.Count];
                this.MemoryFiles.Keys.CopyTo(keys, 0);
                for(int i=0;i<keys.Length;i++)
                {
                    float avgGrade = MemoryFile.CalcAvgMemoryFileGrade(keys[i]);
                    this.MemoryFiles[keys[i]].GradeValue = avgGrade;

                    //write to cache file
                    DateTime currentModificationDate = System.IO.File.GetLastWriteTime(keys[i]);
                    textWriter.WriteLine(String.Format("{0} {1} {2}", keys[i], currentModificationDate.ToString("s"), avgGrade));
                }

                //Close handle to configuration cache file
                textWriter.Close();
            }
        }

        public static void WriteDownLatestMemoryString(string selectedMemoryFile, int selectedMemoryStringNumber, string applicationPrefix)
        {
            string latestPath = Path.Combine(Path.GetTempPath(), applicationPrefix + latestFilename);
            using (TextWriter textWriter = new StreamWriter(latestPath))
            {
                textWriter.WriteLine(selectedMemoryFile + " " + selectedMemoryStringNumber.ToString());
                textWriter.Close();
            }
        }

        public static string DisplayLastMemoryStringInfo(string applicationPrefix)
        {
            string lastMemoryStringInfo = string.Empty;
            string latestPath = Path.Combine(Path.GetTempPath(), applicationPrefix + latestFilename);
            using (TextReader textReader = new StreamReader(latestPath))
            {
                string line = textReader.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    throw new MemoryException("Unable to read infomation about latest memory string displayed");
                }
                lastMemoryStringInfo = line;
            }
            return lastMemoryStringInfo;
        }

        public static void GradeLatestMemoryString(int gradeValue, out float prevGradeValue, out float currentGradeValue, string applicationPrefix)
        {
            string latestPath = Path.Combine(Path.GetTempPath(), applicationPrefix + latestFilename);
            using (TextReader textReader = new StreamReader(latestPath))
            {
                string line = textReader.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    throw new MemoryException("Unable to read infomation about latest memory string displayed");
                }
                string[] brokenLine = line.Split(' ');
                if (brokenLine.Length != 2)
                {
                    throw new MemoryException("Unable to read infomation about latest memory string displayed");
                }
                string memoryFilename = brokenLine[0];
                int numberOfMemoryString = 0;
                if (!int.TryParse(brokenLine[1], out numberOfMemoryString))
                {
                    throw new MemoryException("Unable to read infomation about latest memory string displayed");
                }
                float newAvgGrade = MemoryFile.GradeMemoryString(memoryFilename, numberOfMemoryString, gradeValue, out prevGradeValue, out currentGradeValue);
                ConfigurationFile.SaveNewAvgGrade(memoryFilename, newAvgGrade);
            }
        }

        private static void SaveNewAvgGrade(string memoryFilename, float newAvgGrade)
        {
            string gradesfilepath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Path.GetFileNameWithoutExtension(ConfigurationFile.configFilename) + ".grades");
            List<string> cacheLines = new List<string>();

            using (TextReader textReader = new StreamReader(gradesfilepath))
            {
                string line = null;
                while (null != (line = textReader.ReadLine()))
                {
                    switch (line)
                    {
                        case "":
                            //Eat empty lines
                            cacheLines.Add(line);
                            break;

                        default:
                            string[] brokenLine = line.Split(' ');
                            if (3 != brokenLine.Length)
                            {
                                throw new MemoryException("Configuration Cache File paring error");
                            }
                            if (brokenLine[0] == memoryFilename)
                            {
                                cacheLines.Add(String.Format("{0} {1} {2}", memoryFilename, brokenLine[1], newAvgGrade));
                            }
                            else
                            {
                                cacheLines.Add(line);
                            }
                            break;
                    }
                }

                //Close handle to configuration cache file
                textReader.Close();
            }

            using (TextWriter textWriter = new StreamWriter(gradesfilepath))
            {
                foreach (string line in cacheLines)
                {
                    textWriter.WriteLine(line);
                }
                textWriter.Close();
            }
        }


    }

    //Float wrapper
    public class Grade
    {
        private float gradeValue;

        public float GradeValue
        {
            get { return gradeValue; }
            set { gradeValue = value; }
        }

        public Grade(float value)
        {
            this.GradeValue = value;
        }
    }
}
