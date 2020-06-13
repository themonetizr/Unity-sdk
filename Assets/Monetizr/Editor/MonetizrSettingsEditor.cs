using System;
using UnityEngine;
using UnityEditor;

namespace Monetizr.Editor
{
    [CustomEditor(typeof(MonetizrSettings))]
    public class MonetizrSettingsEditor : UnityEditor.Editor
    {
        private SerializedProperty _accessToken;
        private SerializedProperty _colorScheme;
        private SerializedProperty _bigScreen;
        private SerializedProperty _bigScreenSettings;
        private SerializedProperty _showFullscreenAlerts;
        private SerializedProperty _showLoadingScreen;
        private SerializedProperty _useDeviceLanguage;
        private SerializedProperty _showPolicyLinks;
        private SerializedProperty _webGlNewTab;
        private SerializedProperty _testingMode;
        private SerializedProperty _useAndroidNativePlugin;
        private SerializedProperty _useIosNativePlugin;
        private SerializedProperty _iosBridging;
        private SerializedProperty _iosAutoconfig;
        private void OnEnable()
        {
            _accessToken = serializedObject.FindProperty("accessToken");
            _colorScheme = serializedObject.FindProperty("colorScheme");
            _bigScreen = serializedObject.FindProperty("bigScreen");
            _bigScreenSettings = serializedObject.FindProperty("bigScreenSettings");
            _showFullscreenAlerts = serializedObject.FindProperty("showFullscreenAlerts");
            _showLoadingScreen = serializedObject.FindProperty("showLoadingScreen");
            _useDeviceLanguage = serializedObject.FindProperty("useDeviceLanguage");
            _showPolicyLinks = serializedObject.FindProperty("showPolicyLinks");
            _testingMode = serializedObject.FindProperty("testingMode");
            _webGlNewTab = serializedObject.FindProperty("webGlNewTab");
            _useAndroidNativePlugin = serializedObject.FindProperty("useAndroidNativePlugin");
            _useIosNativePlugin = serializedObject.FindProperty("useIosNativePlugin");
            _iosBridging = serializedObject.FindProperty("iosAutoBridgingHeader");
            _iosAutoconfig = serializedObject.FindProperty("iosAutoconfig");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.LabelField("API access token:");
            EditorGUILayout.PropertyField(_accessToken, GUIContent.none);
            if (GUILayout.Button("Use public testing token"))
            {
                _accessToken.stringValue = "4D2E54389EB489966658DDD83E2D1";
            }
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_showFullscreenAlerts);
            EditorGUILayout.PropertyField(_useAndroidNativePlugin);
            EditorGUILayout.PropertyField(_useIosNativePlugin, new GUIContent("Use iOS Native Plugin"));
            if (_useAndroidNativePlugin.boolValue || _useIosNativePlugin.boolValue)
            {
                EditorGUILayout.HelpBox("Usage of native plugins requires extra setup of dependencies after the build process - see docs.themonetizr.com for more information!", MessageType.Info);
            }
            else
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android ||
                    EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
                {
                    EditorGUILayout.HelpBox("It is recommended to use native plugins on Android and iOS.\n" +
                                            "UGUI views on mobile are provided for preview only and are not feature complete.", MessageType.Info);
                }
            }

            if (_useIosNativePlugin.boolValue)
            {
#if !UNITY_2019_3_OR_NEWER
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("iOS native build settings:", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_iosBridging, new GUIContent("Set bridging header"));
                EditorGUILayout.PropertyField(_iosAutoconfig, new GUIContent("Set Swift version"));
                EditorGUILayout.Space();
#endif
            }
            
            EditorGUILayout.PropertyField(_bigScreen);

            if (_bigScreen.boolValue)
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android ||
                    EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
                {
                    EditorGUILayout.HelpBox("Big Screen view is meant only for desktop platforms and will not be easily usable on mobile.", MessageType.Warning);
                }
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Big Screen view settings:", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_showPolicyLinks);
                EditorGUILayout.PropertyField(_testingMode, new GUIContent("Payment Testing"));
                EditorGUILayout.PropertyField(_bigScreenSettings, new GUIContent("Big Screen Theming"), true);
            }

            if (_bigScreen.boolValue || !_useAndroidNativePlugin.boolValue || !_useIosNativePlugin.boolValue)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Non-native specific settings:", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_colorScheme, true);
                EditorGUILayout.PropertyField(_showLoadingScreen);
                EditorGUILayout.PropertyField(_useDeviceLanguage);
                EditorGUILayout.PropertyField(_webGlNewTab, new GUIContent("WebGL: open checkout in new tab"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

