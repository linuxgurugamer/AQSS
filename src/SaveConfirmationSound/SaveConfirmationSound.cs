// This was originally written by github user @kujuman, who kindly released it into the public domain
using System;
using System.Diagnostics;
using UnityEngine;

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

        void Start()
        {
            Log.Info("SaveConfirmationSound.Start");
            DontDestroyOnLoad(this);

            Audio.initializeAudio();

            GameEvents.onGameStateSaved.Add(OnSave);

            updateStopwatch = new Stopwatch();
            updateStopwatch.Start();

            MinimumTimeBetweenSounds = AutoQuickSaveSystem.configuration.minimumTimeBetweenSounds;
            SoundURL = AutoQuickSaveSystem.configuration.soundLocation;
        }

        private void OnSave(Game data)
        {
            if (!AutoQuickSaveSystem.configuration.soundOnSave || HighLogic.CurrentGame == null || HighLogic.LoadedScene == GameScenes.MAINMENU) return;

            Log.Info(updateStopwatch.ElapsedMilliseconds + "ms");

            if ((float)updateStopwatch.ElapsedMilliseconds <= MinimumTimeBetweenSounds)
            {
                Log.Info("below timer threshold");
                return;
            }
            updateStopwatch.Reset();
            updateStopwatch.Start();

            {
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
                Log.Info("Playing Sound " + SoundURL);
                Audio.markerAudio.PlayOneShot(GameDatabase.Instance.GetAudioClip(SoundURL));
                updateStopwatch.Reset();
                updateStopwatch.Start();
            }
        }
    }

    public class Audio
    {
        private static Audio instance;
        private Audio() { }
        public static Audio Instance
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

        public static bool isLoaded = false;
        public static GameObject audioplayer;
        public static AudioSource markerAudio;
        static bool initted = false;
        public static void initializeAudio()
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