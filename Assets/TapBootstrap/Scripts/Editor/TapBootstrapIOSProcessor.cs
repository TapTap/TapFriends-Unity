using TapCommon.Scripts.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace TapBootstrap.Scripts.Editor
{
    public static class TapBootstrapIOSProcessor
    {
        // 添加标签，unity导出工程后自动执行该函数
        [PostProcessBuildAttribute(100)]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
        {
            if (buildTarget != BuildTarget.iOS) return;
            // 获得工程路径
            var projPath = TapCommonCompile.GetProjPath(path);
            var proj = TapCommonCompile.ParseProjPath(projPath);
            var target = TapCommonCompile.GetUnityTarget(proj);

            if (TapCommonCompile.CheckTarget(target))
            {
                Debug.LogError("Unity-iPhone is NUll");
                return;
            }

            if (TapCommonCompile.HandlerBundle(path,
                Application.dataPath,
                "TapBootstrapResource",
                "com.tapsdk.bootstrap",
                "TapBootstrap",
                new[] {"TapBootstrapResource.bundle"},
                target, projPath, proj))
            {
                Debug.Log("TapBootstrap add Bundle Success!");
                return;
            }

            Debug.LogError("TapBootstrap add Bundle Failed!");
        }
    }
}