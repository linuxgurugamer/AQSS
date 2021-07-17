using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP_Log;

namespace AutoQuickSaveSystem
{
    public static class Configuration
    {
        private static readonly String SETTINGS_FILE_NAME = "AutoQuickSaveSystem.cfg";
        internal static readonly String AUDIO_DIR = "AutoQuickSaveSystem/Audio/";
        public static Log.LEVEL LogLevel { get; set; }

        // backup interval
        public enum QuickSave_Interval {  ONCE_IN_10_MINUTES = 1, ONCE_IN_30_MINUTES = 2, ONCE_PER_HOUR = 3, ONCE_IN_2_HOURS = 4, ONCE_IN_4_HOURS = 5, CUSTOM = 6 }

        // Quicksave options
        public static QuickSave_Interval QuicksaveInterval { get; set; } // ON_QUIT not valid
        public static bool QuicksaveOnLaunch { get; set; }
        public static bool QuicksaveOnSceneChange { get; set; }

        public static int MinTimeBetweenQuicksaves { get; set; }

        public static String LaunchNameTemplate { get; set; }
        public static String QuickSaveNameTemplate { get; set; }
        public static String SceneSaveNameTemplate { get; set; }
        public static int CustomQuicksaveInterval { get; set; }

        public static int DaysToKeepQuicksaves { get; set; }
        public static int MinNumberOfQuicksaves { get; set; }
        public static int MaxNumberOfQuicksaves { get; set; }
        public static int MaxNumberOfLaunchsaves { get; set; }
        public static int MaxNumberOfScenesaves { get; set; }


        public static bool saveVesselInEditor { get; set; }
        public static int editorTimeIntervalToSave { get; set; }

        // Save Confirmation Sounds option
        public static bool SoundOnSave { get; set; } = false;
        public static String SoundLocation { get; set; } = AUDIO_DIR + "click";
        public static int MinimumTimeBetweenSounds { get; set; } = 100; // Milliseconds

        const string DEFAULT_TEMPLATE = "[year0]_[day0]-[hour0]:[min0]";
        const string DEFAULT_LAUNCH_TEMPLATE = DEFAULT_TEMPLATE + ":[sec0]";

        public static void StartUp()
        {
            Init();
            Load();
        }
        public static void Init()
        {
            LogLevel = Log.LEVEL.INFO;

            QuicksaveInterval = QuickSave_Interval.ONCE_IN_10_MINUTES;
            QuicksaveOnLaunch = true;
            QuicksaveOnSceneChange = false;
            LaunchNameTemplate = DEFAULT_LAUNCH_TEMPLATE;
            QuickSaveNameTemplate = DEFAULT_TEMPLATE;
            SceneSaveNameTemplate = DEFAULT_TEMPLATE;
            CustomQuicksaveInterval = 15;
            MinTimeBetweenQuicksaves = 600;

            DaysToKeepQuicksaves = 14;
            MinNumberOfQuicksaves = 10;
            MaxNumberOfQuicksaves = 50;
            MaxNumberOfLaunchsaves = 10;
            MaxNumberOfScenesaves = 10;
            saveVesselInEditor = true;
            editorTimeIntervalToSave = 30;
            SoundOnSave = true;

        }
        public static void Save()
        {
            FileOperations.SaveConfiguration( SETTINGS_FILE_NAME);
        }

        public static void Load()
        {
            FileOperations.LoadConfiguration( SETTINGS_FILE_NAME);
        }
    }

}
