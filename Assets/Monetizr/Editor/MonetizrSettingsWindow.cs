using UnityEditor;
using UnityEngine;

namespace Monetizr.Editor
{
    public class MonetizrSettingsWindow : EditorWindow
    {
        private MonetizrSettings _settings;
        private UnityEditor.Editor _settingsEditor;
        private Vector2 _scrollPos = Vector2.zero;
        
        [MenuItem("Window/Monetizr Settings")]
        private static void OpenWindow()
        {
            MonetizrSettingsWindow window = (MonetizrSettingsWindow)EditorWindow.GetWindow(typeof(MonetizrSettingsWindow));
            window.titleContent.text = "Monetizr";
            window._settings = MonetizrEditor.GetMonetizrSettings();
            window._settingsEditor = UnityEditor.Editor.CreateEditor(window._settings);
            window.Show();
        }

        private void OnGUI()
        {
            if (_settings == null)
            {
                EditorGUILayout.HelpBox("Monetizr setup is almost finished! Click Finish Setup to create a settings file in your project resources.", MessageType.Info);
                if (GUILayout.Button("Finish Setup"))
                {
                    MonetizrEditor.CreateMonetizrSettings();
                    _settings = MonetizrEditor.GetMonetizrSettings();
                }
            }
            else
            {
                if (_settingsEditor == null)
                    _settingsEditor = UnityEditor.Editor.CreateEditor(_settings, typeof(MonetizrSettings));
                if (_settingsEditor.target == null)
                {
                    _settingsEditor.target = _settings;
                }
                if (_settingsEditor.target != null)
                {
                    EditorGUILayout.BeginScrollView(_scrollPos);
                    _settingsEditor.OnInspectorGUI();
                    EditorGUILayout.EndScrollView();
                }
            }
        }
    }
}
