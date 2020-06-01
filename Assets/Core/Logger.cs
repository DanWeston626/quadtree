
#if UNITY_EDITOR
    /// <summary>
    /// Enable verbose debug logging.
    /// Silent in builds
    /// </summary>
    #define DEBUG
#endif

using UnityEngine;

namespace core {
    public static class Log {

        /// <summary>
        /// Debug Log. Enabled/disabled with DEBUG define
        /// </summary>
        /// <param name="log">formatted string i.e "hello {0}"</param>
        /// <param name="args">parameters for formatted string</param>
        public static void dlog(string log, params object[] args) {
#if DEBUG
            Debug.Log(string.Format("<color=green>{0}</color>",string.Format(log, args)));
#endif
        }
    }
}
