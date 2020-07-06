using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using KSP.IO;

using static AutoQuickSaveSystem.AutoQuickSaveSystem;

namespace AutoQuickSaveSystem.QuickSaves
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    class SaveOnSceneChanges : MonoBehaviour
    {
        float MIN_BACKUP_INTERVAL = 15f;
        internal float lastBackup = 0;

        protected void Start()
        {
            GameEvents.onGameSceneLoadRequested.Add(this.CallbackGameSceneLoadRequested);
            DontDestroyOnLoad(this);
        }

        protected void OnDestroy()
        {
            GameEvents.onGameSceneLoadRequested.Remove(this.CallbackGameSceneLoadRequested);
        }

        private void CallbackGameSceneLoadRequested(GameScenes scene)
        {

            if (Configuration.QuicksaveOnSceneChange)
            {
                if (Time.realtimeSinceStartup - lastBackup > (Math.Max(Configuration.MinTimeBetweenQuicksaves, MIN_BACKUP_INTERVAL)))
                {
                    Log.Info("CallbackGameSceneLoadRequested doing Quicksave");
                    SaveConfirmationSound.forceAudio = true;
                    SaveConfirmationSound.firstCall = false;
                    Quicksave.DoQuicksave(Quicksave.SCENE_QS_PREFIX + Configuration.QuickSaveLaunchNameTemplate, "AutoQuickSave to");
                    lastBackup = Time.realtimeSinceStartup;
                }
            }
        }
    }

}