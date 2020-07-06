using System;
using System.IO;
using UnityEngine;
using KSP.UI.Screens;

using ClickThroughFix;
using ToolbarControl_NS;

//using KSP_Log;
using static AutoQuickSaveSystem.AutoQuickSaveSystem;

namespace AutoQuickSaveSystem
{
    internal class MainMenuGui : MonoBehaviour
    {
        private const String TITLE = "A.Q.S.S. - Automatic QuickSave System";
        private const string TEMPLATETITLE = "Template Info";
        private const int WIDTH = 450;
        private const int CONFIG_TEXTFIELD_RIGHT_MARGIN = 165;

        private Rect windowBounds = new Rect(0, 0, WIDTH, 0);
        private Rect templateInfoBounds = new Rect(0, 0, WIDTH, 0);
        private Vector2 audioListscrollPosition = Vector2.zero;

        private GUIStyle STYLE_BACKUPSET_STATUS_NAME = null;
        private GUIStyle STYLE_BACKUPSET_CLONE_NAME = null;
        private GUIStyle STYLE_BACKUPSET_STATUS = null;
        private GUIStyle STYLE_RECOVER_BUTTON = null;
        private GUIStyle STYLE_NAME_TEXTFIELD = null;
        private GUIStyle STYLE_CONFIG_BACKUP_PATH_LABEL = null;
        private GUIStyle STYLE_CONFIG_BACKUP_PATH_FIELD = null;
        private GUIStyle STYLE_CONFIG_TEXTFIELD = null;

        private GUIStyle STYLE_DELETE_BUTTON = null;

        private int selectedAudio = -1;

        private volatile bool visible = false;

        internal const string MODID = "AutoQuickSaveSystem_NS";
        internal const string MODNAME = "Auto Quick Save System";

        ToolbarControl toolbarControl = null;

        enum ConfigPanel { Quicksave, QuickSaveIntervals, QuickSaveAging, Editor, Sound };
        ConfigPanel showPanel = ConfigPanel.Quicksave;
        bool stylesInitted = false;
        string[] files = null;
        bool showTemplateInfo = false;

        protected void Start()
        {
            windowBounds = new Rect((Screen.width - WIDTH) / 2f, 100, WIDTH, 100);
            templateInfoBounds = new Rect((Screen.width - WIDTH), 100, WIDTH, 100);
            if (toolbarControl == null)
            {
                toolbarControl = gameObject.AddComponent<ToolbarControl>();
                toolbarControl.AddToAllToolbars(Toggle, Toggle,
                    ApplicationLauncher.AppScenes.SPACECENTER,
                    MODID,
                    "AQSSButton",
                    "AutoQuickSaveSystem/PluginData/Icons/AQSS-38",
                    "AutoQuickSaveSystem/PluginData/Icons/AQSS-24",
                    MODNAME
                );
            }
        }

        void Toggle()
        {
            visible = !visible;
            files = null;
            Configuration.Load();
        }
        protected void OnGUI()
        {
            try
            {
                if (visible)
                {
                    windowBounds = ClickThruBlocker.GUILayoutWindow(GetInstanceID(), windowBounds, Window, TITLE, HighLogic.Skin.window);


                    if (showTemplateInfo)
                        templateInfoBounds = ClickThruBlocker.GUILayoutWindow(this.GetInstanceID() + 11, templateInfoBounds, ShowTemplateInfoWindow, TEMPLATETITLE, HighLogic.Skin.window);


                }
            }
            catch (Exception e)
            {
                Log.Error("OnGUI exception: " + e.Message);
            }
        }

        private void Window(int id)
        {
            try
            {
                GUILayout.BeginVertical();

                DisplayConfigure();

                GUILayout.EndVertical();

            }
            catch (Exception e)
            {
                Log.Error("Window exception: " + e.Message);
            }

            GUI.DragWindow();
        }

#if false

        private void DrawTitle(String text)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(text, HighLogic.Skin.label);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
#endif

        GUIStyle btnStyle = null;

        private void InitStyles()
        {
            if (stylesInitted)
                return;
            stylesInitted = true;
            // for some reasons, this styles cant be created in the constructor
            // but we do not want to create a new instance every frame...
            {
                STYLE_BACKUPSET_STATUS_NAME = new GUIStyle(GUI.skin.label);
                STYLE_BACKUPSET_STATUS_NAME.stretchWidth = false;
                STYLE_BACKUPSET_STATUS_NAME.fixedWidth = 229;
                STYLE_BACKUPSET_STATUS_NAME.wordWrap = false;
            }
            {
                STYLE_BACKUPSET_CLONE_NAME = new GUIStyle(GUI.skin.label);
                STYLE_BACKUPSET_CLONE_NAME.stretchWidth = false;
                STYLE_BACKUPSET_CLONE_NAME.fixedWidth = 290;
                STYLE_BACKUPSET_CLONE_NAME.wordWrap = false;
            }
            {
                STYLE_BACKUPSET_STATUS = new GUIStyle(GUI.skin.label);
                STYLE_BACKUPSET_STATUS.stretchWidth = false;
                STYLE_BACKUPSET_STATUS.margin = new RectOffset(15, 0, 4, 0);
                STYLE_BACKUPSET_STATUS.fixedWidth = 65;
            }
            {
                STYLE_RECOVER_BUTTON = new GUIStyle(GUI.skin.button);
                STYLE_RECOVER_BUTTON.stretchWidth = false;
                STYLE_RECOVER_BUTTON.fixedWidth = 65;
            }
            {
                STYLE_NAME_TEXTFIELD = new GUIStyle(GUI.skin.textField);
                STYLE_NAME_TEXTFIELD.stretchWidth = false;
                STYLE_NAME_TEXTFIELD.fixedWidth = 355;
                STYLE_NAME_TEXTFIELD.wordWrap = false;
            }
            {
                STYLE_CONFIG_BACKUP_PATH_LABEL = new GUIStyle(GUI.skin.label);
            }
            {
                STYLE_CONFIG_BACKUP_PATH_FIELD = new GUIStyle(GUI.skin.textField);
                STYLE_CONFIG_BACKUP_PATH_FIELD.stretchWidth = false;
                STYLE_CONFIG_BACKUP_PATH_FIELD.fixedWidth = 295;
            }
            {
                STYLE_CONFIG_TEXTFIELD = new GUIStyle(GUI.skin.textField);
                STYLE_CONFIG_TEXTFIELD.stretchWidth = false;
                STYLE_CONFIG_TEXTFIELD.fixedWidth = 60;
                //STYLE_CONFIG_TEXTFIELD.margin = new RectOffset(0,200,0,0);
            }
            {
                STYLE_DELETE_BUTTON = new GUIStyle(GUI.skin.button);
                Color orange = new Color(255, 150, 60);
                STYLE_DELETE_BUTTON.normal.textColor = orange;
                STYLE_DELETE_BUTTON.active.textColor = orange;
                STYLE_DELETE_BUTTON.hover.textColor = orange;
                STYLE_DELETE_BUTTON.focused.textColor = orange;
                STYLE_DELETE_BUTTON.onNormal.textColor = orange;
                STYLE_DELETE_BUTTON.onActive.textColor = orange;
                STYLE_DELETE_BUTTON.onHover.textColor = orange;
                STYLE_DELETE_BUTTON.onFocused.textColor = orange;
            }
            btnStyle = new GUIStyle(GUI.skin.button);

        }

#if false
        private int IndexOf(String s, String[] a)
        {
            try
            {
                for (int i = 0; i < a.Length; i++)
                {
                    if (a[i].Equals(s))
                    {
                        return i;
                    }
                }
            }
            catch
            {
                Log.Error("internal error in IndexOf " + s + ", a[" + a.Length + "]");
            }
            return 0;
        }
#endif
        private void DisplayConfigure()
        {
            if (!stylesInitted)
                InitStyles();

            GUILayout.BeginVertical();
#if DEBUG
            GUILayout.BeginHorizontal();
            GUILayout.Label("Log:");
            LogLevelButton(KSP_Log.Log.LEVEL.OFF, "OFF");
            LogLevelButton(KSP_Log.Log.LEVEL.ERROR, "ERROR");
            LogLevelButton(KSP_Log.Log.LEVEL.WARNING, "WARNING");
            LogLevelButton(KSP_Log.Log.LEVEL.INFO, "INFO");
            LogLevelButton(KSP_Log.Log.LEVEL.DETAIL, "DETAIL");
            LogLevelButton(KSP_Log.Log.LEVEL.TRACE, "TRACE");
            GUILayout.EndHorizontal();
#endif

            GUILayout.Space(20);
            GUILayout.BeginHorizontal();

            GUI.enabled = showPanel != ConfigPanel.Quicksave;
            if (GUILayout.Button("Options"))
            {
                showPanel = ConfigPanel.Quicksave;
                windowBounds.height = 0;
            }

            GUI.enabled = showPanel != ConfigPanel.QuickSaveIntervals;
            if (GUILayout.Button("Intervals"))
            {
                showPanel = ConfigPanel.QuickSaveIntervals;
                windowBounds.height = 0;
            }

            GUI.enabled = showPanel != ConfigPanel.QuickSaveAging;
            if (GUILayout.Button("Age Settings"))
            {
                showPanel = ConfigPanel.QuickSaveAging;
                windowBounds.height = 0;
            }

            GUI.enabled = showPanel != ConfigPanel.Editor;
            if (GUILayout.Button("Editor Options"))
            {
                showPanel = ConfigPanel.Editor;
                windowBounds.height = 0;
            }
            GUI.enabled = showPanel != ConfigPanel.Sound;
            if (GUILayout.Button("Sound Options"))
            {
                showPanel = ConfigPanel.Sound;
                windowBounds.height = 0;
            }

            GUILayout.EndHorizontal();
            GUI.enabled = true;
            switch (showPanel)
            {
                case ConfigPanel.Quicksave:
                    {
                        GUILayout.Space(15);

                        // QuickSave options
                        // quicksave on launch
                        QuickSaveOnLaunchToggle("Quicksave on launch");
                        QuickSaveOnSceneChange("Quicksave on scene change");

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Minimum time between quicksaves (for scene changes only, in seconds): ");
                        String sMinTimeBetweenQuicksaves = GUILayout.TextField(Configuration.MinTimeBetweenQuicksaves.ToString(), STYLE_CONFIG_TEXTFIELD);
                        GUILayout.Space(CONFIG_TEXTFIELD_RIGHT_MARGIN);
                        Configuration.MinTimeBetweenQuicksaves = ParseInt(sMinTimeBetweenQuicksaves);
                        GUILayout.EndHorizontal();


                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Templates:");
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Template Info"))
                            showTemplateInfo = !showTemplateInfo;
                        GUILayout.EndHorizontal();

                        GUILayout.Label("Launch template: ", STYLE_CONFIG_BACKUP_PATH_LABEL);
                        Configuration.QuickSaveLaunchNameTemplate = GUILayout.TextField(Configuration.QuickSaveLaunchNameTemplate);
                        GUILayout.BeginHorizontal();
                        string newName = StringTranslation.AddFormatInfo(Configuration.QuickSaveLaunchNameTemplate, "", "");
                        GUILayout.Label(" ==> ");
                        GUILayout.TextField(Quicksave.LAUNCH_QS_PREFIX + newName);
                        GUILayout.EndHorizontal();

                        GUILayout.Label("Quicksave template: ", STYLE_CONFIG_BACKUP_PATH_LABEL);
                        Configuration.QuickSaveNameTemplate = GUILayout.TextField(Configuration.QuickSaveNameTemplate);
                        GUILayout.BeginHorizontal();
                        newName = StringTranslation.AddFormatInfo(Configuration.QuickSaveNameTemplate, "", "");
                        GUILayout.Label(" ==> ");
                        GUILayout.TextField(Quicksave.AUTO_QS_PREFIX + newName);
                        GUILayout.EndHorizontal();
                        GUILayout.Space(20);
                        GUILayout.EndVertical();
                    }
                    break;

                case ConfigPanel.QuickSaveIntervals:
                    {
                        GUILayout.Label("Quicksave interval: ");

                        QuicksaveIntervalToggle(Configuration.QuickSave_Interval.ONCE_IN_10_MINUTES, "Once in 10 minutes");
                        QuicksaveIntervalToggle(Configuration.QuickSave_Interval.ONCE_IN_30_MINUTES, "Once in 30 minutes");
                        QuicksaveIntervalToggle(Configuration.QuickSave_Interval.ONCE_PER_HOUR, "Once per hour");
                        QuicksaveIntervalToggle(Configuration.QuickSave_Interval.ONCE_IN_2_HOURS, "Once in 2 hours");
                        QuicksaveIntervalToggle(Configuration.QuickSave_Interval.ONCE_IN_4_HOURS, "Once in 4 hours");

                        GUILayout.BeginHorizontal();
                        QuicksaveIntervalToggle(Configuration.QuickSave_Interval.CUSTOM, "Custom (minutes)");
                        GUILayout.FlexibleSpace();
                        String sCustomInterval = GUILayout.TextField(Configuration.CustomQuicksaveInterval.ToString(), STYLE_CONFIG_TEXTFIELD);
                        GUILayout.Space(CONFIG_TEXTFIELD_RIGHT_MARGIN);
                        Configuration.CustomQuicksaveInterval = ParseInt(sCustomInterval);
                        GUILayout.EndHorizontal();

                        GUILayout.EndVertical();
                        GUILayout.Space(20);
                    }
                    break;

                case ConfigPanel.QuickSaveAging:
                    { 
                        GUILayout.Space(15);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Days to keep quicksaves: ");
                        String sDaysToKeepQuicksaves = GUILayout.TextField(Configuration.DaysToKeepQuicksaves.ToString(), STYLE_CONFIG_TEXTFIELD);
                        GUILayout.Space(CONFIG_TEXTFIELD_RIGHT_MARGIN);
                        Configuration.DaysToKeepQuicksaves = ParseInt(sDaysToKeepQuicksaves);
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Min number of quicksaves: ");
                        String sMinNumberOfQuicksaves = GUILayout.TextField(Configuration.MinNumberOfQuicksaves.ToString(), STYLE_CONFIG_TEXTFIELD);
                        GUILayout.Space(CONFIG_TEXTFIELD_RIGHT_MARGIN);
                        Configuration.MinNumberOfQuicksaves = ParseInt(sMinNumberOfQuicksaves);
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Max number of quicksaves: ");
                        String sMaxNumberOfQuicksaves = GUILayout.TextField(Configuration.MaxNumberOfQuicksaves.ToString(), STYLE_CONFIG_TEXTFIELD);
                        GUILayout.Space(CONFIG_TEXTFIELD_RIGHT_MARGIN);
                        Configuration.MaxNumberOfQuicksaves = ParseInt(sMaxNumberOfQuicksaves);
                        GUILayout.EndHorizontal();

                        GUILayout.Space(20);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Max number of launch saves: ");
                        String sMaxNumberOfLaunchsaves = GUILayout.TextField(Configuration.MaxNumberOfLaunchsaves.ToString(), STYLE_CONFIG_TEXTFIELD);
                        GUILayout.Space(CONFIG_TEXTFIELD_RIGHT_MARGIN);
                        Configuration.MaxNumberOfLaunchsaves = ParseInt(sMaxNumberOfLaunchsaves);
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Max number of scene change saves: ");
                        String sMaxNumberOfScenesaves = GUILayout.TextField(Configuration.MaxNumberOfScenesaves.ToString(), STYLE_CONFIG_TEXTFIELD);
                        GUILayout.Space(CONFIG_TEXTFIELD_RIGHT_MARGIN);
                        Configuration.MaxNumberOfScenesaves = ParseInt(sMaxNumberOfScenesaves);
                        GUILayout.EndHorizontal();


                        GUILayout.EndVertical();
                        GUILayout.Space(20);
                    }
                    break;

                case ConfigPanel.Editor:
                    {
                        GUILayout.Space(15);
                        Configuration.saveVesselInEditor = GUILayout.Toggle(Configuration.saveVesselInEditor, "Auto-save vessels in the editor");
                        GUI.enabled = Configuration.saveVesselInEditor;
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Time interval between autosaves (in seconds): ");
                        String seditorTimeToSave = GUILayout.TextField(Configuration.editorTimeIntervalToSave.ToString(), STYLE_CONFIG_TEXTFIELD);
                        GUILayout.Space(CONFIG_TEXTFIELD_RIGHT_MARGIN);
                        Configuration.editorTimeIntervalToSave = ParseInt(seditorTimeToSave);
                        GUILayout.EndHorizontal();

                        GUI.enabled = true;
                        GUILayout.EndVertical();
                        GUILayout.Space(20);
                    }
                    break;

                case ConfigPanel.Sound:
                    GUILayout.Space(15);
                    Configuration.SoundOnSave = GUILayout.Toggle(Configuration.SoundOnSave, "Enable sound on quicksave");

                    if (Configuration.SoundOnSave)
                    {
                        GUILayout.Space(15);

                        if (files == null)
                            files = Directory.GetFiles(KSPUtil.ApplicationRootPath + "GameData/" + Configuration.AUDIO_DIR, "*");
                        audioListscrollPosition = GUILayout.BeginScrollView(audioListscrollPosition, GUI.skin.box, GUILayout.Height(160));
                        int cnt = 0;

                        foreach (var name in files)
                        {
                            var shortName = Path.GetFileNameWithoutExtension(name);
                            if (selectedAudio == -1 && Configuration.AUDIO_DIR + shortName == Configuration.SoundLocation)
                                selectedAudio = cnt;
                            GUILayout.BeginHorizontal();
                            btnStyle.normal.textColor = (cnt == selectedAudio) ? Color.green : GUI.skin.button.normal.textColor;
                            if (GUILayout.Button(shortName, btnStyle, GUILayout.Width(400)))
                            {
                                selectedAudio = cnt;
                                Configuration.SoundLocation = Configuration.AUDIO_DIR + shortName;
                            }
                            if (GUILayout.Button("►", btnStyle, GUILayout.Width(25)))
                            {
                                Audio.InitializeAudio();
                                Audio.markerAudio.PlayOneShot(GameDatabase.Instance.GetAudioClip(Configuration.AUDIO_DIR + shortName));
                            }
                            cnt++;
                            GUILayout.EndHorizontal();
                        }

                        GUILayout.EndScrollView();
                    }
                    GUILayout.EndVertical();
                    break;
            }
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("(Click <B>Save Config</b> button to save) ==>");
            if (GUILayout.Button("Save Config", GUI.skin.button))
            {
                Configuration.Save();
                Toggle();
                Quicksave.instance.RestartLoop();
            }
            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }


        const string data = "Templates have fixed prefixes depending on whether it is a Launch template or a Quicksave template\n\n" +
            "Tokens are surrounded with square brackets\nDate info is from the Kerbal date\n\n" +
            "year       Short version of year, no leading zeroes\n" +
            "year0      Long version of year, with zero padding to make it 3 digits long\n" +

            "day        Short version of day, no leading zeroes\n" +
            "day0       Long version of day, with zero padding to make it 3 digits long\n" +

            "hour       Short version of hour, no leading zeroes\n" +
            "hour0      Long version of hour, with zero padding to make it 2 digits long\n" +

            "min        Short version of min, no leading zeroes\n" +
            "min0       Long version of min, with zero padding to make it 2 digits long\n" +

            "sec        Short version of sec, no leading zeroes\n" +
            "sec0       Long version of sec, with zero padding to make it 2 digits long\n\n" +

            "The following will only be used if there is an active vessel, otherwise they\n" +
            "will simply be deleted\n\n" +

            "MET        For active vessel, mission time\n" +
            "UT         Universal time\n" +
            "vessel     Vessel name\n" +
            "body       Current main body\n" +
            "situation  Current situation\n" +
            "biome      Current biome";

        void ShowTemplateInfoWindow(int id)
        {
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            GUILayout.TextArea(data);
            GUILayout.Space(15);
            if (GUILayout.Button("Close"))
                showTemplateInfo = false;
            GUILayout.EndVertical();
            GUI.DragWindow();
        }
        private int ParseInt(String s)
        {
            try
            {
                return int.Parse(s);
            }
            catch (NotFiniteNumberException)
            {
                Log.Warning("invalid number format: " + s);
                return 0;
            }
        }

        private void QuickSaveOnLaunchToggle(String text)
        {
            Configuration.QuicksaveOnLaunch = GUILayout.Toggle(Configuration.QuicksaveOnLaunch, " " + text);
        }

        private void QuickSaveOnSceneChange(String text)
        {
            Configuration.QuicksaveOnSceneChange = GUILayout.Toggle(Configuration.QuicksaveOnSceneChange, " " + text);
        }

        private void QuicksaveIntervalToggle(Configuration.QuickSave_Interval interval, String text)
        {
            if (GUILayout.Toggle(Configuration.QuicksaveInterval == interval, " " + text))
            {
                Configuration.QuicksaveInterval = interval;
            }
        }

        private void LogLevelButton(KSP_Log.Log.LEVEL level, String text)
        {
            if (GUILayout.Toggle(Log.GetLevel() == level, text, GUI.skin.button) && Log.GetLevel() != level)
            {
                Configuration.LogLevel = level;
                Log.SetLevel(level);
            }
        }
    }
}
