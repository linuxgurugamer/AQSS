using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace AutoQuickSaveSystem
{
    class ConfigNodeIO
    {

        static string PLUGINDATA = "GameData/AutoQuickSaveSystem/PluginData/Settings.cfg";
        const string DATANODE = "A.Q.S.S.";
        const string EXCLUDE = "Exclude";
#if false
        internal static bool fixedWindowUpperRight = true;
        internal static bool fixedWindowUpperLeft = false;
        internal static bool fixedWindowFloating = false;
#endif
        internal static List<string> excludes = null;
        internal static string SafeLoad(string value, bool oldvalue)
        {
            if (value == null)
                return oldvalue.ToString();
            return value;
        }

        static public void LoadData()
        {
            if (File.Exists(KSPUtil.ApplicationRootPath + PLUGINDATA))
            {
                ConfigNode data = ConfigNode.Load(KSPUtil.ApplicationRootPath + PLUGINDATA);
                if (data != null)
                {
                    ConfigNode dataNode = data.GetNode(DATANODE);
                    if (dataNode != null)
                    {
#if false
                        string fixedWindowPos = dataNode.GetValue("WindowPos");
                        if (fixedWindowPos != null)
                        {

                            fixedWindowUpperRight = (fixedWindowPos == "upperRight");
                            fixedWindowUpperLeft = (fixedWindowPos == "upperLeft");
                            fixedWindowFloating = (fixedWindowPos == "floating");

                            // If none are specified, then default to the original location

                            if (!fixedWindowUpperRight && !fixedWindowUpperLeft && !fixedWindowFloating)
                                fixedWindowUpperRight = true;
                        }
#endif
                        excludes = dataNode.GetValuesList(EXCLUDE);
                    }
                }
            }
            else
                excludes = new List<string>();
        }
    }
}
