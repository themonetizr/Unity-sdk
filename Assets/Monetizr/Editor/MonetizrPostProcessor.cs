﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Monetizr.Editor;
using System.IO;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

public class MonetizrPostProcessor {
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
    {
#if UNITY_IOS
        var monetizr = MonetizrEditor.GetMonetizrSettings();

        if (buildTarget == BuildTarget.iOS && monetizr.useIosNativePlugin) {
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
#endif
    }
}
