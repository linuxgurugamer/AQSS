using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using KSP.IO;


namespace AutoQuickSaveSystem.QuickSaves
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    class SaveOnSceneChanges : MonoBehaviour
    {
        float MIN_BACKUP_INTERVAL = 15f;
        internal float lastBackup = 0;

        void Start()
        {
            GameEvents.onGameSceneLoadRequested.Add(this.CallbackGameSceneLoadRequested);
            DontDestroyOnLoad(this);
        }

        void OnDestroy()
        {
            GameEvents.onGameSceneLoadRequested.Remove(this.CallbackGameSceneLoadRequested);
        }

        private void CallbackGameSceneLoadRequested(GameScenes scene)
        {

            if (AutoQuickSaveSystem.configuration.quicksaveOnSceneChange)
            {
                if (Time.realtimeSinceStartup - lastBackup > (Math.Max(AutoQuickSaveSystem.configuration.minTimeBetweenQuicksaves, MIN_BACKUP_INTERVAL)))
                {
                    Quicksave.DoQuicksave(Quicksave.LAUNCH_QS_PREFIX + AutoQuickSaveSystem.configuration.quickSaveLaunchNameTemplate, "AutoQuickSave to");
                    lastBackup = Time.realtimeSinceStartup;
                }
            }
        }
    }

}