using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Monetizr.Editor;
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


            if (monetizr.iosAutoconfig)
            {
                // Automatically set Swift version
                proj.SetBuildProperty(targetGuid, "SWIFT_VERSION", "4.0");
            }

            if (monetizr.iosAutoBridgingHeader)
            {
                // Automatically set bridging header to _only_ Monetizr
                proj.SetBuildProperty(targetGuid, "SWIFT_OBJC_BRIDGING_HEADER", "Libraries/Monetizr/Plugins/iOS/MonetizrUnityBridge.h");
            }
        }
#endif
    }
}
