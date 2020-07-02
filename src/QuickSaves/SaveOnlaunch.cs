using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AutoQuickSaveSystem
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class SaveOnlaunch : MonoBehaviour
    {
        float MIN_BACKUP_INTERVAL = 15f;

        void Start()
        {
            GameEvents.onLaunch.Add(onLaunch);
        }

        void OnDestroy()
        {
            GameEvents.onLaunch.Remove(onLaunch);
        }


        void onLaunch(EventReport er)
        {
            Log.Info("GameEvents.onLaunch");
            if (Time.realtimeSinceStartup - Quicksave.lastBackup > MIN_BACKUP_INTERVAL)
            {
                Quicksave.DoQuicksave(Quicksave.LAUNCH_QS_PREFIX + AutoQuickSaveSystem.configuration.quickSaveLaunchNameTemplate, "Launch Save to");
            }
        }
    }

}