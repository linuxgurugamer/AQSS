using System;
using System.IO;
using UnityEngine;
using KSP.UI.Screens;

using ClickThroughFix;
using ToolbarControl_NS;

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

        enum ConfigPanel { Quicksave, Sound };
        ConfigPanel showPanel = ConfigPanel.Quicksave;
        bool stylesInitted = false;
        string[] files = null;
        bool showTemplateInfo = false;

        void Start()
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
            Configuration config = AutoQuickSaveSystem.configuration;

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

        private void DrawTitle(String text)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(text, HighLogic.Skin.label);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }


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

        private void DisplayConfigure()
        {
            if (!stylesInitted)
                InitStyles();
            //
            Configuration config = AutoQuickSaveSystem.configuration;
            //
            GUILayout.BeginVertical();
            DrawTitle("Configuration");
#if DEBUG
            GUILayout.BeginHorizontal();
            GUILayout.Label("Log:");
            LogLevelButton(Log.LEVEL.OFF, "OFF");
            LogLevelButton(Log.LEVEL.ERROR, "ERROR");
            LogLevelButton(Log.LEVEL.WARNING, "WARNING");
            LogLevelButton(Log.LEVEL.INFO, "INFO");
            LogLevelButton(Log.LEVEL.DETAIL, "DETAIL");
            LogLevelButton(Log.LEVEL.TRACE, "TRACE");
            GUILayout.EndHorizontal();
#endif

            GUILayout.Space(20);
            GUILayout.BeginHorizontal();

            GUI.enabled = showPanel != ConfigPanel.Quicksave;
            if (GUILayout.Button("Quicksave Options"))
            {
                showPanel = ConfigPanel.Quicksave;
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
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Templates:");
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Template Info"))
                            showTemplateInfo = !showTemplateInfo;
                        GUILayout.EndHorizontal();

                        GUILayout.Label("Launch template: ", STYLE_CONFIG_BACKUP_PATH_LABEL);
                        config.quickSaveLaunchNameTemplate = GUILayout.TextField(config.quickSaveLaunchNameTemplate);
                        GUILayout.BeginHorizontal();
                        string newName = StringTranslation.AddFormatInfo(AutoQuickSaveSystem.configuration.quickSaveLaunchNameTemplate, "", "");
                        GUILayout.Label(" ==> ");
                        GUILayout.TextField(Quicksave.LAUNCH_QS_PREFIX + newName);
                        GUILayout.EndHorizontal();

                        GUILayout.Label("Quicksave template: ", STYLE_CONFIG_BACKUP_PATH_LABEL);
                        config.quickSaveNameTemplate = GUILayout.TextField(config.quickSaveNameTemplate);
                        GUILayout.BeginHorizontal();
                        newName = StringTranslation.AddFormatInfo(AutoQuickSaveSystem.configuration.quickSaveNameTemplate, "", "");
                        GUILayout.Label(" ==> ");
                        GUILayout.TextField(Quicksave.AUTO_QS_PREFIX + newName);
                        GUILayout.EndHorizontal();


                        GUILayout.Label("Quicksave interval: ");

                        QuicksaveIntervalToggle(Configuration.QuickSave_Interval.ONCE_IN_10_MINUTES, "Once in 10 minutes");
                        QuicksaveIntervalToggle(Configuration.QuickSave_Interval.ONCE_IN_30_MINUTES, "Once in 30 minutes");
                        QuicksaveIntervalToggle(Configuration.QuickSave_Interval.ONCE_PER_HOUR, "Once per hour");
                        QuicksaveIntervalToggle(Configuration.QuickSave_Interval.ONCE_IN_2_HOURS, "Once in 2 hours");
                        QuicksaveIntervalToggle(Configuration.QuickSave_Interval.ONCE_IN_4_HOURS, "Once in 4 hours");

                        GUILayout.BeginHorizontal();
                        QuicksaveIntervalToggle(Configuration.QuickSave_Interval.CUSTOM, "Custom (minutes)");
                        GUILayout.FlexibleSpace();
                        String sCustomInterval = GUILayout.TextField(config.customQuicksaveInterval.ToString(), STYLE_CONFIG_TEXTFIELD);
                        GUILayout.Space(CONFIG_TEXTFIELD_RIGHT_MARGIN);
                        config.customQuicksaveInterval = ParseInt(sCustomInterval);
                        GUILayout.EndHorizontal();

                        GUILayout.EndVertical();
                        GUILayout.Space(15);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Days to keep quicksaves: ");
                        String sDaysToKeepQuicksaves = GUILayout.TextField(config.daysToKeepQuicksaves.ToString(), STYLE_CONFIG_TEXTFIELD);
                        GUILayout.Space(CONFIG_TEXTFIELD_RIGHT_MARGIN);
                        config.daysToKeepQuicksaves = ParseInt(sDaysToKeepQuicksaves);
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Min number of quicksaves: ");
                        String sMinNumberOfQuicksaves = GUILayout.TextField(config.minNumberOfQuicksaves.ToString(), STYLE_CONFIG_TEXTFIELD);
                        GUILayout.Space(CONFIG_TEXTFIELD_RIGHT_MARGIN);
                        config.minNumberOfQuicksaves = ParseInt(sMinNumberOfQuicksaves);
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Max number of quicksaves: ");
                        String sMaxNumberOfQuicksaves = GUILayout.TextField(config.maxNumberOfQuicksaves.ToString(), STYLE_CONFIG_TEXTFIELD);
                        GUILayout.Space(CONFIG_TEXTFIELD_RIGHT_MARGIN);
                        config.maxNumberOfQuicksaves = ParseInt(sMaxNumberOfQuicksaves);
                        GUILayout.EndHorizontal();

                    }
                    break;

                case ConfigPanel.Sound:
                    GUILayout.Space(15);
                    config.soundOnSave = GUILayout.Toggle(config.soundOnSave, "Enable sound on quicksave");

                    if (config.soundOnSave)
                    {
                        GUILayout.Space(15);

                        if (files == null)
                            files = Directory.GetFiles(KSPUtil.ApplicationRootPath + "GameData/" + Configuration.AUDIO_DIR, "*");
                        audioListscrollPosition = GUILayout.BeginScrollView(audioListscrollPosition, GUI.skin.box, GUILayout.Height(Screen.height / 2));
                        int cnt = 0;

                        foreach (var name in files)
                        {
                            var shortName = Path.GetFileNameWithoutExtension(name);
                            if (selectedAudio == -1 && Configuration.AUDIO_DIR + shortName == config.soundLocation)
                                selectedAudio = cnt;
                            GUILayout.BeginHorizontal();
                            btnStyle.normal.textColor = (cnt == selectedAudio) ? Color.green : GUI.skin.button.normal.textColor;
                            if (GUILayout.Button(shortName, btnStyle, GUILayout.Width(400)))
                            {
                                selectedAudio = cnt;
                                config.soundLocation = Configuration.AUDIO_DIR + shortName;
                            }
                            if (GUILayout.Button("►", btnStyle, GUILayout.Width(25)))
                            {
                                Audio.initializeAudio();
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
            if (GUILayout.Button("Save Config", GUI.skin.button))
            {
                config.Save();
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
            AutoQuickSaveSystem.configuration.quicksaveOnLaunch = GUILayout.Toggle(AutoQuickSaveSystem.configuration.quicksaveOnLaunch, " " + text);
        }

        private void QuicksaveIntervalToggle(Configuration.QuickSave_Interval interval, String text)
        {
            if (GUILayout.Toggle(AutoQuickSaveSystem.configuration.quicksaveInterval == interval, " " + text))
            {
                AutoQuickSaveSystem.configuration.quicksaveInterval = interval;
            }
        }

        private void LogLevelButton(Log.LEVEL level, String text)
        {
            if (GUILayout.Toggle(Log.GetLevel() == level, text, GUI.skin.button) && Log.GetLevel() != level)
            {
                AutoQuickSaveSystem.configuration.logLevel = level;
                Log.SetLevel(level);
            }
        }
    }
}
