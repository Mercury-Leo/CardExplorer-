using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private Vector3 originalScale;
    private AudioSource audioSource;

    [SerializeField] float scale = 1.5f;
    [SerializeField] AudioClip cardSound;

    void Start()
    {
        audioSource = GameObject.Find("SoundManager").GetComponent<AudioSource>();
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        audioSource.PlayOneShot(cardSound);
        transform.localScale = new Vector3(scale, scale, scale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;
    }

    


}
