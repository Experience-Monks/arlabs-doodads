using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Jam3.Util
{
    public class StringUtil
    {
        /// <summary>
        /// Takes a template string and returns the parameters inside it.
        /// Taken from https://stackoverflow.com/questions/5346158/parse-string-using-format-template
        /// </summary>
        /// <param name="template">The template string like "{0}-{1}".</param>
        /// <param name="str">The actual string to evaluate.</param>
        /// <returns></returns>
        public static List<string> ReverseStringFormat(string template, string str)
        {
            //Handels regex special characters.
            template = Regex.Replace(template, @"[\\\^\$\.\|\?\*\+\(\)]", m => "\\" + m.Value);

            string pattern = "^" + Regex.Replace(template, @"\{[0-9]+\}", "(.*?)") + "$";

            Regex r = new Regex(pattern);
            Match match = r.Match(str);

            List<string> ret = new List<string>();

            for (int i = 1; i < match.Groups.Count; i++)
            {
                ret.Add(match.Groups[i].Value);
            }
            return ret;
        }
    }

    public static class StringExtensions
    {
        /// <summary>
        /// Whether the string is null or has 0 characters.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string s)
        {
            return s == null || s.Length <= 0;
        }
    }
}