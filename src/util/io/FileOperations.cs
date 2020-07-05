// just uncomment this line to restrict file access to KSP installation folder
#define _UNLIMITED_FILE_ACCESS
// for debugging
// #define _DEBUG

using System;
using System.IO;
using UnityEngine;

namespace AutoQuickSaveSystem
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class FileOperations : MonoBehaviour
    {

        private static String ROOT_PATH;
        private static String CONFIG_BASE_FOLDER;

        void Start()
        {
            ROOT_PATH = KSPUtil.ApplicationRootPath;
            CONFIG_BASE_FOLDER = ROOT_PATH + "GameData/";
            FilePath = "PluginData/AutoQuickSaveSystem.cfg";
        }
        public static bool InsideApplicationRootPath(String path)
        {
            if (path == null) return false;
            try
            {
                String fullpath = Path.GetFullPath(path);
                return fullpath.StartsWith(Path.GetFullPath(ROOT_PATH));
            }
            catch
            {
                return false;
            }
        }

        public static bool ValidPathForWriteOperation(String path)
        {
#if (_UNLIMITED_FILE_ACCESS)
            return true;
#else
            String fullpath = Path.GetFullPath(path);
            return InsideApplicationRootPath(fullpath);
#endif
        }

        private static void CheckPathForWriteOperation(String path)
        {
            if (!ValidPathForWriteOperation(path))
            {
                Log.Error("invalid write path: " + path);
                throw new InvalidOperationException("write path outside KSP home folder: " + path);
            }
        }


        public static void DeleteFile(String file)
        {
            CheckPathForWriteOperation(file);
            Log.Info("deleting file " + file);
            File.Delete(file);
        }

        public static void CopyFile(String from, String to)
        {
            CheckPathForWriteOperation(to);
            Log.Info("copy file " + from + " to " + to);
            File.Copy(from, to);
        }

        public static void CopyDirectory(String from, String to, String excludemarkerfile = ".nobackup")
        {
            if (FileExists(from + "/" + excludemarkerfile))
            {
                Log.Info("directory '" + from + "' excluded from backup (marked by file)");
                return;
            }
            Log.Detail("no exclude marker file '" + excludemarkerfile + "' found in folder '" + from + "'");

            string dirName = new DirectoryInfo(from).Name;
            foreach (var e in ConfigNodeIO.excludes)
            {
                if (dirName == e)
                {
                    Log.Info("directory '" + dirName + "' excluded from backup (excluded by config)");
                    return;
                }
            }
            Log.Detail("folder '" + from + "' not in exclude list");

            CheckPathForWriteOperation(to);

            Log.Info("copy directory " + from + " to " + to);

            // create target directory if not existient
            if (!DirectoryExists(to))
            {
                CreateDirectoryRetry(to);
            }

            String[] files = GetFiles(from);
            foreach (String file in files)
            {
                String name = GetFileName(file);
                CopyFileRetry(file, to + "/" + name);
            }
            String[] folders = GetDirectories(from);
            foreach (String folder in folders)
            {
                String name = GetFileName(folder);
                CreateDirectoryRetry(to + "/" + name);
                CopyDirectory(folder, to + "/" + name);
            }
        }

        public static void CreateDirectory(String directory)
        {
            CheckPathForWriteOperation(directory);
            Log.Info("creating directory " + directory);
            Directory.CreateDirectory(directory);
        }

        public static void DeleteDirectory(String directory)
        {
            CheckPathForWriteOperation(directory);
            Log.Info("deleting directory " + directory);
            Directory.Delete(directory, true);
        }

        public static void CreateFile(String file)
        {
            CheckPathForWriteOperation(file);
            Log.Info("creating file " + file);
            File.Create(file);
        }

        public static void CreateDirectoryRetry(String directory, int retries = 3, int delayinMillis = 500)
        {
            do
            {
                try
                {
                    CreateDirectory(directory);
                    return;
                }
                catch (Exception e)
                {
                    Log.Exception("CreateDirectoryRetry", e);
                    if (retries > 0)
                    {
                        retries--;
                        Log.Info("retrying operation: create directory in " + delayinMillis + " ms");
                        //Thread.Sleep(delayinMillis);
                    }
                    else
                    {
                        throw e;
                    }
                }
            } while (retries > 0);
        }

        public static void CopyFileRetry(String from, String to, int retries = 6, int delayinMillis = 200)
        {
            do
            {
                try
                {
                    CopyFile(from, to);
                    return;
                }
                catch (Exception e)
                {
                    Log.Exception("CopyFileRetry", e);
                    if (retries > 0)
                    {
                        retries--;
                        Log.Info("retrying operation: copy file in " + delayinMillis + " ms");
                        //Thread.Sleep(delayinMillis);
                    }
                    else
                    {
                        throw e;
                    }
                }
            } while (retries > 0);
        }

        public static void DeleteFileRetry(String file, int retries = 6, int delayinMillis = 100)
        {
            do
            {
                try
                {
                    DeleteFile(file);
                    return;
                }
                catch (Exception e)
                {
                    Log.Exception("DeleteFileRetry", e);
                    if (retries > 0)
                    {
                        retries--;
                        Log.Info("retrying operation: delete file in " + delayinMillis + " ms");
                        //Thread.Sleep(delayinMillis);
                    }
                    else
                    {
                        throw e;
                    }
                }
            } while (retries > 0);
        }

        public static void DeleteDirectoryRetry(String directory, int retries = 6, int delayinMillis = 100)
        {
            do
            {
                try
                {
                    DeleteDirectory(directory);
                    return;
                }
                catch (Exception e)
                {
                    Log.Exception("DeleteDirectoryRetry", e);
                    if (retries > 0)
                    {
                        retries--;
                        Log.Info("retrying operation: delete directory in " + delayinMillis + " ms");
                        //Thread.Sleep(delayinMillis);
                    }
                    else
                    {
                        throw e;
                    }
                }
            } while (retries > 0);
        }


        public static String[] GetDirectories(String path)
        {
            return Directory.GetDirectories(path);
        }

        public static String[] GetFiles(String path)
        {
            return Directory.GetFiles(path);
        }

        public static bool FileExists(String file)
        {
            return File.Exists(file);
        }

        public static bool DirectoryExists(String file)
        {
            return Directory.Exists(file);
        }

        public static String GetFileName(String path)
        {
            return Path.GetFileName(path);
        }

        public static String ExpandBackupPath(String path)
        {
            if (path == null) return KSPUtil.ApplicationRootPath;
            path = path.Trim();
            if (path.StartsWith("./") || path.StartsWith(".\\"))
            {
                path = KSPUtil.ApplicationRootPath + path.Substring(2);
            }
            return path;
        }

#if (_DEBUG)
         /**
          * Used for debugging purposes only
          */
         public static void AppendText(String filename, String text)
         {
            using (StreamWriter sw = File.AppendText(filename))
            {
               sw.WriteLine(text);
               sw.Flush();
            }
         }
#endif





        internal static String _AssemblyLocation
        { get { return System.Reflection.Assembly.GetExecutingAssembly().Location; } }


        internal static String _AssemblyFolder
        { get { return System.IO.Path.GetDirectoryName(_AssemblyLocation); } }

        private static String _FilePath;
        /// <summary>
        /// Location of file for saving and loading methods
        ///
        /// This can be an absolute path (eg c:\test.cfg) or a relative path from the location of the assembly dll (eg. ../config/test)
        /// </summary>
        public static String FilePath
        {
            get { return _FilePath; }
            set
            {
                //Combine the Location of the assembly and the provided string. This means we can use relative or absolute paths
                _FilePath = System.IO.Path.Combine(_AssemblyFolder + "/../PluginData/", value).Replace("\\", "/");
            }
        }
        const string NODENAME = "AQSS";

        public static void SaveConfiguration(Configuration configuration, String file)
        {
            FilePath = file;

            ConfigNode f = new ConfigNode();
            ConfigNode node = new ConfigNode(NODENAME);
            node.AddValue("logLevel", ((int)configuration.logLevel).ToString());

            node.AddValue("quicksaveOnLaunch", configuration.quicksaveOnLaunch);
            node.AddValue("quicksaveOnSceneChange", configuration.quicksaveOnSceneChange);


            node.AddValue("quickSaveLaunchNameTemplate", configuration.quickSaveLaunchNameTemplate);

            node.AddValue("quicksaveInterval", (int)configuration.quicksaveInterval);
            node.AddValue("quickSaveNameTemplate", configuration.quickSaveNameTemplate);
            node.AddValue("customQuicksaveInterval", configuration.customQuicksaveInterval);

            
            node.AddValue("minTimeBetweenQuicksaves", configuration.minTimeBetweenQuicksaves);

            node.AddValue("daysToKeepQuicksaves", configuration.daysToKeepQuicksaves);
            node.AddValue("minNumberOfQuicksaves", configuration.minNumberOfQuicksaves);
            node.AddValue("maxNumberOfQuicksaves", configuration.maxNumberOfQuicksaves);


            node.AddValue("soundOnSave", configuration.soundOnSave);
            node.AddValue("soundLocation", configuration.soundLocation);
            node.AddValue("minimumTimeBetweenSounds", configuration.minimumTimeBetweenSounds);

            f.AddNode(node);
            f.Save(FilePath);
        }

        public static void LoadConfiguration(Configuration configuration, String file)
        {
            FilePath = file;
            if (File.Exists(FilePath))
            {
                ConfigNode f = ConfigNode.Load(FilePath);
                ConfigNode node = f.GetNode(NODENAME);
                if (node != null)
                {
                    configuration.Init();
                    configuration.logLevel = (Log.LEVEL)int.Parse(SafeLoad(node, "logLevel", (int)configuration.logLevel));
                    configuration.quicksaveOnLaunch = bool.Parse(SafeLoad(node, "quicksaveOnLaunch", configuration.quicksaveOnLaunch));
                    configuration.quicksaveOnSceneChange = bool.Parse(SafeLoad(node, "quicksaveOnSceneChange", configuration.quicksaveOnSceneChange));

                    
                    configuration.quickSaveLaunchNameTemplate = SafeLoad(node, "quickSaveLaunchNameTemplate", configuration.quickSaveLaunchNameTemplate);

                    configuration.quicksaveInterval = (Configuration.QuickSave_Interval)int.Parse(SafeLoad(node, "quicksaveInterval", (int)configuration.quicksaveInterval));

                    configuration.quickSaveNameTemplate = SafeLoad(node, "quickSaveNameTemplate", configuration.quickSaveNameTemplate);
                    configuration.customQuicksaveInterval = int.Parse(SafeLoad(node, "customQuicksaveInterval ", configuration.customQuicksaveInterval));

                    
                    configuration.minTimeBetweenQuicksaves = int.Parse(SafeLoad(node, "minTimeBetweenQuicksaves", configuration.minTimeBetweenQuicksaves));

                    configuration.daysToKeepQuicksaves = int.Parse(SafeLoad(node, "daysToKeepQuicksaves", configuration.daysToKeepQuicksaves));
                    configuration.minNumberOfQuicksaves = int.Parse(SafeLoad(node, "minNumberOfQuicksaves", configuration.minNumberOfQuicksaves));
                    configuration.maxNumberOfQuicksaves = int.Parse(SafeLoad(node, "maxNumberOfQuicksaves", configuration.maxNumberOfQuicksaves));

                    configuration.soundOnSave = bool.Parse(SafeLoad(node, "soundOnSave", configuration.soundOnSave));
                    configuration.soundLocation = SafeLoad(node, "soundLocation", configuration.soundLocation);
                    configuration.minimumTimeBetweenSounds = int.Parse(SafeLoad(node, "minimumTimeBetweenSounds ", configuration.minimumTimeBetweenSounds));
                }
                else
                {
                    Log.Info("no config file: default configuration");
                }
            }
        }

        static string SafeLoad(ConfigNode node, string name, string oldvalue)
        {
            string value = node.GetValue(name);
            if (value == null)
                return oldvalue;
            return value;
        }

        static string SafeLoad(ConfigNode node, string name, int oldvalue)
        {
            string value = node.GetValue(name);
            if (value == null)
                return oldvalue.ToString();
            return value;
        }
        static string SafeLoad(ConfigNode node, string name, bool oldvalue)
        {
            string value = node.GetValue(name);
            if (value == null)
                return oldvalue.ToString();
            return value;
        }

    }

}
