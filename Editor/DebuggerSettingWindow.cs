namespace TT.Debugger {
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public class DebuggerSettingsWindow : EditorWindow {

        const string WINDOW_TITLE = "Debugger Settings",
            FOCUS_COLOR = "Focus Color",
            DEFAULT_PREFIX_COLOR = "Default Prefix Color",
            PREFIX_HEADER = "Prefix Colors",
            TAG_HEADER = "Tags",
            ADD = "+",
            REMOVE = "-";

        const float ADD_WIDTH = 20f;

        Vector2 _scroll;
        bool _showPrefixColors,
            _showTags;

        DebuggerSettings _settings;
        List<string> _prefixKeys = new List<string>();
        List<DebuggerSettings.ColorContainer> _prefixValues = new List<DebuggerSettings.ColorContainer>();
        List<string> _tagKeys = new List<string>();
        List<DebuggerSettings.BoolContainer> _tagValues = new List<DebuggerSettings.BoolContainer>();

        [MenuItem("Tools/Totoro Tools/Debugger Settings")]
        public static void ShowWindow() {
            GetWindow(t: typeof(DebuggerSettingsWindow), utility: false, title: WINDOW_TITLE);
        }

        private void OnGUI() {
            InitializeSettings();
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            UpdateDefaultSettingsGUI();
            EditorGUILayout.Space();
            UpdateDictionaryGUI(ref _showPrefixColors, PREFIX_HEADER, ref _prefixKeys, ref _prefixValues, new DebuggerSettings.ColorContainer(),
                ref _settings.PrefixColors, GetColor, EditorGUILayout.ColorField, SetColor);
            EditorGUILayout.Space();
            UpdateDictionaryGUI(ref _showTags, TAG_HEADER, ref _tagKeys, ref _tagValues, new DebuggerSettings.BoolContainer(),
                ref _settings.Tags, GetBool, EditorGUILayout.Toggle, SetBool);
            EditorGUILayout.EndScrollView();
            DebuggerSettings.Set(_settings);
        }

        private void InitializeSettings() {
            if (_settings == null) {
                _settings = DebuggerSettings.Get;
                foreach (var key in _settings.PrefixColors.Keys) {
                    _prefixKeys.Add(key);
                    _prefixValues.Add(_settings.PrefixColors[key]);
                }
                foreach (var key in _settings.Tags.Keys) {
                    _tagKeys.Add(key);
                    _tagValues.Add(_settings.Tags[key]);
                }
            }
        }

        private void UpdateDefaultSettingsGUI() {
            _settings.FocusColor.Set(EditorGUILayout.ColorField(FOCUS_COLOR, _settings.FocusColor.Get()));
            _settings.DefaultPrefixColor.Set(EditorGUILayout.ColorField(DEFAULT_PREFIX_COLOR, _settings.DefaultPrefixColor.Get()));
        }

        private void UpdateDictionaryGUI<T, U>(ref bool show, string header, ref List<string> keys, ref List<T> values, T defaultValue,
            ref Dictionary<string, T> settingDictionary, Func<T, U> getValue, Func<U, GUILayoutOption[], U> setValueGUI, Action<T, U> setValue) {
            HeaderDictionaryGUI(ref show, header, ref keys, ref values, defaultValue);
            if (show) {
                for (int i = 0; i < keys.Count; i++) {
                    if (!UpdateDictionaryGUI(i, ref keys, ref values, ref settingDictionary, getValue, setValueGUI, setValue)) {
                        break;
                    }
                }
            }
        }

        private void HeaderDictionaryGUI<T>(ref bool show, string header, ref List<string> keys, ref List<T> values, T defaultValue) {
            EditorGUILayout.BeginHorizontal();

            show = EditorGUILayout.BeginFoldoutHeaderGroup(show, header);

            if (show && GUILayout.Button(ADD, GUILayout.Width(ADD_WIDTH))) {
                keys.Add(string.Empty);
                values.Add(defaultValue);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.EndHorizontal();
        }

        private bool UpdateDictionaryGUI<T, U>(int i,
            ref List<string> keys, ref List<T> values, ref Dictionary<string, T> settingDictionary,
            Func<T, U> getValue, Func<U, GUILayoutOption[], U> setValueGUI, Action<T, U> setValue) {

            EditorGUILayout.BeginHorizontal();

            var t = keys[i];
            keys[i] = EditorGUILayout.DelayedTextField(t);

            var c = getValue(values[i]);
            setValue(values[i], setValueGUI(c, new GUILayoutOption[0]));

            var typeChanged = t != keys[i];
            var oldType = !string.IsNullOrEmpty(t);
            var newType = !string.IsNullOrEmpty(keys[i]);
            var colorChanged = !c.Equals(getValue(values[i]));

            if (typeChanged) {

                if (oldType) {
                    if (settingDictionary.ContainsKey(t)) {
                        settingDictionary.Remove(t);
                    }
                }

                if (newType) {
                    settingDictionary[keys[i]] = values[i];
                }

            }
            else if (colorChanged && newType) {
                settingDictionary[keys[i]] = values[i];
            }

            var remove = GUILayout.Button(REMOVE, GUILayout.Width(ADD_WIDTH));

            if (remove) {
                if (settingDictionary.ContainsKey(keys[i])) {
                    settingDictionary.Remove(keys[i]);
                }
                keys.RemoveAt(i);
                values.RemoveAt(i);
            }

            EditorGUILayout.EndHorizontal();
            return !remove;
        }

        private Color GetColor(DebuggerSettings.ColorContainer container) => container.Get();
        private void SetColor(DebuggerSettings.ColorContainer container, Color color) => container.Set(color);

        private bool GetBool(DebuggerSettings.BoolContainer container) => container.Get();
        private void SetBool(DebuggerSettings.BoolContainer container, bool b) => container.Set(b);
    }
}
