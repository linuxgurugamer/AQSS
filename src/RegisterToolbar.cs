using UnityEngine;
using ToolbarControl_NS;
using KSP_Log;

using static AutoQuickSaveSystem.AutoQuickSaveSystem;

namespace AutoQuickSaveSystem
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        protected void Start()
        {
            ToolbarControl.RegisterMod(MainMenuGui.MODID, MainMenuGui.MODNAME);
        }
    }

    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class InitLog : MonoBehaviour
    {
        protected void Awake()
        {
            AutoQuickSaveSystem.Log = new KSP_Log.Log("AQSS"
#if DEBUG
                , KSP_Log.Log.LEVEL.INFO
#endif
                );
        }
    }

}
