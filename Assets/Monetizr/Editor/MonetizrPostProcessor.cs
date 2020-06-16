using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Monetizr.Editor;
using System.IO;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
#if UNITY_ANDROID
using UnityEditor.Android;
#endif

namespace Monetizr.Editor
{
    public class MonetizrPostProcessor {
    [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
        {
            var monetizr = MonetizrEditor.GetMonetizrSettings();
            if (monetizr == null)
            {
                Debug.LogWarning("MONETIZR: Could not load settings. Build will still continue!");
                return;
            }
    #if UNITY_IOS

            if (buildTarget == BuildTarget.iOS)
            {
                if (monetizr.useIosNativePlugin)
                {
    #if UNITY_2019_3_OR_NEWER
                    // Unity 2019.3 requires much less fiddling with project properties.
                    var projPath = buildPath + "/Unity-iPhone.xcodeproj/project.pbxproj";
                    var proj = new PBXProject();
                    proj.ReadFromFile(projPath);

                    var targetGuid = proj.GetUnityFrameworkTargetGuid();

                    var bridgePath = buildPath + "/Libraries/Monetizr/Plugins/iOS/MonetizrUnityBridge.m";
                    var bridgeFile = File.ReadAllText(bridgePath);
                    var bundleId = PlayerSettings.applicationIdentifier.Split('.');
                    bridgeFile = bridgeFile.Replace("{POST-PROCESS-OVERWRITE}", "<UnityFramework/UnityFramework-Swift.h>");
                    File.WriteAllText(bridgePath, bridgeFile);

                    var frameworkHeaderPath = buildPath + "/UnityFramework/UnityFramework.h";
                    var frameworkHeaderFile = File.ReadAllText(frameworkHeaderPath);
                    frameworkHeaderFile = frameworkHeaderFile.Insert(0, "#import \"MonetizrUnityBridge.h\"\n");
                    File.WriteAllText(frameworkHeaderPath, frameworkHeaderFile);

                    var bridgeHeaderGuid = proj.FindFileGuidByProjectPath("Libraries/Monetizr/Plugins/iOS/MonetizrUnityBridge.h");
                    proj.AddPublicHeaderToBuild(targetGuid, bridgeHeaderGuid);

                    proj.WriteToFile(projPath);
    #else
                    var projPath = buildPath + "/Unity-iPhone.xcodeproj/project.pbxproj";
                    var proj = new PBXProject();
                    proj.ReadFromFile(projPath);

                    var targetGuid = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
                    var projGuid = proj.ProjectGuid();

                    // Set the correct path to Swift header. Hopefully nothing else messes with this.
                    var bridgePath = buildPath + "/Libraries/Monetizr/Plugins/iOS/MonetizrUnityBridge.m";
                    var bridgeFile = File.ReadAllText(bridgePath);
                    var bundleId = PlayerSettings.applicationIdentifier.Split('.');
                    bridgeFile = bridgeFile.Replace("{POST-PROCESS-OVERWRITE}", "\"" + bundleId[bundleId.Length - 1] + "-Swift.h\"");
                    File.WriteAllText(bridgePath, bridgeFile);

                    if (monetizr.iosAutoconfig)
                    {
                        // Automatically set Swift version
                        proj.SetBuildProperty(targetGuid, "SWIFT_VERSION", "4.0");
                        proj.SetBuildProperty(projGuid, "SWIFT_VERSION", "5.0");
                    }

                    if (monetizr.iosAutoBridgingHeader)
                    {
                        // Automatically set bridging header to _only_ Monetizr
                        proj.SetBuildProperty(targetGuid, "SWIFT_OBJC_BRIDGING_HEADER", "Libraries/Monetizr/Plugins/iOS/MonetizrUnityBridge.h");
                    }

                    proj.WriteToFile(projPath);
    #endif
                }
                else
                {
                    // Native plugin is disabled, need to remove files from project.
                    // Otherwise it will look for the Monetizr SDK and fail.
                    var projPath = buildPath + "/Unity-iPhone.xcodeproj/project.pbxproj";
                    var proj = new PBXProject();
                    proj.ReadFromFile(projPath);

                    var filesToRemove = new List<string>(new string[]{
                        "Libraries/Monetizr/Plugins/iOS/MonetizrUnityBridge.h",
                        "Libraries/Monetizr/Plugins/iOS/MonetizrUnityBridge.m",
                        "Libraries/Monetizr/Plugins/iOS/MonetizrUnityInterface.swift"
                    });
                    filesToRemove.ForEach(f => proj.RemoveFile(proj.FindFileGuidByProjectPath(f)));

                    proj.WriteToFile(projPath);
                }
            }
    #endif
    #if UNITY_ANDROID
            if (buildTarget == BuildTarget.Android && monetizr.useAndroidNativePlugin)
            {
                if (!EditorUserBuildSettings.exportAsGoogleAndroidProject)
                {
                    Debug.LogWarning("MONETIZR: Android native plugin is enabled but you are exporting an apk. If you wish to integrate native plugin, export an Android Studio project!");
                    return;
                }
                //TODO: A way to automate Android builds would be nice
            }
    #endif
        }
    }
}


