using System;
using UnityEngine;
using KSP.IO;
namespace AutoQuickSaveSystem
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    public class AutoQuickSaveSystem : MonoBehaviour
    {
        public static Configuration configuration = new Configuration();


        private MainMenuGui gui;

        public AutoQuickSaveSystem()
        {
            Log.Info("new instance of A.Q.S.S.");
        }

        public void Awake()
        {
            Log.Info("awake");

            DontDestroyOnLoad(this);
        }

        public void Start()
        {
            Log.SetLevel(Log.LEVEL.INFO);
            Log.Info("start");
            configuration.Load();
            ConfigNodeIO.LoadData();
            Log.SetLevel(configuration.logLevel);


            if (this.gui == null)
            {
                this.gui = this.gameObject.AddComponent<MainMenuGui>();                
            }
        }




        internal void OnDestroy()
        {
            Log.Info("destroying A.Q.S.S.");
            ConfigNodeIO.excludes.Clear();
            ConfigNodeIO.excludes = null;
            configuration.Save();
        }

    }

}
