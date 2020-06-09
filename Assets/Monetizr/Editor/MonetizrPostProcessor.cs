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

public class MonetizrPostProcessor {
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
    {
#if UNITY_IOS
        var monetizr = MonetizrEditor.GetMonetizrSettings();

        if (buildTarget == BuildTarget.iOS && monetizr.useIosNativePlugin) {
            var projPath = buildPath + "/Unity-iPhone.xcodeproj/project.pbxproj";
            var proj = new PBXProject();
            proj.ReadFromFile(projPath);

            var targetGuid = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
            var projGuid = proj.ProjectGuid();

            // Set the correct path to Swift header. Hopefully nothing else messes with this.
            var bridgePath = buildPath + "/Libraries/Monetizr/Plugins/iOS/MonetizrUnityBridge.m";
            var bridgeFile = File.ReadAllText(bridgePath);
            var bundleId = PlayerSettings.applicationIdentifier.Split('.');
            bridgeFile = bridgeFile.Replace("{BUNDLEID(replbypostprocess)}", bundleId[bundleId.Length - 1]);
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
        }
#endif
    }
}
