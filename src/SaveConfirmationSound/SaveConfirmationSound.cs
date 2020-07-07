// This was originally written by github user @kujuman, who kindly released it into the public domain
using System;
using System.Diagnostics;
using UnityEngine;

using static AutoQuickSaveSystem.AutoQuickSaveSystem;

namespace AutoQuickSaveSystem
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class SaveConfirmationSound : MonoBehaviour
    {
        //public string settingsURL = "GameData/SaveConfirmationSound/settings.cfg";
        public bool DEBUG = false;
        public DateTime LastSaveTime;
        int MinimumTimeBetweenSounds;
        string SoundURL;
        Stopwatch updateStopwatch;

        internal static bool forceAudio = false;
        internal static bool firstCall = true;

        protected void Start()
        {
            Log.Info("SaveConfirmationSound.Start");
            DontDestroyOnLoad(this);

            Audio.InitializeAudio();

            GameEvents.onGameStateSaved.Add(OnSave);

            updateStopwatch = new Stopwatch();
            updateStopwatch.Start();

            MinimumTimeBetweenSounds = Configuration.MinimumTimeBetweenSounds;
            SoundURL = Configuration.SoundLocation;
        }

        private void OnSave(Game data)
        {
            if (!Configuration.SoundOnSave || HighLogic.CurrentGame == null || HighLogic.LoadedScene == GameScenes.MAINMENU || HighLogic.LoadedSceneIsEditor) return;
            Log.Info("SaveConfirmationSound.OnSave");
            if (firstCall)
            {
                firstCall = false;
                return;
            }
            if (!forceAudio)
            {
                Log.Info(updateStopwatch.ElapsedMilliseconds + "ms");

                if ((float)updateStopwatch.ElapsedMilliseconds <= MinimumTimeBetweenSounds)
                {
                    Log.Info("below timer threshold");
                    return;
                }

                // Check the datetime stamp of the persistent.sfs, if it is <1 min, don't quicksave
                var persistentName = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/persistent.sfs";

                DateTime lastWriteTime = System.IO.File.GetLastWriteTime(persistentName);
                TimeSpan elapsed = DateTime.Now - lastWriteTime;
                if (elapsed.TotalSeconds < Configuration.MinTimeBetweenQuicksaves)
                {
                    return;
                }
            }
            forceAudio = false;

            updateStopwatch.Reset();
            updateStopwatch.Start();

            Log.Info("And Quick Saving");

            if (!MapView.MapIsEnabled)
            {
                Log.Info("In flight");
            }
            else
            {
                Log.Info("In map");
                Audio.markerAudio.transform.SetParent(MapView.MapCamera.transform);
            }
            if (Audio.markerAudio.isPlaying)
                return;
            Log.Info("Playing Sound " + SoundURL);
            Audio.markerAudio.PlayOneShot(GameDatabase.Instance.GetAudioClip(SoundURL));
            updateStopwatch.Reset();
            updateStopwatch.Start();
        }
    }

    internal class Audio
    {
        private static Audio instance;
        private Audio() { }
        internal static Audio Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Audio();
                }
                return instance;
            }
        }

        internal static bool isLoaded = false;
        internal static GameObject audioplayer;
        internal static AudioSource markerAudio;
        static bool initted = false;
        internal static void InitializeAudio()
        {
            if (initted) return;
            initted = true;

            audioplayer = new GameObject();
            markerAudio = new AudioSource();
            try
            {
                markerAudio = audioplayer.AddComponent<AudioSource>();
                markerAudio.volume = GameSettings.UI_VOLUME;
                markerAudio.panStereo = 0;
                markerAudio.dopplerLevel = 0;
                markerAudio.bypassEffects = true;
                markerAudio.loop = true;
                markerAudio.rolloffMode = AudioRolloffMode.Linear;
                Audio.markerAudio.transform.SetParent(FlightCamera.fetch.mainCamera.transform);
            }
            catch (Exception)
            {
                throw;
            }
            isLoaded = true;
        }
    }

}