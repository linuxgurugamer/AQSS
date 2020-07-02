using System;
using System.Collections;
using UnityEngine;

namespace AutoQuickSaveSystem
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    internal class Quicksave : MonoBehaviour
    {
        internal static Quicksave instance;

        static public string dateFormat = "yyyy-MM-dd--HH-mm-ss";
        static internal float lastBackup = 0;
        internal const string LAUNCH_QS_PREFIX = "LaunchQSave_";
        internal const string AUTO_QS_PREFIX = "AutoQSave_";

        void Start()
        {
            instance = this;
            DontDestroyOnLoad(this);
            StartCoroutine("QuickSaveLoop");
        }
        internal void RestartLoop()
        {
            StopCoroutine("QuickSaveLoop");
            StartCoroutine("QuickSaveLoop");
        }

#if true
        static DateTime setTime = DateTime.Now;
        IEnumerator QuickSaveLoop()
        {
            float sleepTime = 1f;
            while (true)
            {
                TimeSpan elapsed = DateTime.Now - setTime;

                if (!HighLogic.LoadedSceneIsEditor)
                {
                    switch (AutoQuickSaveSystem.configuration.quicksaveInterval)
                    {
                        case Configuration.QuickSave_Interval.ONCE_IN_10_MINUTES:
                            sleepTime = 10;
                            if (elapsed.TotalMinutes >= 10)
                            {
                                QuicksaveGame();
                            }
                            break;
                        case Configuration.QuickSave_Interval.ONCE_IN_30_MINUTES:
                            sleepTime = 30;
                            if (elapsed.TotalMinutes >= 30)
                            {
                                QuicksaveGame();
                            }
                            break;
                        case Configuration.QuickSave_Interval.ONCE_PER_HOUR:
                            sleepTime = 60;
                            if (elapsed.TotalHours >= 1)
                            {
                                QuicksaveGame();
                            }
                            break;
                        case Configuration.QuickSave_Interval.ONCE_IN_2_HOURS:
                            sleepTime = 120;
                            if (elapsed.TotalHours >= 2)
                            {
                                QuicksaveGame();
                            }
                            break;
                        case Configuration.QuickSave_Interval.ONCE_IN_4_HOURS:
                            sleepTime = 240;
                            if (elapsed.TotalHours >= 4)
                            {
                                QuicksaveGame();
                            }
                            break;
                        case Configuration.QuickSave_Interval.CUSTOM:
                            sleepTime = AutoQuickSaveSystem.configuration.customQuicksaveInterval;
                            if (elapsed.Minutes >= AutoQuickSaveSystem.configuration.customQuicksaveInterval)
                            {
                                QuicksaveGame();
                            }
                            break;
                        default:
                            Log.Error("invalid quicksave interval ignored");

                            break;
                    }
                    yield return new WaitForSecondsRealtime((float)(sleepTime * 60f - elapsed.TotalSeconds));
                }
            }
        }
#endif
        internal static void QuicksaveGame()
        {
            DoQuicksave(AUTO_QS_PREFIX + AutoQuickSaveSystem.configuration.quickSaveNameTemplate, "AutoQuickSave to");
            setTime = DateTime.Now;
        }
        internal static void DoQuicksave(string template, string message)
        {
            String game = HighLogic.SaveFolder;

            string newName = StringTranslation.AddFormatInfo(template, "", dateFormat);
            if (newName != null && newName.Length > 0)
            {

                string str = GamePersistence.SaveGame(newName, HighLogic.SaveFolder, SaveMode.BACKUP);

                lastBackup = Time.realtimeSinceStartup;

                ScreenMessages.PostScreenMessage(message + ": " + newName);

            }

            QuicksaveCleanup.Cleanup();
        }
    }

}