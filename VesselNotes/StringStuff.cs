using UnityEngine;

namespace VesselNotesNS
{
    internal static class StringStuff
    {
        public static void CopyToClipboard(this string s)
        {
            TextEditor te = new TextEditor();
            te.text = s;
            te.SelectAll();
            te.Copy();
        }
    }

}
