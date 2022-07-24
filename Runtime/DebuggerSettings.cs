namespace TT.Debugger {
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    [Serializable] public class DebuggerSettings {

        [Serializable] public class ColorContainer {
            public Color Get() => new Color(r, g, b);
            public void Set(Color value) {
                r = value.r;
                g = value.g;
                b = value.b;
            }
            public float r;
            public float g;
            public float b;
        }

        [Serializable] public class BoolContainer {
            public bool Get() => b;
            public void Set(bool value) => b = value;
            public bool b;
        }

        static string FULL_PATH => string.Join('/', Application.streamingAssetsPath, PATH);
        const string PATH = "debugger_settings.json";
        const float D = 100,
            H_D = 360;

        public ColorContainer FocusColor = new ColorContainer();
        public ColorContainer DefaultPrefixColor = new ColorContainer();
        public Dictionary<string, ColorContainer> PrefixColors = new Dictionary<string, ColorContainer>();
        public Dictionary<string, BoolContainer> Tags = new Dictionary<string, BoolContainer>();

        static void CreateFolder() {
            if (!File.Exists(FULL_PATH)) {
                if (!Directory.Exists(Application.streamingAssetsPath)) {
                    Directory.CreateDirectory(Application.streamingAssetsPath);
                }
                var settings = new DebuggerSettings();
                settings.FocusColor.Set(Color.HSVToRGB(58f / H_D, 52f / D, 86f / D));
                settings.DefaultPrefixColor.Set(Color.HSVToRGB(177f / H_D, 77f / D, 91f / D));
                var temp = JsonConvert.SerializeObject(settings);
                File.WriteAllText(FULL_PATH, temp);
            }
        }

        public static DebuggerSettings Get {
            get {
                CreateFolder();
                var temp = File.ReadAllText(FULL_PATH);
                var settings = JsonConvert.DeserializeObject<DebuggerSettings>(temp);
                return settings;
            }
        }

        public static void Set(DebuggerSettings settings) {
            var temp = JsonConvert.SerializeObject(settings);
            CreateFolder();
            File.WriteAllText(FULL_PATH, temp);
        }

        public Color PrefixColor(Type type) {
            if (PrefixColors.ContainsKey(type.Name)) {
                return PrefixColors[type.Name].Get();
            }
            return DefaultPrefixColor.Get();
        }

        public DebuggerSettings() { }
    }
}
