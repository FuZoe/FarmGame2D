using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class GameSystemsBootstrap : MonoBehaviour
{
    private static bool created;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateSystems()
    {
        if (created) return;
        created = true;
        GameObject root = new GameObject("GameSystems");
        DontDestroyOnLoad(root);
        root.AddComponent<GameClock>();
        root.AddComponent<StaminaSystem>();
        root.AddComponent<WalletSystem>();
        root.AddComponent<FarmSystem>();
        root.AddComponent<ShopSystem>();
        root.AddComponent<QuestSystem>();
        root.AddComponent<CookingSystem>();
        root.AddComponent<NpcSystem>();
        root.AddComponent<SaveSystem>();
        root.AddComponent<SettingsSystem>();
        root.AddComponent<AudioManager>();
        root.AddComponent<SceneTransitionManager>();
        root.AddComponent<GameSystemsBootstrap>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureInventoryManager();
        GameObject oldWorld = GameObject.Find("GameWorld");
        if (oldWorld != null) Destroy(oldWorld);
        GameObject world = new GameObject("GameWorld");
        world.AddComponent<WorldGameplayController>();
        world.AddComponent<GameHUD>();
    }

    private static void EnsureInventoryManager()
    {
        if (InventoryManager.Instance != null) return;
        InventoryManager existing = Object.FindAnyObjectByType<InventoryManager>();
        if (existing == null) existing = new GameObject("InventoryManager").AddComponent<InventoryManager>();
    }
}

public sealed class WorldGameplayController : MonoBehaviour
{
    private readonly Dictionary<string, GameObject> plotViews = new Dictionary<string, GameObject>();
    private readonly List<GameObject> npcViews = new List<GameObject>();
    private Player player;
    private ToolKind selectedTool = ToolKind.Hoe;
    private static Sprite squareSprite;

    private void Start()
    {
        player = Object.FindAnyObjectByType<Player>();
        if (player != null) FarmSystem.Instance?.EnsureDefaultPlots(player.transform.position);
        if (FarmSystem.Instance != null) FarmSystem.Instance.PlotChanged += RefreshPlot;
        RenderPlots();
        SpawnNpcs();
    }

    private void OnDestroy()
    {
        if (FarmSystem.Instance != null) FarmSystem.Instance.PlotChanged -= RefreshPlot;
    }

    private void Update()
    {
        if (player == null) player = Object.FindAnyObjectByType<Player>();
        if (Input.GetKeyDown(KeyCode.Alpha1)) selectedTool = ToolKind.Hoe;
        if (Input.GetKeyDown(KeyCode.Alpha2)) selectedTool = ToolKind.WateringCan;
        if (Input.GetKeyDown(KeyCode.Alpha3)) selectedTool = ToolKind.SeedBag;
        if (Input.GetKeyDown(KeyCode.Alpha4)) selectedTool = ToolKind.Basket;
        if (Input.GetKeyDown(KeyCode.E)) UseSelectedTool();
        if (Input.GetKeyDown(KeyCode.Z)) Plant(CropKind.Carrot);
        if (Input.GetKeyDown(KeyCode.X)) Plant(CropKind.Tomato);
        if (Input.GetKeyDown(KeyCode.N)) InteractNpc();
        if (Input.GetKeyDown(KeyCode.B)) ShopSystem.Instance?.Buy(ItemType.seed_carrot);
        if (Input.GetKeyDown(KeyCode.V)) ShopSystem.Instance?.Sell(ItemType.seed_carrot);
        if (Input.GetKeyDown(KeyCode.K)) CookingSystem.Instance?.Cook("Carrot Soup");
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (QuestSystem.Instance != null && !QuestSystem.Instance.Accepted) QuestSystem.Instance.Accept();
            else QuestSystem.Instance?.ClaimReward();
        }
        if (Input.GetKeyDown(KeyCode.Space)) GameClock.Instance?.Sleep();
        if (Input.GetKeyDown(KeyCode.F5)) SaveSystem.Instance?.Save();
        if (Input.GetKeyDown(KeyCode.F9)) SaveSystem.Instance?.Load();
        if (Input.GetKeyDown(KeyCode.F10)) SettingsSystem.Instance?.ToggleFullscreen();
        if (Input.GetKeyDown(KeyCode.PageUp)) SettingsSystem.Instance?.SetMasterVolume((SettingsSystem.Instance?.MasterVolume ?? 1f) + 0.1f);
        if (Input.GetKeyDown(KeyCode.PageDown)) SettingsSystem.Instance?.SetMasterVolume((SettingsSystem.Instance?.MasterVolume ?? 1f) - 0.1f);
        if (Input.GetKeyDown(KeyCode.F1)) SceneTransitionManager.Instance?.LoadSceneByIndex(0);
        if (Input.GetKeyDown(KeyCode.F2)) SceneTransitionManager.Instance?.LoadSceneByIndex(1);
        if (Input.GetKeyDown(KeyCode.F3)) SceneTransitionManager.Instance?.LoadSceneByIndex(2);
        if (Input.GetKeyDown(KeyCode.F4)) SceneTransitionManager.Instance?.LoadSceneByIndex(3);
    }

    private FarmPlotState NearestPlot()
    {
        if (player == null || FarmSystem.Instance == null) return null;
        FarmPlotState closest = null;
        float distance = 2.1f;
        foreach (FarmPlotState plot in FarmSystem.Instance.Plots)
        {
            float current = Vector2.Distance(player.transform.position, new Vector2(plot.x, plot.y));
            if (current < distance) { distance = current; closest = plot; }
        }
        return closest;
    }

    private void UseSelectedTool()
    {
        FarmPlotState plot = NearestPlot();
        if (plot == null) return;
        Vector2Int position = new Vector2Int(plot.x, plot.y);
        if (selectedTool == ToolKind.Hoe) FarmSystem.Instance.Till(position);
        if (selectedTool == ToolKind.WateringCan) FarmSystem.Instance.Water(position);
        if (selectedTool == ToolKind.Basket) FarmSystem.Instance.Harvest(position);
    }

    private void Plant(CropKind crop)
    {
        if (selectedTool != ToolKind.SeedBag || player == null) return;
        FarmPlotState plot = NearestPlot();
        if (plot != null) FarmSystem.Instance.Plant(new Vector2Int(plot.x, plot.y), crop);
    }

    private void InteractNpc()
    {
        if (player == null || npcViews.Count == 0) return;
        GameObject closest = null;
        float distance = 2.5f;
        foreach (GameObject npc in npcViews)
        {
            float current = Vector2.Distance(player.transform.position, npc.transform.position);
            if (current < distance) { distance = current; closest = npc; }
        }
        if (closest == null) return;
        string id = closest.name;
        string message = NpcSystem.Instance != null ? NpcSystem.Instance.Interact(id) : string.Empty;
        GameHUD.ShowMessage(message);
    }

    private void RenderPlots()
    {
        if (FarmSystem.Instance == null) return;
        foreach (FarmPlotState plot in FarmSystem.Instance.Plots) RefreshPlot(plot);
    }

    private void RefreshPlot(FarmPlotState plot)
    {
        string key = plot.x + ":" + plot.y;
        if (!plotViews.TryGetValue(key, out GameObject view))
        {
            view = new GameObject("Plot_" + key);
            view.transform.SetParent(transform);
            view.transform.position = new Vector3(plot.x, plot.y, 0f);
            SpriteRenderer renderer = view.AddComponent<SpriteRenderer>();
            renderer.sprite = GetSquareSprite();
            renderer.sortingOrder = 20;
            plotViews[key] = view;
        }
        SpriteRenderer spriteRenderer = view.GetComponent<SpriteRenderer>();
        spriteRenderer.color = plot.ready ? new Color(0.95f, 0.75f, 0.2f) :
            plot.planted ? new Color(0.3f, 0.75f, 0.25f) :
            plot.watered ? new Color(0.25f, 0.5f, 0.85f) :
            plot.tilled ? new Color(0.45f, 0.25f, 0.12f) : new Color(0.3f, 0.55f, 0.2f);
    }

    private void SpawnNpcs()
    {
        if (player == null) return;
        CreateNpc("Mayor", player.transform.position + new Vector3(4f, 1f, 0f), new Color(0.85f, 0.35f, 0.35f));
        CreateNpc("Shopkeeper", player.transform.position + new Vector3(5f, -1f, 0f), new Color(0.35f, 0.65f, 0.9f));
        CreateNpc("Cook", player.transform.position + new Vector3(4f, -3f, 0f), new Color(0.9f, 0.55f, 0.25f));
    }

    private void CreateNpc(string id, Vector3 position, Color color)
    {
        GameObject npc = new GameObject(id);
        npc.transform.SetParent(transform);
        npc.transform.position = position;
        SpriteRenderer renderer = npc.AddComponent<SpriteRenderer>();
        renderer.sprite = GetSquareSprite();
        renderer.color = color;
        renderer.sortingOrder = 25;
        npcViews.Add(npc);
    }

    private static Sprite GetSquareSprite()
    {
        if (squareSprite != null) return squareSprite;
        Texture2D texture = new Texture2D(1, 1) { filterMode = FilterMode.Point };
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        squareSprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        return squareSprite;
    }
}

public sealed class GameHUD : MonoBehaviour
{
    private static GameHUD instance;
    private Text status;
    private Text message;
    private float messageUntil;

    private void Awake()
    {
        instance = this;
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        gameObject.AddComponent<CanvasScaler>();
        gameObject.AddComponent<GraphicRaycaster>();
        status = CreateText(canvas.transform, new Vector2(16f, -16f), 380f, 110f, 15);
        message = CreateText(canvas.transform, new Vector2(16f, -132f), 650f, 45f, 16);
        message.color = new Color(1f, 0.9f, 0.45f);
    }

    private void Update()
    {
        if (status == null) return;
        string time = GameClock.Instance != null ? GameClock.Instance.DisplayTime : "Day 1 06:00";
        string stamina = StaminaSystem.Instance != null ? $"Stamina {StaminaSystem.Instance.CurrentStamina}/{StaminaSystem.Instance.MaxStamina}" : string.Empty;
        string coins = WalletSystem.Instance != null ? $"Coins {WalletSystem.Instance.Coins}" : string.Empty;
        status.text = $"{time}   {stamina}   {coins}\n" +
            "1 Hoe  2 Water  3 Seeds  4 Basket  E Use  Z/X Plant\n" +
            "N Talk  B Buy  V Sell  K Cook  Q Quest  Space Sleep\n" +
            "F5 Save  F9 Load  F1-F4 Scenes  F10 Fullscreen";
        if (message != null && Time.unscaledTime > messageUntil) message.text = string.Empty;
    }

    public static void ShowMessage(string text, float duration = 4f)
    {
        if (instance == null || instance.message == null) return;
        instance.message.text = text;
        instance.messageUntil = Time.unscaledTime + duration;
    }

    private static Text CreateText(Transform parent, Vector2 position, float width, float height, int fontSize)
    {
        GameObject textObject = new GameObject("HUDText");
        textObject.transform.SetParent(parent, false);
        RectTransform rect = textObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(width, height);
        Text text = textObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = fontSize;
        text.color = Color.white;
        text.alignment = TextAnchor.UpperLeft;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        return text;
    }
}
