using System.IO;
using UnityEditor.Build;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class ReleaseBuild
{
    // Keep release identity and output paths centralized for local and CI builds.
    private const string AndroidApplicationId = "com.g2.choppingblock";
    private const string IosApplicationId = "com.g2.choppingblock";
    private const string BundleVersion = "1.0.0";
    private const int AndroidVersionCode = 1;
    private const string IosBuildNumber = "1";

    private static readonly string[] BetaScenePaths =
    {
        "Assets/Scenes/MainMenu.unity",
        "Assets/Scenes/Fun Facts.unity",
        "Assets/Scenes/Gameplay.unity",
        "Assets/Scenes/GameOver.unity",
        "Assets/Scenes/Settings.unity",
        "Assets/Scenes/TrophyCase.unity",
        "Assets/Scenes/BackgroundSelect.unity",
        "Assets/Scenes/ClassicTutorial.unity",
    };

    [MenuItem("Build/Release/Validate Beta Configuration")]
    public static void ValidateBetaConfiguration()
    {
        ConfigureReleasePlayerSettings();
        EnsureBetaBuildScenes();
        ValidateBetaScenes();
        ValidateInstalledSupport();
        Debug.Log("ReleaseBuild: beta configuration validated.");
    }

    [MenuItem("Build/Release/Build Android APK")]
    public static void BuildAndroidApk()
    {
        BuildAndroid(appBundle: false);
    }

    [MenuItem("Build/Release/Build Android AAB")]
    public static void BuildAndroidAab()
    {
        BuildAndroid(appBundle: true);
    }

    [MenuItem("Build/Release/Export iOS Xcode Project")]
    public static void ExportIosXcodeProject()
    {
        ConfigureReleasePlayerSettings();
        EnsureBetaBuildScenes();
        ValidateBetaScenes();

        if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.iOS, BuildTarget.iOS))
        {
            throw new BuildFailedException(
                "iOS build support is not installed in this Unity editor. Install the iOS Build Support module on a Mac before exporting.");
        }

        string outputPath = Path.Combine("Builds", "iOS");
        EnsureDirectory(outputPath);

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = BetaScenePaths,
            locationPathName = outputPath,
            targetGroup = BuildTargetGroup.iOS,
            target = BuildTarget.iOS,
            options = BuildOptions.None,
        };

        RunBuild(options, "iOS Xcode export");
    }

    public static void ValidateBetaConfigurationBatch()
    {
        ValidateBetaConfiguration();
    }

    public static void BuildAndroidApkBatch()
    {
        BuildAndroidApk();
    }

    public static void BuildAndroidAabBatch()
    {
        BuildAndroidAab();
    }

    public static void ExportIosXcodeProjectBatch()
    {
        ExportIosXcodeProject();
    }

    private static void BuildAndroid(bool appBundle)
    {
        ConfigureReleasePlayerSettings();
        EnsureBetaBuildScenes();
        ValidateBetaScenes();

        if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android, BuildTarget.Android))
        {
            throw new BuildFailedException("Android build support is not installed in this Unity editor.");
        }

        string outputDirectory = Path.Combine("Builds", "Android");
        EnsureDirectory(outputDirectory);

        EditorUserBuildSettings.buildAppBundle = appBundle;

        string outputPath = Path.Combine(
            outputDirectory,
            appBundle ? "ChoppingBlock-beta-release.aab" : "ChoppingBlock-beta-release.apk");

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = BetaScenePaths,
            locationPathName = outputPath,
            targetGroup = BuildTargetGroup.Android,
            target = BuildTarget.Android,
            options = BuildOptions.None,
        };

        RunBuild(options, appBundle ? "Android AAB" : "Android APK");
    }

    private static void ConfigureReleasePlayerSettings()
    {
        PlayerSettings.productName = "Chopping Block";
        PlayerSettings.companyName = "G2";
        PlayerSettings.bundleVersion = BundleVersion;
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, AndroidApplicationId);
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, IosApplicationId);
        PlayerSettings.Android.bundleVersionCode = AndroidVersionCode;
        PlayerSettings.iOS.buildNumber = IosBuildNumber;

        // Android: IL2CPP + ARM64
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel25;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;

        // iOS: ARM64, target iPhone 15 Pro Max and modern devices
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.iOS, ScriptingImplementation.IL2CPP);
        PlayerSettings.iOS.targetOSVersionString = "16.0";
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
        PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneOnly;
    }

    private static void EnsureBetaBuildScenes()
    {
        var scenes = new EditorBuildSettingsScene[BetaScenePaths.Length];
        for (int i = 0; i < BetaScenePaths.Length; i++)
        {
            scenes[i] = new EditorBuildSettingsScene(BetaScenePaths[i], true);
        }

        EditorBuildSettings.scenes = scenes;
    }

    private static void ValidateBetaScenes()
    {
        foreach (string scenePath in BetaScenePaths)
        {
            if (!File.Exists(scenePath))
            {
                throw new BuildFailedException("Missing required beta scene: " + scenePath);
            }
        }
    }

    private static void ValidateInstalledSupport()
    {
        string unityRoot = EditorApplication.applicationContentsPath;
        string androidSupport = Path.Combine(unityRoot, "PlaybackEngines", "AndroidPlayer");
        string iosSupport = Path.Combine(unityRoot, "PlaybackEngines", "iOSSupport");

        if (!Directory.Exists(androidSupport))
        {
            throw new BuildFailedException("Android build support is missing from the current Unity installation.");
        }

        if (!Directory.Exists(iosSupport))
        {
            Debug.LogWarning("ReleaseBuild: iOS build support is not installed in this Unity editor. Android builds can proceed, but iOS export must run on a Mac with the iOS module.");
        }
    }

    private static void EnsureDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    private static void RunBuild(BuildPlayerOptions options, string label)
    {
        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new BuildFailedException(
                $"ReleaseBuild: {label} failed with result {report.summary.result}. See the Unity Editor log for details.");
        }

        Debug.Log($"ReleaseBuild: {label} completed at {report.summary.outputPath}");
    }
}
