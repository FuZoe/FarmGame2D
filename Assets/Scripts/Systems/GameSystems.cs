using System;
using System.Collections.Generic;
using UnityEngine;

public enum ToolKind
{
    Hoe,
    WateringCan,
    SeedBag,
    Basket
}

public enum CropKind
{
    Carrot,
    Tomato
}

public class CookingRecipe
{
    public ItemType ingredient;
    public int reward;

    public CookingRecipe(ItemType ingredient, int reward)
    {
        this.ingredient = ingredient;
        this.reward = reward;
    }
}

[Serializable]
public class FarmPlotState
{
    public int x;
    public int y;
    public bool tilled;
    public bool watered;
    public CropKind crop;
    public int growthDays;
    public bool planted;
    public bool ready;

    public FarmPlotState() { }

    public FarmPlotState(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public static class InventoryService
{
    public static bool IsReady => InventoryManager.Instance != null && InventoryManager.Instance.Seedbackpack != null;

    public static int Count(ItemType type)
    {
        if (!IsReady) return 0;
        int total = 0;
        foreach (SlotData slot in InventoryManager.Instance.Seedbackpack.slotList)
        {
            if (slot.item != null && slot.item.type == type) total += slot.count;
        }
        return total;
    }

    public static bool TryConsume(ItemType type, int amount = 1)
    {
        if (amount <= 0 || Count(type) < amount) return false;
        int remaining = amount;
        foreach (SlotData slot in InventoryManager.Instance.Seedbackpack.slotList)
        {
            if (slot.item == null || slot.item.type != type) continue;
            ItemData item = slot.item;
            int removed = Mathf.Min(slot.count, remaining);
            int kept = slot.count - removed;
            remaining -= removed;
            slot.Clear();
            if (kept > 0)
            {
                slot.AddItem(item);
                for (int i = 1; i < kept; i++) slot.AddOne();
            }
            if (remaining == 0) return true;
        }
        return false;
    }

    public static void Add(ItemType type, int amount = 1)
    {
        if (IsReady && amount > 0) InventoryManager.Instance.AddToSeedBackpack(type);
        for (int i = 1; IsReady && i < amount; i++) InventoryManager.Instance.AddToSeedBackpack(type);
    }
}

public sealed class GameClock : MonoBehaviour
{
    public static GameClock Instance { get; private set; }
    public int Day { get; private set; } = 1;
    public int MinuteOfDay { get; private set; } = 6 * 60;
    public float RealSecondsPerGameMinute = 0.5f;
    public bool Paused { get; set; }
    public event Action<int, int> TimeChanged;
    public event Action<int> DayChanged;
    private float accumulator;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        if (Paused || RealSecondsPerGameMinute <= 0) return;
        accumulator += Time.deltaTime;
        while (accumulator >= RealSecondsPerGameMinute)
        {
            accumulator -= RealSecondsPerGameMinute;
            AdvanceMinutes(1);
        }
    }

    public void SetTime(int day, int minute)
    {
        Day = Mathf.Max(1, day);
        MinuteOfDay = Mathf.Clamp(minute, 0, 23 * 60 + 59);
        TimeChanged?.Invoke(Day, MinuteOfDay);
    }

    public void AdvanceMinutes(int amount)
    {
        if (amount <= 0) return;
        int total = MinuteOfDay + amount;
        while (total >= 24 * 60)
        {
            total -= 24 * 60;
            Day++;
            DayChanged?.Invoke(Day);
        }
        MinuteOfDay = total;
        TimeChanged?.Invoke(Day, MinuteOfDay);
    }

    public void Sleep()
    {
        AdvanceMinutes(24 * 60 - MinuteOfDay + 6 * 60);
    }

    public string DisplayTime => $"Day {Day}  {MinuteOfDay / 60:00}:{MinuteOfDay % 60:00}";
}

public sealed class StaminaSystem : MonoBehaviour
{
    public static StaminaSystem Instance { get; private set; }
    public int MaxStamina = 100;
    public int CurrentStamina { get; private set; } = 100;
    public event Action<int, int> Changed;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (GameClock.Instance != null) GameClock.Instance.DayChanged += OnDayChanged;
    }

    private void OnDestroy()
    {
        if (GameClock.Instance != null) GameClock.Instance.DayChanged -= OnDayChanged;
        if (Instance == this) Instance = null;
    }

    private void OnDayChanged(int day) => Restore(MaxStamina);

    public bool Consume(int amount)
    {
        if (amount <= 0) return true;
        if (CurrentStamina < amount) return false;
        CurrentStamina -= amount;
        Changed?.Invoke(CurrentStamina, MaxStamina);
        return true;
    }

    public void Restore(int amount)
    {
        CurrentStamina = Mathf.Clamp(CurrentStamina + Mathf.Max(0, amount), 0, MaxStamina);
        Changed?.Invoke(CurrentStamina, MaxStamina);
    }

    public void SetValue(int value)
    {
        CurrentStamina = Mathf.Clamp(value, 0, MaxStamina);
        Changed?.Invoke(CurrentStamina, MaxStamina);
    }
}

public sealed class WalletSystem : MonoBehaviour
{
    public static WalletSystem Instance { get; private set; }
    public int Coins { get; private set; } = 100;
    public event Action<int> Changed;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public bool Spend(int amount)
    {
        if (amount < 0 || Coins < amount) return false;
        Coins -= amount;
        Changed?.Invoke(Coins);
        return true;
    }

    public void Add(int amount)
    {
        Coins = Mathf.Max(0, Coins + amount);
        Changed?.Invoke(Coins);
    }

    public void SetValue(int amount)
    {
        Coins = Mathf.Max(0, amount);
        Changed?.Invoke(Coins);
    }
}

public sealed class FarmSystem : MonoBehaviour
{
    public static FarmSystem Instance { get; private set; }
    public readonly List<FarmPlotState> Plots = new List<FarmPlotState>();
    public event Action<FarmPlotState> PlotChanged;
    public event Action<CropKind> Harvested;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (GameClock.Instance != null) GameClock.Instance.DayChanged += AdvanceDay;
    }

    public void EnsureDefaultPlots(Vector3 center)
    {
        if (Plots.Count > 0) return;
        Vector2Int origin = new Vector2Int(Mathf.RoundToInt(center.x), Mathf.RoundToInt(center.y));
        for (int y = -1; y <= 2; y++)
            for (int x = -2; x <= 2; x++) Plots.Add(new FarmPlotState(origin.x + x, origin.y + y));
    }

    public FarmPlotState GetPlot(Vector2Int position)
    {
        return Plots.Find(plot => plot.x == position.x && plot.y == position.y);
    }

    public bool Till(Vector2Int position)
    {
        FarmPlotState plot = GetPlot(position);
        if (plot == null || plot.tilled || plot.planted || !SpendAction(2)) return false;
        plot.tilled = true;
        PlotChanged?.Invoke(plot);
        return true;
    }

    public bool Water(Vector2Int position)
    {
        FarmPlotState plot = GetPlot(position);
        if (plot == null || !plot.tilled || !SpendAction(1)) return false;
        plot.watered = true;
        PlotChanged?.Invoke(plot);
        return true;
    }

    public bool Plant(Vector2Int position, CropKind crop)
    {
        FarmPlotState plot = GetPlot(position);
        ItemType seed = crop == CropKind.Carrot ? ItemType.seed_carrot : ItemType.seed_tomato;
        if (plot == null || !plot.tilled || plot.planted || InventoryService.Count(seed) < 1) return false;
        if (!SpendAction(1) || !InventoryService.TryConsume(seed)) return false;
        plot.crop = crop;
        plot.planted = true;
        plot.growthDays = 0;
        plot.ready = false;
        PlotChanged?.Invoke(plot);
        return true;
    }

    public bool Harvest(Vector2Int position)
    {
        FarmPlotState plot = GetPlot(position);
        if (plot == null || !plot.ready || !SpendAction(2)) return false;
        CropKind crop = plot.crop;
        ItemType seed = crop == CropKind.Carrot ? ItemType.seed_carrot : ItemType.seed_tomato;
        InventoryService.Add(seed, 2);
        WalletSystem.Instance?.Add(crop == CropKind.Carrot ? 15 : 20);
        plot.planted = false;
        plot.ready = false;
        plot.watered = false;
        plot.growthDays = 0;
        Harvested?.Invoke(crop);
        PlotChanged?.Invoke(plot);
        return true;
    }

    private bool SpendAction(int amount) => StaminaSystem.Instance == null || StaminaSystem.Instance.Consume(amount);

    private void AdvanceDay(int day)
    {
        foreach (FarmPlotState plot in Plots)
        {
            if (!plot.planted) continue;
            if (plot.watered) plot.growthDays++;
            plot.watered = false;
            plot.ready = plot.growthDays >= 3;
            PlotChanged?.Invoke(plot);
        }
    }

    public void NotifyAllPlots()
    {
        foreach (FarmPlotState plot in Plots) PlotChanged?.Invoke(plot);
    }
}

public sealed class ShopSystem : MonoBehaviour
{
    public static ShopSystem Instance { get; private set; }
    private readonly Dictionary<ItemType, int> prices = new Dictionary<ItemType, int>
    {
        { ItemType.seed_carrot, 5 },
        { ItemType.seed_tomato, 8 }
    };

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool Buy(ItemType type, int amount = 1)
    {
        if (!prices.TryGetValue(type, out int price) || amount <= 0) return false;
        if (WalletSystem.Instance == null || !WalletSystem.Instance.Spend(price * amount)) return false;
        InventoryService.Add(type, amount);
        return true;
    }

    public bool Sell(ItemType type, int amount = 1)
    {
        if (!prices.TryGetValue(type, out int price) || amount <= 0 || !InventoryService.TryConsume(type, amount)) return false;
        WalletSystem.Instance?.Add(Mathf.Max(1, price * amount / 2));
        return true;
    }

    public int Price(ItemType type) => prices.TryGetValue(type, out int price) ? price : 0;
}

public sealed class QuestSystem : MonoBehaviour
{
    public static QuestSystem Instance { get; private set; }
    public bool Accepted { get; private set; }
    public bool Completed { get; private set; }
    public bool RewardClaimed { get; private set; }
    public string CurrentQuest => "Harvest your first crop";

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (FarmSystem.Instance != null) FarmSystem.Instance.Harvested += OnHarvested;
    }

    private void OnHarvested(CropKind crop)
    {
        if (Accepted) Completed = true;
    }

    public string Accept()
    {
        Accepted = true;
        return CurrentQuest;
    }

    public bool ClaimReward()
    {
        if (!Completed || RewardClaimed) return false;
        RewardClaimed = true;
        WalletSystem.Instance?.Add(50);
        return true;
    }
}

public sealed class CookingSystem : MonoBehaviour
{
    public static CookingSystem Instance { get; private set; }
    public int MealsCooked { get; private set; }
    private readonly Dictionary<string, CookingRecipe> recipes = new Dictionary<string, CookingRecipe>
    {
        { "Carrot Soup", new CookingRecipe(ItemType.seed_carrot, 12) },
        { "Tomato Salsa", new CookingRecipe(ItemType.seed_tomato, 16) }
    };

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool Cook(string recipeName)
    {
        CookingRecipe recipe;
        if (!recipes.TryGetValue(recipeName, out recipe) || !InventoryService.TryConsume(recipe.ingredient)) return false;
        MealsCooked++;
        WalletSystem.Instance?.Add(recipe.reward);
        return true;
    }

    public string[] RecipeNames => new[] { "Carrot Soup", "Tomato Salsa" };
}

public sealed class NpcSystem : MonoBehaviour
{
    public static NpcSystem Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public string Interact(string npcId)
    {
        if (npcId == "Mayor") return QuestSystem.Instance != null && QuestSystem.Instance.Accepted ? "Keep tending the farm." : "Please grow a crop for the town.";
        if (npcId == "Shopkeeper") return "The shop sells carrot and tomato seeds.";
        if (npcId == "Cook") return "Bring seeds here and press K to cook a meal.";
        return "Hello, farmer.";
    }
}
