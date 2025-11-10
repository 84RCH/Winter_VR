using UnityEngine;
using UnityEngine.Audio;

public class RandomAudioPlayer : MonoBehaviour
{
    public AudioSource audioSource;

    [Tooltip("ƒTƒEƒ“ƒh")]
    public AudioClip[] clips;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            PlayRandomClip();
            other.tag = "Untagged";
        }
    }

    void PlayRandomClip()
    {
        if (clips == null || clips.Length == 0) return;

        int index = Random.Range(0, clips.Length);
        AudioClip clip = clips[index];

        audioSource.PlayOneShot(clip);

        //GameObject SoundPlayer = new GameObject();
        //SoundPlayer.AddComponent<AudioSource>();
        //SoundPlayer.GetComponent<AudioSource>().PlayOneShot(clip);
        //
        //Destroy(SoundPlayer, clip.length + 0.1f);
    }
}
