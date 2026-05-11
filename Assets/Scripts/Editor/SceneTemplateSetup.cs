using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// <summary>
/// One-time editor tool to create ModeSelect and Settings template scenes.
/// Run via Tools > Setup Template Scenes, then delete this script.
/// </summary>
public class SceneTemplateSetup : Editor
{
    [MenuItem("Tools/Setup Template Scenes")]
    public static void SetupAllScenes()
    {
        SetupModeSelectScene();
        SetupSettingsScene();
        Debug.Log("Template scenes created. Delete Assets/Scripts/Editor/SceneTemplateSetup.cs when done.");
    }

    // ---------------------------------------------------------------
    // MODE SELECT SCENE
    // ---------------------------------------------------------------
    [MenuItem("Tools/Setup ModeSelect Scene")]
    public static void SetupModeSelectScene()
    {
        // Create new scene
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // --- Camera ---
        GameObject camGO = new GameObject("Main Camera");
        Camera cam = camGO.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 1f);
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.nearClipPlane = -1f;
        cam.farClipPlane = 1000f;
        cam.depth = -1f;
        camGO.tag = "MainCamera";
        camGO.AddComponent<AudioListener>();

        // --- Canvas ---
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        // --- EventSystem ---
        GameObject eventGO = new GameObject("EventSystem");
        eventGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        // --- Manager ---
        GameObject managerGO = new GameObject("ModeSelectManager");
        managerGO.AddComponent<ModeSelectManager>();

        // --- Title ---
        CreateText(canvasGO.transform, "TitleText", "SELECT MODE", 72,
            new Vector2(0, 700), new Vector2(800, 100), TextAnchor.MiddleCenter);

        // --- Classic Button ---
        GameObject classicBtn = CreateButton(canvasGO.transform, "ClassicButton", "CLASSIC",
            new Vector2(0, 300), new Vector2(600, 120));
        AddButtonClick(classicBtn, managerGO, "PlayClassic");

        // --- Countdown Button ---
        GameObject countdownBtn = CreateButton(canvasGO.transform, "CountdownButton", "COUNTDOWN",
            new Vector2(0, 100), new Vector2(600, 120));
        AddButtonClick(countdownBtn, managerGO, "PlayCountdown");

        // --- Quiz Button ---
        GameObject quizBtn = CreateButton(canvasGO.transform, "QuizButton", "QUIZ",
            new Vector2(0, -100), new Vector2(600, 120));
        AddButtonClick(quizBtn, managerGO, "PlayQuiz");

        // --- Back Button ---
        GameObject backBtn = CreateButton(canvasGO.transform, "BackButton", "BACK",
            new Vector2(0, -350), new Vector2(400, 100));
        AddButtonClick(backBtn, managerGO, "GoBack");

        // --- Coming Soon Popup ---
        GameObject popup = new GameObject("ComingSoonPopup");
        popup.transform.SetParent(canvasGO.transform, false);
        Image popupBg = popup.AddComponent<Image>();
        popupBg.color = new Color(0f, 0f, 0f, 0.85f);
        RectTransform popupRect = popup.GetComponent<RectTransform>();
        popupRect.anchoredPosition = Vector2.zero;
        popupRect.sizeDelta = new Vector2(700, 400);
        popup.SetActive(false);

        GameObject popupText = CreateText(popup.transform, "ComingSoonText", "Coming Soon!", 60,
            new Vector2(0, 50), new Vector2(600, 100), TextAnchor.MiddleCenter);

        GameObject closeBtn = CreateButton(popup.transform, "CloseButton", "OK",
            new Vector2(0, -100), new Vector2(200, 80));
        AddButtonClick(closeBtn, managerGO, "CloseComingSoon");

        // Wire popup references on ModeSelectManager
        ModeSelectManager modeScript = managerGO.GetComponent<ModeSelectManager>();
        SerializedObject so = new SerializedObject(modeScript);
        so.FindProperty("comingSoonPopup").objectReferenceValue = popup;
        so.FindProperty("comingSoonText").objectReferenceValue = popupText.GetComponent<Text>();
        so.ApplyModifiedProperties();

        // Save
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/ModeSelect.unity");
        Debug.Log("ModeSelect scene created and saved to Assets/Scenes/ModeSelect.unity");
    }

    // ---------------------------------------------------------------
    // SETTINGS SCENE
    // ---------------------------------------------------------------
    [MenuItem("Tools/Setup Settings Scene")]
    public static void SetupSettingsScene()
    {
        // Open existing Settings scene (it's nearly empty)
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Settings.unity", OpenSceneMode.Single);

        // Clear everything except camera if needed
        foreach (GameObject go in scene.GetRootGameObjects())
        {
            if (go.name != "Main Camera")
                Object.DestroyImmediate(go);
        }

        // Make sure camera is set up
        GameObject camGO = GameObject.FindWithTag("MainCamera");
        if (camGO == null)
        {
            camGO = new GameObject("Main Camera");
            Camera cam = camGO.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 1f);
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.nearClipPlane = -1f;
            cam.farClipPlane = 1000f;
            cam.depth = -1f;
            camGO.tag = "MainCamera";
            camGO.AddComponent<AudioListener>();
        }

        // --- Canvas ---
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        // --- EventSystem ---
        GameObject eventGO = new GameObject("EventSystem");
        eventGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        // --- Manager ---
        GameObject managerGO = new GameObject("SettingsManager");
        managerGO.AddComponent<SettingsManager>();

        // --- Title ---
        CreateText(canvasGO.transform, "TitleText", "SETTINGS", 72,
            new Vector2(0, 750), new Vector2(700, 100), TextAnchor.MiddleCenter);

        // --- Mute Button ---
        GameObject muteBtn = CreateButton(canvasGO.transform, "MuteButton", "Mute Sound",
            new Vector2(0, 500), new Vector2(500, 100));
        Text muteBtnText = muteBtn.GetComponentInChildren<Text>();
        AddButtonClick(muteBtn, managerGO, "ToggleMute");

        // --- Music Volume Label + Slider ---
        CreateText(canvasGO.transform, "MusicVolumeLabel", "Music Volume", 40,
            new Vector2(0, 350), new Vector2(600, 60), TextAnchor.MiddleCenter);
        GameObject musicSliderGO = new GameObject("MusicVolumeSlider");
        musicSliderGO.transform.SetParent(canvasGO.transform, false);
        Slider musicSlider = musicSliderGO.AddComponent<Slider>();
        musicSlider.minValue = 0f;
        musicSlider.maxValue = 1f;
        musicSlider.value = 0.6f;
        RectTransform msRect = musicSliderGO.GetComponent<RectTransform>();
        msRect.anchoredPosition = new Vector2(0, 270);
        msRect.sizeDelta = new Vector2(600, 60);

        // --- SFX Volume Label + Slider ---
        CreateText(canvasGO.transform, "SFXVolumeLabel", "SFX Volume", 40,
            new Vector2(0, 150), new Vector2(600, 60), TextAnchor.MiddleCenter);
        GameObject sfxSliderGO = new GameObject("SFXVolumeSlider");
        sfxSliderGO.transform.SetParent(canvasGO.transform, false);
        Slider sfxSlider = sfxSliderGO.AddComponent<Slider>();
        sfxSlider.minValue = 0f;
        sfxSlider.maxValue = 1f;
        sfxSlider.value = 0.8f;
        RectTransform ssRect = sfxSliderGO.GetComponent<RectTransform>();
        ssRect.anchoredPosition = new Vector2(0, 70);
        ssRect.sizeDelta = new Vector2(600, 60);

        // --- Graphics Dropdown ---
        CreateText(canvasGO.transform, "GraphicsLabel", "Graphics Quality", 40,
            new Vector2(0, -70), new Vector2(600, 60), TextAnchor.MiddleCenter);
        GameObject dropdownGO = new GameObject("GraphicsDropdown");
        dropdownGO.transform.SetParent(canvasGO.transform, false);
        Dropdown dropdown = dropdownGO.AddComponent<Dropdown>();
        dropdown.options.Clear();
        dropdown.options.Add(new Dropdown.OptionData("Standard"));
        dropdown.options.Add(new Dropdown.OptionData("Low Performance"));
        RectTransform ddRect = dropdownGO.GetComponent<RectTransform>();
        ddRect.anchoredPosition = new Vector2(0, -160);
        ddRect.sizeDelta = new Vector2(500, 80);

        // --- Attribution Text ---
        GameObject attrGO = CreateText(canvasGO.transform, "AttributionText",
            "Music by Kevin MacLeod (incompetech.com)\nLicensed CC-BY 4.0\nSFX from Kenney.nl (CC0)",
            28, new Vector2(0, -350), new Vector2(800, 120), TextAnchor.MiddleCenter);

        // --- Home Button ---
        GameObject homeBtn = CreateButton(canvasGO.transform, "HomeButton", "HOME",
            new Vector2(0, -580), new Vector2(400, 100));
        AddButtonClick(homeBtn, managerGO, "GoHome");

        // Wire SettingsManager serialized fields
        SettingsManager settingsScript = managerGO.GetComponent<SettingsManager>();
        SerializedObject so = new SerializedObject(settingsScript);
        so.FindProperty("musicVolumeSlider").objectReferenceValue = musicSlider;
        so.FindProperty("sfxVolumeSlider").objectReferenceValue = sfxSlider;
        so.FindProperty("muteButtonText").objectReferenceValue = muteBtnText;
        so.FindProperty("graphicsDropdown").objectReferenceValue = dropdown;
        so.FindProperty("attributionText").objectReferenceValue = attrGO.GetComponent<Text>();
        so.ApplyModifiedProperties();

        EditorSceneManager.SaveScene(scene);
        Debug.Log("Settings scene built and saved.");
    }

    // ---------------------------------------------------------------
    // HELPERS
    // ---------------------------------------------------------------
    static GameObject CreateText(Transform parent, string name, string content, int fontSize,
        Vector2 pos, Vector2 size, TextAnchor anchor)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Text t = go.AddComponent<Text>();
        t.text = content;
        t.fontSize = fontSize;
        t.alignment = anchor;
        t.color = Color.white;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return go;
    }

    static GameObject CreateButton(Transform parent, string name, string label,
        Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image img = go.AddComponent<Image>();
        img.color = new Color(0.2f, 0.6f, 0.3f, 1f);
        Button btn = go.AddComponent<Button>();
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(go.transform, false);
        Text t = textGO.AddComponent<Text>();
        t.text = label;
        t.fontSize = 48;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = Color.white;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        RectTransform trt = textGO.GetComponent<RectTransform>();
        trt.anchoredPosition = Vector2.zero;
        trt.sizeDelta = size;

        return go;
    }

    static void AddButtonClick(GameObject btnGO, GameObject target, string methodName)
    {
        Button btn = btnGO.GetComponent<Button>();
        if (btn == null) return;
        UnityEditor.Events.UnityEventTools.AddStringPersistentListener(
            btn.onClick,
            target.SendMessage,
            methodName
        );
    }
}
