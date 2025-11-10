using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class TargetControl : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;

    public GameObject Cracker;

    public Transform crackerOffset1;
    public Transform crackerOffset2;
    public Transform crackerOffset3;
    public Transform crackerOffset4;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            rb.useGravity = true;
            audioSource.PlayOneShot(audioClip);
            SetCraker();
            Destroy(this, 3f);
        }
    }

    void PlaySE()
    {
        GameObject SEPlayer = new GameObject();
        SEPlayer.AddComponent<AudioSource>();
        SEPlayer.GetComponent<AudioSource>().PlayOneShot(audioClip);

        Destroy(SEPlayer, audioClip.length + 0.1f);
    }

    private void SetCraker()
    {
        GameObject cracker1 = Instantiate(Cracker, crackerOffset1);
        GameObject cracker2 = Instantiate(Cracker, crackerOffset2);
        GameObject cracker3 = Instantiate(Cracker, crackerOffset3);
        GameObject cracker4 = Instantiate(Cracker, crackerOffset4);

        Destroy(cracker1, 4f);
        Destroy(cracker2, 4f);
        Destroy(cracker3, 4f);
        Destroy(cracker4, 4f);
    }
}
