using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    public GameObject aboutPanel;
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    public Image soundButton;

    private SoundManagerForMap sound;

    private void Start()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                soundButton.sprite = musicOffSprite;
            }
            else
            {
                soundButton.sprite = musicOnSprite;
            }
        }
        else
        {
            soundButton.sprite = musicOnSprite;
        }

        sound = FindObjectOfType<SoundManagerForMap>();
    }

    private void Update()
    {
        
    }

    public void CancelButton()
    {

        this.gameObject.SetActive(false);        
    }

    public void SoundButton()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                soundButton.sprite = musicOnSprite;
                PlayerPrefs.SetInt("Sound", 1);
                sound.AdjustVolume();
            }
            else
            {
                soundButton.sprite = musicOffSprite;
                PlayerPrefs.SetInt("Sound", 0);
                sound.AdjustVolume();
            }
        }
    }

    public void AboutButton()
    {
        aboutPanel.SetActive(true);
    }

    public void AboutButtonExit()
    {
        aboutPanel.SetActive(false);
    }
}
