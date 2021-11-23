using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerForMap : MonoBehaviour
{    
    public AudioSource backgroundMusic;

    private void Start()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 1)
            {
                backgroundMusic.volume = 1;
                backgroundMusic.Play();                
            }
            else
            {
                backgroundMusic.volume = 0;
                backgroundMusic.Play();
            }
        }
        else
        {
            PlayerPrefs.SetInt("Sound", 1);
            backgroundMusic.Play();
        }
    }

    public void AdjustVolume()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                backgroundMusic.volume = 0;                
            }
            else
            {
                backgroundMusic.volume = 1;                
            }
        }
    }    
}