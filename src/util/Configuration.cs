using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoQuickSaveSystem
{
    public class Configuration
    {
        private static readonly String SETTINGS_FILE_NAME = "AutoQuickSaveSystem.cfg";
        internal static readonly String AUDIO_DIR = "AutoQuickSaveSystem/Audio/";
        public Log.LEVEL logLevel { get; set; }

        // backup interval
        public enum QuickSave_Interval { EACH_SAVE = 0, ONCE_IN_10_MINUTES = 1, ONCE_IN_30_MINUTES = 2, ONCE_PER_HOUR = 3, ONCE_PER_DAY = 4, ONCE_PER_WEEK = 5, ONCE_IN_2_HOURS = 6, ONCE_IN_4_HOURS = 7, CUSTOM = 8, ON_QUIT = 9 }

        // Quicksave options
        public QuickSave_Interval quicksaveInterval { get; set; } // ON_QUIT not valid
        public bool quicksaveOnLaunch { get; set; }
        public bool quicksaveOnSceneChange { get; set; }

        public int minTimeBetweenQuicksaves { get; set; }

        public String quickSaveLaunchNameTemplate { get; set; }
        public String quickSaveNameTemplate { get; set; }
        public int customQuicksaveInterval { get; set; } = 15;

        public int daysToKeepQuicksaves { get; set; }
        public int minNumberOfQuicksaves { get; set; }
        public int maxNumberOfQuicksaves { get; set; }

        // Save Confirmation Sounds option
        public bool soundOnSave { get; set; } = false;
        public String soundLocation { get; set; } = AUDIO_DIR + "click";
        public int minimumTimeBetweenSounds { get; set; } = 100; // Milliseconds

        public Configuration()
        {
            Init();
        }

        public void Init()
        {
            logLevel = Log.LEVEL.INFO;

            quicksaveInterval = QuickSave_Interval.ONCE_IN_10_MINUTES;
            quicksaveOnLaunch = true;
            quicksaveOnSceneChange = false;
            quickSaveLaunchNameTemplate = "Y[year0]D[day0]H[hour0]M[min0]S[sec0]";
            quickSaveNameTemplate = "Y[year0]D[day0]H[hour0]M[min0]";
            customQuicksaveInterval = 15;
            minTimeBetweenQuicksaves = 600;

            daysToKeepQuicksaves = 14;
            minNumberOfQuicksaves = 10;
            maxNumberOfQuicksaves = 50;

            soundOnSave = true;

        }
        public void Save()
        {
            FileOperations.SaveConfiguration(this, SETTINGS_FILE_NAME);
        }

        public void Load()
        {
            FileOperations.LoadConfiguration(this, SETTINGS_FILE_NAME);
        }
    }

}
