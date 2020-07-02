using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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

        internal static FileEntry[] GetBackups()
        {
            SortedList<string, FileEntry> saveFiles = new SortedList<string, FileEntry>();
            var savePath = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder;

            string newName = StringTranslation.AddFormatInfo(AutoQuickSaveSystem.configuration.quickSaveNameTemplate, "", "");

            var files = Directory.GetFiles(savePath, Quicksave.AUTO_QS_PREFIX + "*");
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

            // constraint for cleanup
            int minNumberOfQuicksaves = AutoQuickSaveSystem.configuration.minNumberOfQuicksaves;
            int maxNumberOfQuicksaves = AutoQuickSaveSystem.configuration.maxNumberOfQuicksaves;
            int daysToKeepQuicksaves = AutoQuickSaveSystem.configuration.daysToKeepQuicksaves;

            // no cleanup (keep all quicksaves forever?)
            if (maxNumberOfQuicksaves == 0 && daysToKeepQuicksaves == 0)
                return;

            var backups = GetBackups();
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