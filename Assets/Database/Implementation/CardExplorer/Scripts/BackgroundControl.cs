using UnityEngine;
using UnityEngine.UI;

public class BackgroundControl : MonoBehaviour
{

    [SerializeField] private Sprite SoundOn;
    [SerializeField] private Sprite SoundOff;
    [SerializeField] private Button Btn;
    [SerializeField] private GameObject BGObject;


    public void ControlBGMusic()
    {
        if (Btn.image.sprite == SoundOn)
        {
            Btn.image.sprite = SoundOff;
            BGObject.SetActive(false);
        }
        else
        {
            Btn.image.sprite = SoundOn;
            BGObject.SetActive(true);
        }
            
    }
}
