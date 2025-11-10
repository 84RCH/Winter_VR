using UnityEngine;
using System.IO;
using System.Reflection;

public class ConfigManager : MonoBehaviour
{
    [System.Serializable] // 必须有
    public class GameConfig
    {
        public bool useCustomMode;

        public float customLanuchForce;
        public float customSpawnInterval;

        public bool useVoice;
    }

    public static GameConfig Config;
    private static string configPath;


    void Awake()
    {
        // 根据平台决定路径
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        string exeDir = Directory.GetParent(Application.dataPath).FullName;
        configPath = Path.Combine(exeDir, "config.json");
#else
        configPath = Path.Combine(Application.persistentDataPath, "config.json");
#endif

        LoadOrCreateConfig();
    }

    private void LoadOrCreateConfig()
    {
        if (File.Exists(configPath))
        {
            string json = File.ReadAllText(configPath);
            Config = JsonUtility.FromJson<GameConfig>(json);
        }
        else
        {
            // 默认配置
            Config = new GameConfig
            {
                useCustomMode = false,
                customLanuchForce = 20f,
                customSpawnInterval = 0.5f,
                useVoice = true
            };
            // 保存
            File.WriteAllText(configPath, JsonUtility.ToJson(Config, true));
        }
    }
}