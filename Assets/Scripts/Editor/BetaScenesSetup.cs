using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// One-time editor tool to create Account and BackgroundSelect scenes.
/// Run via Tools > Setup Beta Scenes, then delete this script.
/// </summary>
public class BetaScenesSetup : Editor
{
    [MenuItem("Tools/Setup Beta Scenes")]
    public static void SetupAll()
    {
        SetupAccountScene();
        SetupBackgroundSelectScene();
        Debug.Log("Beta scenes created. Delete Assets/Scripts/Editor/BetaScenesSetup.cs when done.");
    }

    // ---------------------------------------------------------------
    // ACCOUNT / PROFILE SCENE
    // ---------------------------------------------------------------
    [MenuItem("Tools/Setup Account Scene")]
    public static void SetupAccountScene()
    {
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
        GameObject canvasGO = MakeCanvas();

        // --- EventSystem ---
        MakeEventSystem();

        // --- Manager ---
        GameObject managerGO = new GameObject("AccountManager");
        managerGO.AddComponent<AccountManager>();

        // --- Title ---
        CreateText(canvasGO.transform, "TitleText", "PROFILE", 72,
            new Vector2(0, 750), new Vector2(700, 100), TextAnchor.MiddleCenter);

        // --- Name label + InputField ---
        CreateText(canvasGO.transform, "NameLabel", "Your Name", 40,
            new Vector2(0, 550), new Vector2(600, 60), TextAnchor.MiddleCenter);

        GameObject nameFieldGO = new GameObject("NameInputField");
        nameFieldGO.transform.SetParent(canvasGO.transform, false);
        Image nameBg = nameFieldGO.AddComponent<Image>();
        nameBg.color = new Color(0.25f, 0.25f, 0.25f, 1f);
        InputField nameInput = nameFieldGO.AddComponent<InputField>();
        nameInput.characterLimit = 20;
        RectTransform nfRect = nameFieldGO.GetComponent<RectTransform>();
        nfRect.anchoredPosition = new Vector2(0, 460);
        nfRect.sizeDelta = new Vector2(600, 70);

        // InputField needs a Text child
        GameObject nameTextGO = new GameObject("Text");
        nameTextGO.transform.SetParent(nameFieldGO.transform, false);
        Text nameText = nameTextGO.AddComponent<Text>();
        nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        nameText.fontSize = 40;
        nameText.color = Color.white;
        nameText.alignment = TextAnchor.MiddleLeft;
        RectTransform ntRect = nameTextGO.GetComponent<RectTransform>();
        ntRect.anchoredPosition = new Vector2(10, 0);
        ntRect.sizeDelta = new Vector2(-20, 0);
        ntRect.anchorMin = Vector2.zero;
        ntRect.anchorMax = Vector2.one;
        nameInput.textComponent = nameText;

        // Placeholder text
        GameObject placeholderGO = new GameObject("Placeholder");
        placeholderGO.transform.SetParent(nameFieldGO.transform, false);
        Text placeholderText = placeholderGO.AddComponent<Text>();
        placeholderText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        placeholderText.fontSize = 40;
        placeholderText.color = new Color(0.6f, 0.6f, 0.6f, 1f);
        placeholderText.fontStyle = FontStyle.Italic;
        placeholderText.text = "Enter name...";
        placeholderText.alignment = TextAnchor.MiddleLeft;
        RectTransform phRect = placeholderGO.GetComponent<RectTransform>();
        phRect.anchoredPosition = new Vector2(10, 0);
        phRect.sizeDelta = new Vector2(-20, 0);
        phRect.anchorMin = Vector2.zero;
        phRect.anchorMax = Vector2.one;
        nameInput.placeholder = placeholderText;

        // --- Age Range label + Dropdown ---
        CreateText(canvasGO.transform, "AgeLabel", "Age", 40,
            new Vector2(0, 340), new Vector2(600, 60), TextAnchor.MiddleCenter);

        GameObject dropdownGO = new GameObject("AgeRangeDropdown");
        dropdownGO.transform.SetParent(canvasGO.transform, false);
        Image ddBg = dropdownGO.AddComponent<Image>();
        ddBg.color = new Color(0.2f, 0.6f, 0.3f, 1f);
        Dropdown ageDropdown = dropdownGO.AddComponent<Dropdown>();
        ageDropdown.options.Clear();
        ageDropdown.options.Add(new Dropdown.OptionData("8 - 9"));
        ageDropdown.options.Add(new Dropdown.OptionData("10 - 11"));
        ageDropdown.options.Add(new Dropdown.OptionData("12 +"));
        RectTransform ddRect = dropdownGO.GetComponent<RectTransform>();
        ddRect.anchoredPosition = new Vector2(0, 250);
        ddRect.sizeDelta = new Vector2(400, 80);

        // --- Best Score display ---
        GameObject pointsGO = CreateText(canvasGO.transform, "PointsText", "Best Score: 0", 44,
            new Vector2(0, 100), new Vector2(600, 70), TextAnchor.MiddleCenter);

        // --- Feedback text ---
        GameObject feedbackGO = CreateText(canvasGO.transform, "FeedbackText", "Saved!", 36,
            new Vector2(0, -50), new Vector2(500, 60), TextAnchor.MiddleCenter);
        feedbackGO.SetActive(false);

        // --- Save Button ---
        GameObject saveBtn = CreateButton(canvasGO.transform, "SaveButton", "SAVE",
            new Vector2(0, -220), new Vector2(500, 110));
        AddButtonClick(saveBtn, managerGO, "SaveProfile");

        // --- Back Button ---
        GameObject backBtn = CreateButton(canvasGO.transform, "BackButton", "BACK",
            new Vector2(0, -380), new Vector2(400, 100));
        AddButtonClick(backBtn, managerGO, "GoBack");

        // Wire AccountManager serialized fields
        AccountManager accountScript = managerGO.GetComponent<AccountManager>();
        SerializedObject so = new SerializedObject(accountScript);
        so.FindProperty("nameInputField").objectReferenceValue = nameInput;
        so.FindProperty("ageRangeDropdown").objectReferenceValue = ageDropdown;
        so.FindProperty("pointsText").objectReferenceValue = pointsGO.GetComponent<Text>();
        so.FindProperty("feedbackText").objectReferenceValue = feedbackGO.GetComponent<Text>();
        so.ApplyModifiedProperties();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Account.unity");
        Debug.Log("Account scene created and saved to Assets/Scenes/Account.unity");
    }

    // ---------------------------------------------------------------
    // BACKGROUND SELECT SCENE
    // ---------------------------------------------------------------
    [MenuItem("Tools/Setup BackgroundSelect Scene")]
    public static void SetupBackgroundSelectScene()
    {
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
        GameObject canvasGO = MakeCanvas();

        // --- EventSystem ---
        MakeEventSystem();

        // --- Manager ---
        GameObject managerGO = new GameObject("BackgroundSelectManager");
        managerGO.AddComponent<BackgroundSelectManager>();

        // --- Title ---
        CreateText(canvasGO.transform, "TitleText", "BACKGROUNDS", 72,
            new Vector2(0, 750), new Vector2(900, 100), TextAnchor.MiddleCenter);

        // --- Preview Image ---
        GameObject previewGO = new GameObject("PreviewImage");
        previewGO.transform.SetParent(canvasGO.transform, false);
        Image previewImg = previewGO.AddComponent<Image>();
        previewImg.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        RectTransform previewRect = previewGO.GetComponent<RectTransform>();
        previewRect.anchoredPosition = new Vector2(0, 300);
        previewRect.sizeDelta = new Vector2(700, 500);

        // --- Background Name ---
        GameObject bgNameGO = CreateText(canvasGO.transform, "BackgroundNameText", "Default", 44,
            new Vector2(0, 0), new Vector2(700, 70), TextAnchor.MiddleCenter);

        // --- Page Indicator ---
        GameObject pageGO = CreateText(canvasGO.transform, "PageIndicatorText", "1 / 1", 36,
            new Vector2(0, -80), new Vector2(300, 60), TextAnchor.MiddleCenter);

        // --- Left Arrow ---
        GameObject leftBtn = CreateButton(canvasGO.transform, "LeftArrowButton", "<",
            new Vector2(-380, 300), new Vector2(120, 120));
        AddButtonClick(leftBtn, managerGO, "NavigateLeft");

        // --- Right Arrow ---
        GameObject rightBtn = CreateButton(canvasGO.transform, "RightArrowButton", ">",
            new Vector2(380, 300), new Vector2(120, 120));
        AddButtonClick(rightBtn, managerGO, "NavigateRight");

        // --- Feedback text ---
        GameObject feedbackGO = CreateText(canvasGO.transform, "FeedbackText", "Background selected!", 36,
            new Vector2(0, -180), new Vector2(600, 60), TextAnchor.MiddleCenter);
        feedbackGO.SetActive(false);

        // --- Select Button ---
        GameObject selectBtn = CreateButton(canvasGO.transform, "SelectButton", "SELECT",
            new Vector2(0, -320), new Vector2(500, 110));
        AddButtonClick(selectBtn, managerGO, "SelectBackground");

        // --- Back Button ---
        GameObject backBtn = CreateButton(canvasGO.transform, "BackButton", "BACK",
            new Vector2(0, -480), new Vector2(400, 100));
        AddButtonClick(backBtn, managerGO, "GoBack");

        // Wire BackgroundSelectManager serialized fields
        BackgroundSelectManager bgScript = managerGO.GetComponent<BackgroundSelectManager>();
        SerializedObject so = new SerializedObject(bgScript);
        so.FindProperty("previewImage").objectReferenceValue = previewImg;
        so.FindProperty("backgroundNameText").objectReferenceValue = bgNameGO.GetComponent<Text>();
        so.FindProperty("pageIndicatorText").objectReferenceValue = pageGO.GetComponent<Text>();
        so.FindProperty("leftArrowButton").objectReferenceValue = leftBtn.GetComponent<Button>();
        so.FindProperty("rightArrowButton").objectReferenceValue = rightBtn.GetComponent<Button>();
        so.FindProperty("feedbackText").objectReferenceValue = feedbackGO.GetComponent<Text>();
        so.ApplyModifiedProperties();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/BackgroundSelect.unity");
        Debug.Log("BackgroundSelect scene created and saved to Assets/Scenes/BackgroundSelect.unity");
    }

    // ---------------------------------------------------------------
    // HELPERS
    // ---------------------------------------------------------------
    static GameObject MakeCanvas()
    {
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();
        return canvasGO;
    }

    static void MakeEventSystem()
    {
        GameObject eventGO = new GameObject("EventSystem");
        eventGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
    }

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
