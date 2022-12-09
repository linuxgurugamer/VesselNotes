using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static VesselNotesNS.RegisterToolbar;



namespace VesselNotesNS
{

    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    class LaunchCheck : MonoBehaviour
    {
        UnityAction launchDelegate;
        UnityAction defaultLaunchDelegate;

        const int WIDTH = 300;
        const int HEIGHT = 200;

        int btnId;
        internal void Start()
        {
            if (!HighLogic.CurrentGame.Parameters.CustomParams<VN_Settings>().checkForVesselNotesLog)
                return;

            ButtonManager.BtnManager.InitializeListener(EditorLogic.fetch.launchBtn, EditorLogic.fetch.launchVessel, "VesselNotes");

            btnId = ButtonManager.BtnManager.AddListener(EditorLogic.fetch.launchBtn, OnLaunchButtonInput, "VesselNotes", "VesselNotes");
        }


        public void OnLaunchButtonInput()
        {
            if (!VesselHasNotes())
            {
                PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f),
                   new Vector2(0.5f, 0.5f),
                   new MultiOptionDialog("Vessel Notes & Logs",
                       "No part installed has the DataLog part installed\n\n" +
                       "Plese select your option from the choices below",
                       "No DataLog Part",
                       HighLogic.UISkin,
                       new Rect(0.5f, 0.5f, WIDTH, HEIGHT),
                       //new DialogGUIFlexibleSpace(),
                       new DialogGUIVerticalLayout(
                           new DialogGUIFlexibleSpace(),

                           new DialogGUIHorizontalLayout(
                               new DialogGUIFlexibleSpace(),
                               new DialogGUIButton("OK to launch",
                                   delegate
                                   {
                                       //ResetDelegates();
                                       //defaultLaunchDelegate();
                                       ButtonManager.BtnManager.InvokeNextDelegate(btnId, "VesselNotes-next");

                                   }, 240.0f, 30.0f, true),
                                new DialogGUIFlexibleSpace()
                            ),

                            //new DialogGUIFlexibleSpace(),

                            new DialogGUIHorizontalLayout(
                               new DialogGUIFlexibleSpace(),
                               new DialogGUIButton("Return to Editor", () => { }, 240.0f, 30.0f, true),
                               new DialogGUIFlexibleSpace()
                               )
                           )
                       ),
                        false,
                        HighLogic.UISkin);
            }
            else
            {
                //ResetDelegates();
                Log.Info("VesselNotes.LaunchCheck.OnLaunchButtonInput 3");
                //defaultLaunchDelegate();
                ButtonManager.BtnManager.InvokeNextDelegate(btnId, "VesselNotes-next");
            }
        }

        void OnExitButtonInput()
        {
            ResetDelegates();
        }

        internal void ResetDelegates()
        {
            EditorLogic.fetch.launchBtn.onClick.RemoveListener(launchDelegate);
            EditorLogic.fetch.launchBtn.onClick.AddListener(defaultLaunchDelegate);
        }


        bool VesselHasNotes()
        {
            foreach (var p in  EditorLogic.fetch.ship.Parts)
            {
                if (p.FindModuleImplementing<VesselNotesLogs>())
                    return true;
            }
            return false;
        }
    }
}
