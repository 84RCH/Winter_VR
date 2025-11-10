using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    public enum GameMode
    {
        DefualtMode,
        CustumMode
    }

    public GameMode Mode;
    public bool NSFWMode = true;

    [System.Serializable]
    public class LauncherPreset
    {
        public float LanuchForce;
        public float spawnInterval;
    }

    public LauncherPreset DefualtModePreset;
    public LauncherPreset CustomModePreset;

    public BallLauncher BallLauncher;
    public GameObject MissArea1;
    public GameObject MissArea2;
    public GameObject MissArea3;
    public CountDown CountDown;

    public GameObject Player;
    private void Awake()
    {
        SpownPlayer();
    }
    private void Start()
    {
        LordConfig();
        StartCoroutine(SetPreSet());
    }

    private void SpownPlayer()
    {
        if(!GameObject.FindGameObjectWithTag("Player"))
        {
            Instantiate(Player, new Vector3(-1f, 1f, 0f), Quaternion.identity);
        }
    }

    private void LordConfig()
    {
        if (ConfigManager.Config.useCustomMode)
        {
            Mode = GameMode.CustumMode;

            CustomModePreset.LanuchForce = ConfigManager.Config.customLanuchForce;
            CustomModePreset.spawnInterval = ConfigManager.Config.customSpawnInterval;
        }
        else
        {
            Mode = GameMode.DefualtMode;
        }

        if (ConfigManager.Config.useVoice)
        {
            NSFWMode = true;
        }
        else
        {
            NSFWMode= false;
        }
    }

    private System.Collections.IEnumerator SetPreSet()
    {
        yield return null; // 等待一帧

        if (Mode == GameMode.DefualtMode)
        {
            BallLauncher.launchForce = DefualtModePreset.LanuchForce;
            BallLauncher.spawnInterval = DefualtModePreset.spawnInterval;
        }
        else if (Mode == GameMode.CustumMode)
        {
            BallLauncher.launchForce = CustomModePreset.LanuchForce;
            BallLauncher.spawnInterval = CustomModePreset.spawnInterval;
        }

        if(!NSFWMode)
        {
            MissArea1.SetActive(false);
            MissArea2.SetActive(false);
            MissArea3.SetActive(false);
            CountDown.SetNSFW();
        }
    }

    public void StartGame()
    {
        CountDown.FindText();
        CountDown.StartCountDown();
    }

    public void ReStartGame()
    {
        SceneManager.LoadScene(0);
    }


}
