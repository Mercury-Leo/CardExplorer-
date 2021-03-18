using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField] AudioClip shuffleSound;

    void Awake()
    {
        audioSource = GameObject.Find("SoundManager").GetComponent<AudioSource>();
    }

    public void PlayShuffle()
    {
        audioSource.PlayOneShot(shuffleSound);
    }
}
