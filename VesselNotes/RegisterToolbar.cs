#if true
using UnityEngine;
using File = System.IO.File;
using KSP_Log;
using KSP.UI.Screens;
using ToolbarControl_NS;

namespace VesselNotesNS
{

    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        internal static Log Log = null;
        bool initted = false;
        internal void InitLog()
        {
#if DEBUG
                Log = new Log("VesselNotes", Log.LEVEL.INFO);
#else
                Log = new Log("VesselNotes", Log.LEVEL.ERROR);
#endif
        }

        public static GUIStyle myStyle = null;
        internal  void InitStyle()
        {
            if (myStyle == null)
            {
                myStyle = new GUIStyle(GUI.skin.textArea);
                //myStyle.fontSize = FontSize;
                myStyle.richText = true;
            }
        }

        void Start()
        {
            ToolbarControl.RegisterMod(EnterExitGame.MODID, EnterExitGame.MODNAME);
            InitLog(); ;
        }
        void OnGUI()
        {
            if (!initted)
            {
                initted = true;
                InitStyle();
                EnterExitGame.InitStyle();
            }
        }
    }
}
#endif