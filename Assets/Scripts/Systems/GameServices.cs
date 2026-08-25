using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class InventorySaveEntry
{
    public ItemType type;
    public int count;
}

[Serializable]
public class FarmSaveData
{
    public int day;
    public int minute;
    public int stamina;
    public int coins;
    public List<InventorySaveEntry> inventory = new List<InventorySaveEntry>();
    public List<FarmPlotState> plots = new List<FarmPlotState>();
}

public sealed class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }
    public string SavePath => Path.Combine(Application.persistentDataPath, "farm_save.json");

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Save()
    {
        FarmSaveData data = new FarmSaveData
        {
            day = GameClock.Instance != null ? GameClock.Instance.Day : 1,
            minute = GameClock.Instance != null ? GameClock.Instance.MinuteOfDay : 360,
            stamina = StaminaSystem.Instance != null ? StaminaSystem.Instance.CurrentStamina : 100,
            coins = WalletSystem.Instance != null ? WalletSystem.Instance.Coins : 100
        };

        if (InventoryService.IsReady)
        {
            Dictionary<ItemType, int> counts = new Dictionary<ItemType, int>();
            foreach (SlotData slot in InventoryManager.Instance.Seedbackpack.slotList)
            {
                if (slot.item == null || slot.count <= 0) continue;
                if (!counts.ContainsKey(slot.item.type)) counts[slot.item.type] = 0;
                counts[slot.item.type] += slot.count;
            }
            foreach (KeyValuePair<ItemType, int> pair in counts)
                data.inventory.Add(new InventorySaveEntry { type = pair.Key, count = pair.Value });
        }

        if (FarmSystem.Instance != null) data.plots.AddRange(FarmSystem.Instance.Plots);
        File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
    }

    public bool Load()
    {
        if (!File.Exists(SavePath)) return false;
        FarmSaveData data = JsonUtility.FromJson<FarmSaveData>(File.ReadAllText(SavePath));
        if (GameClock.Instance != null) GameClock.Instance.SetTime(data.day, data.minute);
        if (StaminaSystem.Instance != null) StaminaSystem.Instance.SetValue(data.stamina);
        if (WalletSystem.Instance != null) WalletSystem.Instance.SetValue(data.coins);
        if (InventoryService.IsReady)
        {
            foreach (SlotData slot in InventoryManager.Instance.Seedbackpack.slotList) slot.Clear();
            if (data.inventory != null)
                foreach (InventorySaveEntry entry in data.inventory) InventoryService.Add(entry.type, entry.count);
        }
        if (FarmSystem.Instance != null && data.plots != null && data.plots.Count > 0)
        {
            FarmSystem.Instance.Plots.Clear();
            FarmSystem.Instance.Plots.AddRange(data.plots);
            FarmSystem.Instance.NotifyAllPlots();
        }
        return true;
    }
}

public sealed class SettingsSystem : MonoBehaviour
{
    public static SettingsSystem Instance { get; private set; }
    public float MasterVolume { get; private set; }
    public bool Fullscreen { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        MasterVolume = PlayerPrefs.GetFloat("master_volume", 1f);
        Fullscreen = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        Apply();
    }

    public void SetMasterVolume(float value)
    {
        MasterVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("master_volume", MasterVolume);
        Apply();
    }

    public void ToggleFullscreen()
    {
        Fullscreen = !Fullscreen;
        PlayerPrefs.SetInt("fullscreen", Fullscreen ? 1 : 0);
        Apply();
    }

    public void Apply()
    {
        AudioListener.volume = MasterVolume;
        Screen.fullScreen = Fullscreen;
        PlayerPrefs.Save();
    }
}

public sealed class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public AudioSource MusicSource { get; private set; }
    public AudioSource EffectsSource { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        MusicSource = gameObject.AddComponent<AudioSource>();
        EffectsSource = gameObject.AddComponent<AudioSource>();
        MusicSource.loop = true;
        MusicSource.playOnAwake = false;
        EffectsSource.playOnAwake = false;
    }

    private void Start()
    {
        if (MusicSource.clip == null) PlayMusic(CreateAmbientClip());
    }

    private AudioClip CreateAmbientClip()
    {
        const int sampleRate = 22050;
        const int seconds = 2;
        AudioClip clip = AudioClip.Create("FarmAmbient", sampleRate * seconds, 1, sampleRate, false);
        float[] samples = new float[sampleRate * seconds];
        for (int i = 0; i < samples.Length; i++)
            samples[i] = Mathf.Sin(2f * Mathf.PI * 220f * i / sampleRate) * 0.025f;
        clip.SetData(samples, 0);
        MusicSource.volume = 0.2f;
        return clip;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        MusicSource.clip = clip;
        MusicSource.Play();
    }

    public void PlayEffect(AudioClip clip)
    {
        if (clip != null) EffectsSource.PlayOneShot(clip);
    }
}

public sealed class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }
    private readonly string[] sceneNames = { "Home", "Spawn", "Restaurant", "alley" };

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName)) return;
        if (SceneUtility.GetBuildIndexByScenePath("Assets/Scenes/" + sceneName + ".scene") < 0) return;
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneByIndex(int index)
    {
        if (index >= 0 && index < sceneNames.Length) LoadScene(sceneNames[index]);
    }

    public void ReturnHome() => LoadScene("Home");
}
