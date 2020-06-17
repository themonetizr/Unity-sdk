using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Monetizr.Editor
{
    [CustomEditor(typeof(MonetizrSettings))]
    public class MonetizrSettingsEditor : UnityEditor.Editor
    {
        private void AddDefineSymbolsIos ()
        {
            string[] symbols = {"MONETIZR_IOS_NATIVE"};
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup ( BuildTargetGroup.iOS );
            List<string> allDefines = definesString.Split ( ';' ).ToList ();
            allDefines.AddRange ( symbols.Except ( allDefines ) );
            PlayerSettings.SetScriptingDefineSymbolsForGroup (
                EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join ( ";", allDefines.ToArray () ) );
        }
        
        private void RemoveDefineSymbolsIos ()
        {
            string[] symbols = {"MONETIZR_IOS_NATIVE"};
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup ( BuildTargetGroup.iOS );
            List<string> allDefines = definesString.Split ( ';' ).ToList ();
            List<string> newDefines = new List<string>();
            newDefines.AddRange ( allDefines.Except ( symbols ) );
            PlayerSettings.SetScriptingDefineSymbolsForGroup (
                EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join ( ";", newDefines.ToArray () ) );
        }
        
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
        private SerializedProperty _applePay;
        private SerializedProperty _applePayMerchantId;
        private SerializedProperty _applePayCompanyName;
        private SerializedProperty _applePayTestMode;
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
            _applePay = serializedObject.FindProperty("applePay");
            _applePayMerchantId = serializedObject.FindProperty("applePayMerchantId");
            _applePayCompanyName = serializedObject.FindProperty("applePayCompanyName");
            _applePayTestMode = serializedObject.FindProperty("applePayTestMode");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.LabelField("Monetizr Unity Plugin 2.0.0", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("API access token:");
            EditorGUILayout.PropertyField(_accessToken, GUIContent.none);
            if (GUILayout.Button("Use public testing token"))
            {
                _accessToken.stringValue = "4D2E54389EB489966658DDD83E2D1";
            }
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_showFullscreenAlerts, new GUIContent("Show testing alerts"));
            EditorGUILayout.PropertyField(_useAndroidNativePlugin);
            EditorGUILayout.PropertyField(_useIosNativePlugin, new GUIContent("Use iOS Native Plugin"));
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
            if (_useIosNativePlugin.boolValue)
            {
                if (!defines.Contains("MONETIZR_IOS_NATIVE"))
                {
                    AddDefineSymbolsIos();
                }
            }
            else
            {
                if (defines.Contains("MONETIZR_IOS_NATIVE"))
                {
                    RemoveDefineSymbolsIos();
                }
            }
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
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Apple Pay", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_applePay, new GUIContent("Apple Pay"));
                if(_applePay.boolValue)
                {
                    EditorGUILayout.HelpBox("You will need to obtain a CSR from Monetizr to complete Apple Pay setup - see docs.themonetizr.com for more information!", MessageType.Info);
                    EditorGUILayout.PropertyField(_applePayMerchantId, new GUIContent("Merchant ID"));
                    EditorGUILayout.PropertyField(_applePayCompanyName, new GUIContent("Company Name"));
                    EditorGUILayout.PropertyField(_applePayTestMode, new GUIContent("Payment Test Mode"));
                }

#if !UNITY_2019_3_OR_NEWER
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("iOS native build settings:", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_iosBridging, new GUIContent("Set bridging header"));
                EditorGUILayout.PropertyField(_iosAutoconfig, new GUIContent("Set Swift version"));
#endif
                EditorGUILayout.Space();
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
                EditorGUILayout.PropertyField(_testingMode, new GUIContent("Payment Test Mode"));
                EditorGUILayout.PropertyField(_bigScreenSettings, new GUIContent("Big Screen Theming"), true);
            }

            if (_bigScreen.boolValue || !_useAndroidNativePlugin.boolValue || !_useIosNativePlugin.boolValue)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("UGUI specific settings:", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_colorScheme, true);
                EditorGUILayout.PropertyField(_showLoadingScreen);
                EditorGUILayout.PropertyField(_useDeviceLanguage);
                EditorGUILayout.PropertyField(_webGlNewTab, new GUIContent("WebGL: open checkout in new tab"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
