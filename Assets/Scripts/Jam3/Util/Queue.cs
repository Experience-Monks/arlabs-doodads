using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Jam3.Util
{
    /// <summary>
    /// Jam3's Queue extensions.
    /// </summary>
    public static class QueueExtensions
    {
        /// <summary>
        /// Converts the contents of the Queue<string> to a human-readable string representation.
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public static string Description<T>(this Queue<T> q)
        {
            StringBuilder sb = new StringBuilder("Queue:");
            for (int i = 0; i < q.Count; i++)
            {
                sb.AppendFormat("\n{0}: {1}", i, q.ElementAt(i));
            }
            return sb.ToString();
        }
    }
}