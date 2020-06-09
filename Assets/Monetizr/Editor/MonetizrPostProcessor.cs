using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS.Xcode;
#endif

public class MonetizrPostProcessor {
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
    {
#if UNITY_IOS
        if(buildTarget == BuildTarget.iOS) {

        }
#endif
    }
}
