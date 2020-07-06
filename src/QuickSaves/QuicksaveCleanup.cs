using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static AutoQuickSaveSystem.AutoQuickSaveSystem;

namespace AutoQuickSaveSystem
{
    internal static class QuicksaveCleanup
    {
        internal class FileEntry
        {
            internal string name;
            internal DateTime lastAccessTime;

            internal FileEntry(string name, DateTime lastAccessTime)
            {
                this.name = name;
                this.lastAccessTime = lastAccessTime;
            }
        }
        static void DeleteQuicksave(String fname)
        {
            Log.Info("DeleteQuicksave: " + fname);
            System.IO.File.Delete(fname);
        }

        internal static FileEntry[] GetBackups(string prefix)
        {
            SortedList<string, FileEntry> saveFiles = new SortedList<string, FileEntry>();
            var savePath = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder;

            var files = Directory.GetFiles(savePath, prefix + "*");
            foreach (var s in files)
            {
                DateTime dt = System.IO.File.GetLastWriteTime(s);
                saveFiles.Add(s, new FileEntry(s, dt));
                Log.Info("QuicksaveCleanup, s: " + s);
            }
            List<FileEntry> feList = new List<FileEntry>();

            foreach (var i in saveFiles)
                feList.Add(i.Value);
            return feList.ToArray();
        }

        internal static void Cleanup()
        {
            Log.Info("cleaning up Quicksaves");

            CleanupRegularQuicksaves();
            CleanupOtherSaves(Quicksave.LAUNCH_QS_PREFIX, Configuration.MaxNumberOfLaunchsaves);
            CleanupOtherSaves(Quicksave.SCENE_QS_PREFIX, Configuration.MaxNumberOfScenesaves);
        }

        static void CleanupOtherSaves(string prefix, int maxNumber)
        {
            var backups = GetBackups(prefix);

            // total number of backups before cleanup
            int totalBackupCount = backups.Length;
            int backupsToClean = totalBackupCount - 2 * maxNumber;


            // backupsToClean is now set, so that minNumberOfBackups are kept
            for (int i = 0; i < backupsToClean; i++)
            {
                Log.Info("Deleting backup file: " + backups[i].name);
                DeleteQuicksave(backups[i].name);
            }
        }

        static void CleanupRegularQuicksaves()
        {
            // constraint for cleanup
            int minNumberOfQuicksaves = Configuration.MinNumberOfQuicksaves;
            int maxNumberOfQuicksaves = Configuration.MaxNumberOfQuicksaves;
            int daysToKeepQuicksaves = Configuration.DaysToKeepQuicksaves;

            // no cleanup (keep all quicksaves forever?)
            if (maxNumberOfQuicksaves == 0 && daysToKeepQuicksaves == 0)
                return;

            var backups = GetBackups(Quicksave.AUTO_QS_PREFIX);
            // the point in time until backups have to be kept
            DateTime timeOfObsoleteBackups = DateTime.Now.AddDays(-daysToKeepQuicksaves);

            // total number of backups before cleanup
            int totalBackupCount = backups.Length;

            // make sure minNumberOfBackups successful backups are kept
            int backupsToClean = totalBackupCount - minNumberOfQuicksaves * 2; // *2 because there are two files for each quicksave


            // backupsToClean is now set, so that minNumberOfBackups are kept
            for (int i = 0; i < backupsToClean; i++)
            {
                String backupName = backups[i].name;
                DateTime t = backups[i].lastAccessTime;
                // backup has to be kept, because of time constraints, if not then backup may be obsolete
                bool backupObsoleteByTime = (t < timeOfObsoleteBackups) && (daysToKeepQuicksaves > 0);
                // backup has to be kept, because of number constraints, if not then backup may be obsolete
                bool backupObsoleteByNumber = (totalBackupCount - i > maxNumberOfQuicksaves) && (maxNumberOfQuicksaves > 0);
                // backups are obsolete, if they are obsolete by number constratins AND time constraints
                if (backupObsoleteByTime || backupObsoleteByNumber)
                {
                    // delete backup, if obsolete
                    DeleteQuicksave(backupName);
                }
            }
            // refresh backup array (for GUI display)
        }
    }

}