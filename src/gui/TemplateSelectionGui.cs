using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP;
using UnityEngine;
using System.IO;
using System.Reflection;
using KSP.UI.Screens;

using ClickThroughFix;
using ToolbarControl_NS;
using static AutoQuickSaveSystem.AutoQuickSaveSystem;
using System.Runtime.CompilerServices;

namespace AutoQuickSaveSystem
{
    internal class TemplateSelectionGui : MonoBehaviour
    {
        internal static TemplateSelectionGui templateGui = null;

        internal const string MODNAME = InstallChecker.FOLDERNAME;
        internal const string DEF_NODENAME = "AQSS";
        internal const string DEF_DEFAULT_NODENAME = "Templates";
        internal const string VALUENAME = "template";

        internal static GUIStyle lStyle = null;
        internal static GUIStyle labelStyle = null;

        Vector2 templateSelScrollVector = new Vector2(0, 0);
        List<string> templateList;

        internal static string dataDir;
        internal static string modDir;
        internal static string dataFile;

        static string lastSelectedTemplate = "";

        static string templateType = "";
        internal static string selectedTemplate = "";

        static Rect templateWindowPosition = new Rect(0, 0, 400, 200);


        internal static void StartWindow(string templtType, string selTemplate, Rect winBounds, Rect buttonBounds)
        {
            templateType = templtType;
            selectedTemplate = selTemplate;
            lastSelectedTemplate = selTemplate;

            templateWindowPosition.x = winBounds.x + winBounds.width;
            templateWindowPosition.y = winBounds.y + buttonBounds.y;

        }

        void Start()
        {
            modDir = KSPUtil.ApplicationRootPath + "GameData/";
            dataDir = modDir + MODNAME + "/PluginData/";
            dataFile = dataDir + "Templates.cfg";

            LoadTemplateList();
        }

        void LoadTemplateList()
        {
            Log.Info("LoadTemplateList, datafile: " + dataFile);
            ConfigNode templates = ConfigNode.Load(dataFile);

            ConfigNode r = templates.GetNode(DEF_NODENAME);
            if (r.HasNode(DEF_DEFAULT_NODENAME))
            {
                ConfigNode resAlertnode = r.GetNode(DEF_DEFAULT_NODENAME);
                templateList = resAlertnode.GetValuesList(VALUENAME);
                Log.Info("templateList.Count: " + templateList.Count());
            }
            else
                templateList = new List<string>();
        }

        void OnGUI()
        {
            GUI.skin = HighLogic.Skin;
            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.alignment = TextAnchor.MiddleLeft;
                lStyle = new GUIStyle(labelStyle);
                lStyle.alignment = TextAnchor.MiddleLeft;
            }

            templateWindowPosition = ClickThruBlocker.GUILayoutWindow(12395038, templateWindowPosition, TemplateSelectionWindow, templateType + "Template Selection");
        }

        void TemplateSelectionWindow(int id)
        {
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();

            templateSelScrollVector = GUILayout.BeginScrollView(templateSelScrollVector);
            int cnt = 0;
            foreach (var template in templateList)
            {
                GUILayout.BeginHorizontal();

                bool b = (lastSelectedTemplate == template);
                bool newB = GUILayout.Toggle(b, "");
                if (newB)
                    lastSelectedTemplate = template;

                if (lastSelectedTemplate == template)
                {
                    lStyle.normal.textColor = Color.green;
                }
                else
                {
                    lStyle.normal.textColor = labelStyle.normal.textColor;
                }

                GUILayout.Label(template, lStyle);

                cnt++;
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("OK", GUILayout.Width(90)))
            {
                selectedTemplate = lastSelectedTemplate;
                Apply(templateType);
                templateGui = null;
                Destroy(this);
            }
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Cancel", GUILayout.Width(90)))
            {
                templateGui = null;
                Destroy(this);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        void Apply(string str)
        {
            switch (str)
            {
                case "Launch":
                    Configuration.LaunchNameTemplate = selectedTemplate;
                    break;
                case "Quicksave":
                    Configuration.QuickSaveNameTemplate = selectedTemplate;
                    break;
                case "Scenesave":
                    Configuration.SceneSaveNameTemplate = selectedTemplate;
                    break;
                default:
                    Log.Error("Apply, str: " + str);
                    break;
            }
        }
    }
}
