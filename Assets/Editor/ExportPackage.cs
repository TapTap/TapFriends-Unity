using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class ExportPackage : UnityEditor.Editor
    {
        static void ExportUnityPackage(string moduleName)
        {
            var exportPath = Directory.GetParent(Application.dataPath).FullName + "/TapSDK-UnityPackage";

            if (!Directory.Exists(exportPath))
            {
                Directory.CreateDirectory(exportPath);
            }

            var path = exportPath + $"/TapTap_{moduleName}.unitypackage";

            string[] resPaths = {$"Assets/TapTap/{moduleName}"};

            var assetsPathNames = AssetDatabase.GetDependencies(resPaths);

            AssetDatabase.ExportPackage(assetsPathNames, path, ExportPackageOptions.Recurse);
        }

        static void PushUnityPackage()
        {
            string[] moduleNames = {"Common", "Bootstrap", "Moment", "TapDB", "Login"};

            foreach (var module in moduleNames)
            {
                ExportUnityPackage(module);
            }
        }

        static string[] GetBuildScenes()
        {
            List<string> names = new List<string>();
            for (var index = 0; index < EditorBuildSettings.scenes.Length; index++)
            {
                var e = EditorBuildSettings.scenes[index];
                if (e == null)
                    continue;
                if (e.enabled)
                    names.Add(e.path);
            }

            return names.ToArray();
        }

        static void UpdateSetting(string key, string environmentKey, string defaultMacOSPath)
        {
            var defaultPath = defaultMacOSPath;
            if (string.IsNullOrEmpty(defaultPath) || !Directory.Exists(defaultPath))
                throw new DirectoryNotFoundException($"{key} {environmentKey} {defaultPath}");
            EditorPrefs.SetString(key, defaultPath);
        }

        static void PackageAndroid()
        {
            // 签名文件配置，若不配置，则使用Unity默认签名
            PlayerSettings.Android.keyaliasName = "wxlogin";
            PlayerSettings.Android.keyaliasPass = "111111";
            PlayerSettings.Android.keystoreName =
                Application.dataPath.Replace("/Assets", "") + "/sign_password_111111.keystore";
            PlayerSettings.Android.keystorePass = "111111";
            
            var exportPath = "";
            var unityVersion = "";
            foreach (var arg in System.Environment.GetCommandLineArgs())
            {
                Debug.Log("args:" + arg);
                if (arg.StartsWith("-EXPORT_PATH"))
                {
                    exportPath = arg.Split('=')[1].Trim('"');
                    Debug.Log("ExportPath:" + exportPath);
                }
                else if (arg.StartsWith("-UNITY_VERSION"))
                {
                    unityVersion = arg.Split('=')[1].Trim('"');
                }
            }
            
            UpdateSetting("AndroidSdkRoot", "ANDROID_SDK",
                "/Applications/Unity/Hub/Editor/" + unityVersion + "/PlaybackEngines/AndroidPlayer/SDK");
            UpdateSetting("AndroidNdkRoot", "ANDROID_NDK",
                "/Applications/Unity/Hub/Editor/" + unityVersion + "/PlaybackEngines/AndroidPlayer/NDK");

            var path = (exportPath + "/" + "TapSDK2-Unity"  + ".apk").Replace("//", "/");

            try
            {
                BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.Android, BuildOptions.None);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        static void PackageIOS()
        {
            var path = Application.dataPath.Replace("/Assets", "") + "/TapSDK2-Unity";

            AssetDatabase.Refresh();
            try
            {
                BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.iOS, BuildOptions.None);
            }
            catch (System.Exception m)
            {
                Debug.LogError(m.Message);
            }
        }
    }
}