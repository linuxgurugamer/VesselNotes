#if false
using UnityEngine;
using File = System.IO.File;

using KSP.UI.Screens;
using ToolbarControl_NS;

namespace VesselNotesNS
{

    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod(LogsToFile.MODID, LogsToFile.MODNAME);
        }
    }
}
#endif