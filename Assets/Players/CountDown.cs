using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{
    public float CountDownMax = 7f;

    public AudioClip[] CDVocie;

    public Text CountText;

    public BallLauncher BallLauncher;

    private float timer = 0f;
    private int nextVoiceSecMax = 7;
    private int nextVoiceSec = 0;
    private int voiceIndex = 0;
    private int timeScale = 1;

    private  bool NSFWMode = false;
    private bool isStart = false;

    private AudioSource audioSource;

    void Start()
    {
        audioSource= GetComponent<AudioSource>();
        FindText();
        CountText.gameObject.SetActive(false);
        timer = CountDownMax + 2;
        nextVoiceSec = nextVoiceSecMax;
    }

    void Update()
    {
        if (!isStart) return;

        Timer();
        UpdateText();
        PlayCountVoice();
    }

    void Timer()
    {
        timer -= timeScale * Time.deltaTime;
    }

    void UpdateText()
    {
        float text = timer - 2f;
        CountText.text = text.ToString("F0");

        if (text < 0f)
        {
            CountText.text = "0";
        }
    }

    void UpdateText(bool clear)
    {
        if(clear)
        {
            CountText.text = "";
        }
    }

    void PlayCountVoice()
    {
        if ((int)timer == nextVoiceSec)
        {
            nextVoiceSec--;
            PlayVoice();
        }
    }

    void PlayVoice()
    {
        if (CDVocie == null || CDVocie.Length == 0) return;

        if (voiceIndex == CDVocie.Length)
        {
            timeScale = 0;
            UpdateText(true);
            BallLauncher.StartLauncher();
            isStart = false;

            return;
        }

        AudioClip clip = CDVocie[voiceIndex];
        voiceIndex++;

        if (NSFWMode) return;

        audioSource.PlayOneShot(clip);

        //GameObject SoundPlayer = new GameObject();
        //SoundPlayer.AddComponent<AudioSource>();
        //SoundPlayer.GetComponent<AudioSource>().PlayOneShot(clip);
        //
        //Destroy(SoundPlayer, clip.length + 0.1f);
    }

    public void SetNSFW()
    {
        NSFWMode = true;
    }

    public void StartCountDown()
    {
        isStart = true;

        CountText.gameObject.SetActive(true);
    }

    public void FindText()
    {
        if (CountText == null)
        {
            CountText = GameObject.FindGameObjectWithTag("CountText").GetComponent<Text>();
        }
    }

}
