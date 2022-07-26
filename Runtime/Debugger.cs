namespace TT.Debugger { 
    using UnityEngine;
    using System;

    public static class Debugger {

        private const string PREFIX = "<b><color=#{1}>{0}</color></b>";
        private const string FOCUS_START_KEY = "<f>";
        private const string FOCUS_END_KEY = "</f>";
        private const string FOCUS_END_COLOR = "</b></color>";

        private static string FOCUS_START_COLOR => $"<color=#{ColorUtility.ToHtmlStringRGB(Settings.FocusColor.Get())}><b>";

        private static DebuggerSettings Settings {
            get {
                if (!Application.isPlaying) {
                    _settings = null;
                }
                if (_settings == null) {
                    _settings = DebuggerSettings.Get;
                }
                return _settings;
            }
        }
        private static DebuggerSettings _settings;

        private static string Prefix(this object logger) => string.Format(PREFIX, logger, ColorUtility.ToHtmlStringRGB(Settings.PrefixColor(logger.GetType())));

        private static void BaseLog(this object logger, object message, Action<object> logMethod, params string[] tags) {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            foreach (var tag in tags) {
                if (Settings.Tags.ContainsKey(tag) && !Settings.Tags[tag].Get()) {
                    return;
                }
            }
            logMethod?.Invoke($"{logger.Prefix()}\n{message.ToString().Replace(FOCUS_START_KEY, FOCUS_START_COLOR).Replace(FOCUS_END_KEY, FOCUS_END_COLOR)}");
#endif
        }

        public static void Log(this object logger, object message, params string[] tags) => BaseLog(logger, message, Debug.Log, tags);
        public static void LogError(this object logger, object message, params string[] tags) => BaseLog(logger, message, Debug.LogError, tags);
        public static void LogWarning(this object logger, object message, params string[] tags) => BaseLog(logger, message, Debug.LogWarning, tags);
    }
}
