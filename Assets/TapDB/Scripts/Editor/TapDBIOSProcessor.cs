using TapCommon.Scripts.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace TapDB.Scripts.Editor
{
    public class TapDBIOSProcessor : MonoBehaviour
    {
        [PostProcessBuild(100)]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
        {
            if (buildTarget != BuildTarget.iOS) return;

            var projPath = TapCommonCompile.GetProjPath(path);
            var proj = TapCommonCompile.ParseProjPath(projPath);
            var target = TapCommonCompile.GetUnityTarget(proj);
            var unityFrameworkTarget = TapCommonCompile.GetUnityFrameworkTarget(proj);
            if (TapCommonCompile.CheckTarget(target))
            {
                Debug.LogError("Unity-iPhone is NUll");
                return;
            }
            
            proj.AddFrameworkToProject(unityFrameworkTarget, "AdSupport.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTarget, "CoreMotion.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTarget, "Security.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTarget, "SystemConfiguration.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTarget, "AppTrackingTransparency.framework", true);
            
        }
    }
}