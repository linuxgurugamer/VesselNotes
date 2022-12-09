using System;
using System.IO;
using System.Text;
using static VesselNotesNS.RegisterToolbar;

namespace VesselNotesNS
{

    // [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    //internal class LogsToFile:MonoBehaviour
    internal partial class VesselNotesLogs
    {
        //
        // Following region is from https://gist.github.com/06b/27ae0b00d7321fa683c1
        //
#region encode

        /// <summary>
        /// Creates a new instance of a Guid using the string value, 
        /// then returns the base64 encoded version of the Guid.
        /// </summary>
        /// <param name="value">An actual Guid string (i.e. not a ShortGuid)</param>
        /// <returns></returns>
        public static string Encode(string value)
        {
            Guid guid = new Guid(value);
            return Encode(guid);
        }

        /// <summary>
        /// Encodes the given Guid as a base64 string that is 22 
        /// characters long.
        /// </summary>
        /// <param name="guid">The Guid to encode</param>
        /// <returns></returns>
        public static string Encode(Guid guid)
        {
            string encoded = Convert.ToBase64String(guid.ToByteArray());
            encoded = encoded
                .Replace("/", "_")
                .Replace("+", "-");
            return encoded.Substring(0, 22);
        }
#endregion

        string GetFilename(string dir, Vessel v, ref int cnt)
        {
            string name = KSPUtil.SanitizeFilename(v.vesselName + "-" + Encode(v.id));

            //var dateAndTime = DateTime.Now;
            //name += "-" + (dateAndTime.Year - 2000).ToString() + "." + dateAndTime.Month.ToString("D2") + "." + dateAndTime.Day.ToString("D2");
            //name += "-" + (dateAndTime.Hour).ToString("D2") + "." + (dateAndTime.Minute).ToString("D2");
            //if (cnt > 0)
            //    name += "-" + cnt.ToString();
            Log.Info("GetFilename, name: " + dir + name);
            return dir + name;
        }

        public static string SaveDir { get { return KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/VesselLogs/";} }
        internal void SaveLogsToFile(string from, Vessel v, Part p)
        {
            ConfigNode node = new ConfigNode();
            ConfigNode vesselNode = new ConfigNode();

            Log.Info("SaveLogsToFile, from: " + from);
            
            node.AddValue("VesselName", v.vesselName);
            node.AddValue("VesselId", v.id);
            node.AddValue("GameTime", Planetarium.GetUniversalTime());

            StringBuilder sbPrint = new StringBuilder();

            //var SaveDir = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/VesselLogs/";
            if (!Directory.Exists(SaveDir))
                Directory.CreateDirectory(SaveDir);

            //var allNotesModules = v.FindPartModulesImplementing<VesselNotes>();
            var m = p.FindModuleImplementing<VesselNotesLogs>();

            int cnt = 0;
            sbPrint.Clear();
            if (m.logList.list.Count > 0)
            {
                sbPrint.AppendLine("Part: " + p.partInfo.title);
                sbPrint.AppendLine("Vessel: " + vessel.vesselName);

                 
                foreach (NOTE n in m.logList.list)
                {
                    ConfigNode note = new ConfigNode("VESSELLOG");

                    string str = "";
                    if (n.note != null)
                        str = n.note.Replace("\n", "<EOL>");
                    note.AddValue("NOTE", str);

                    note.AddValue("TITLE", n.title);
                    note.AddValue("GAMEDATETIME", n.gameDateTime);
                    note.AddValue("GUID", n.guid);
                    note.AddValue("VESSEL_ID", n.noteListGuid);
                    note.AddValue("PRIVATENOTE", n.privateNote);
                    //vesselNode.AddNode(note);
                
                    node.AddNode(note);


                    string[] s = n.note.Split('\n');

                    foreach (var s1 in s)
                        sbPrint.AppendLine(s1);
                }
                var s2 = GetFilename(SaveDir, v, ref cnt);
                //while (File.Exists(s1 + ".txt"))
                //{
                //    Log.Info("File found: " + s2 + ".txt");
                //    cnt++;
                //}
                File.WriteAllText(s2 + ".txt", sbPrint.ToString());
                node.Save(s2 + ".cfg");
            }
            ScreenMessages.PostScreenMessage("Logs saved to file", 5, ScreenMessageStyle.UPPER_CENTER);
        }
    }
}
