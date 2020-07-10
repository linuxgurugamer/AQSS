// Much of this file copied from MagiCore

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using KSP.Localization;

using static AutoQuickSaveSystem.AutoQuickSaveSystem;

namespace AutoQuickSaveSystem
{
    /// <summary>
    /// Similar to variable replacement, but purely for strings. For Dated Quicksave and Sensible Screenshot. Not for language translations unfortunately
    /// </summary>
    public class StringTranslation
    {
        /// <summary>
        /// The main entry point. Takes a string, replaces "variables", and returns the final string. Handles common values like "[year]"
        /// </summary>
        /// <param name="original">The original, source string</param>
        /// <param name="caller">The calling mod's name</param>
        /// <param name="DateString">A C# date string</param>
        /// <param name="extraVariables">Any additional variables to use for replacements</param>
        /// <returns>The string post replacements</returns>
        public static string AddFormatInfo(string original, string caller, string DateString, Dictionary<string, string> extraVariables = null)
        {
            string convertedDate = DateTime.Now.ToString(DateString);

            string replaced = original;
            replaced = ReplaceToken(replaced, "date", convertedDate);
            //Take original and replace all the common things (aka, the variables all our mods share, like [year])
            replaced = ReplaceStandardTokens(replaced);

            //Take that string and then replace all the extraVariables (mod specific things that get passed through, like the event that triggered a screenshot)
            if (extraVariables != null)
            {
                foreach (KeyValuePair<string, string> kvp in extraVariables)
                {
                    replaced = ReplaceToken(replaced, kvp.Key, kvp.Value); //ReplaceToken is a function that replaces [X] with the value (regardless of case or if it's wrapped with [] or <>)
                }
            }
            replaced = Localizer.Format("<<1>>", replaced);
            return replaced;
        }

        /// <summary>
        /// Replaces all instances of a particular token "[token] or <token>" with the provided value
        /// </summary>
        /// <param name="sourceString">The original string to perform the replacement in</param>
        /// <param name="variable">The variable to replace with the value</param>
        /// <param name="value">The value to replace the variable with</param>
        /// <returns>The string post replacement</returns>
        public static string ReplaceToken(string sourceString, string variable, string value)
        {
            string lwrString = sourceString.ToLower();

            int index;
            while ((index = lwrString.IndexOf("[" + variable.ToLower() + "]")) >= 0 || (index = lwrString.IndexOf("<" + variable.ToLower() + ">")) >= 0)
            {
                string newStr = sourceString.Substring(0, index) + value; //verify
                if (index + 2 + variable.Length < sourceString.Length)
                    newStr += sourceString.Substring(index + 2 + variable.Length);
                sourceString = newStr;
                lwrString = sourceString.ToLower();
            }

            return sourceString;
        }

        /// <summary>
        /// Replaces the common tokens like "[year]" with their appropriate values as gathered from KSP
        /// </summary>
        /// <param name="sourceString">The source string to act on</param>
        /// <returns>The string post replacements</returns>
        public static string ReplaceStandardTokens(string sourceString)
        {
            string str = sourceString;

            bool counterFound = str.Contains("cnt");



            str = ReplaceToken(str, "UT", Planetarium.fetch != null ? Math.Round(Planetarium.GetUniversalTime()).ToString() : "0");
            //str = ReplaceToken(str, "save", HighLogic.SaveFolder != null && HighLogic.SaveFolder.Trim().Length > 0 ? HighLogic.SaveFolder : "NA");
            //str = ReplaceToken(str, "version", Versioning.GetVersionString());
            str = ReplaceToken(str, "vessel", HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null ? FlightGlobals.ActiveVessel.vesselName : "");
            str = ReplaceToken(str, "body", Planetarium.fetch != null ? Planetarium.fetch.CurrentMainBody.GetDisplayName() : "");
            str = ReplaceToken(str, "situation", HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null ? FlightGlobals.ActiveVessel.situation.ToString() : "");
            str = ReplaceToken(str, "biome", HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null ? ScienceUtil.GetExperimentBiome(FlightGlobals.ActiveVessel.mainBody, FlightGlobals.ActiveVessel.latitude, FlightGlobals.ActiveVessel.longitude) : "");


            int[] times = { 0, 0, 0, 0, 0 };
            if (Planetarium.fetch != null)
                times = ConvertUT(Planetarium.GetUniversalTime());
            if (!counterFound)
                str = ReplaceToken(str, "year", times[0].ToString());
            str = ReplaceToken(str, "year0", times[0].ToString("D3"));
            if (!counterFound)
                str = ReplaceToken(str, "day", times[1].ToString());
            str = ReplaceToken(str, "day0", times[1].ToString("D3"));
            if (!counterFound)
                str = ReplaceToken(str, "hour", times[2].ToString());
            str = ReplaceToken(str, "hour0", times[2].ToString("D2"));
            if (!counterFound)
                str = ReplaceToken(str, "min", times[3].ToString());
            str = ReplaceToken(str, "min0", times[3].ToString("D2"));
            if (!counterFound)
                str = ReplaceToken(str, "sec", times[4].ToString());
            str = ReplaceToken(str, "sec0", times[4].ToString("D2"));


            string time = KSPUtil.PrintTimeCompact(0, false);
            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null)
                time = KSPUtil.PrintTimeCompact((int)FlightGlobals.ActiveVessel.missionTime, false);
            time = time.Replace(":", "-"); //Can't use colons in filenames on Windows, so we'll replace them with "-"

            str = ReplaceToken(str, "MET", time);

            if (str.Contains("cnt"))
            {
                int zeroes = 0;
                while (str.Contains("cnt" + Repeated('0', zeroes + 1)))
                    zeroes++;
                string token = "cnt" + Repeated('0', zeroes);

                string searchstr = ReplaceToken(str, token, "*.sfs");
                int beginning = str.IndexOf(token);
                string[] files = Directory.GetFiles(SaveDir, searchstr);
                int cnt = files.Length;
                for (int i = 0; i < files.Length; i++)
                {
                    int z = zeroes;
                    string counterStr = files[i].Substring(SaveDir.Length + beginning);
                    if (zeroes == 0)
                    {
                        while (z < counterStr.Length && Char.IsDigit(counterStr[z]))
                            z++;
                    }
                    counterStr = counterStr.Substring(0, z);

                    int x = 0;
                    int.TryParse(counterStr, out x);
                    cnt = Math.Max(cnt, x);
                }
                str = ReplaceToken(str, token, (cnt + 1).ToString("D" + zeroes.ToString()));
            }
            return str;
        }

        /// <summary>
        /// Repeat a string a certain number of times
        /// </summary>
        /// <param name="s"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        static string Repeated(char value, int count)
        {
            return new string(value, count);
#if false
            return new StringBuilder(value.Length * count).Insert(0, value, count).ToString();
#endif
#if false
            string str = "";
            while (i-- > 0)
                str += s;
            return str;
#endif
        }
        public static string SaveDir
        {
            get
            {
                return Path.GetFullPath(Path.Combine(Path.Combine(KSPUtil.ApplicationRootPath, "saves"), HighLogic.SaveFolder));
            }
        }


        public static int[] ConvertUT(double UT)
        {
            double time = UT;
            int[] ret = { 0, 0, 0, 0, 0 };
            ret[0] = (int)Math.Floor(time / (KSPUtil.dateTimeFormatter.Year)) + 1; //year
            time %= (KSPUtil.dateTimeFormatter.Year);
            ret[1] = (int)Math.Floor(time / KSPUtil.dateTimeFormatter.Day) + 1; //days
            time %= (KSPUtil.dateTimeFormatter.Day);
            ret[2] = (int)Math.Floor(time / (KSPUtil.dateTimeFormatter.Hour)); //hours
            time %= (KSPUtil.dateTimeFormatter.Hour);
            ret[3] = (int)Math.Floor(time / (KSPUtil.dateTimeFormatter.Minute)); //minutes
            time %= (KSPUtil.dateTimeFormatter.Minute);
            ret[4] = (int)Math.Floor(time); //seconds

            return ret;
        }

    }

}