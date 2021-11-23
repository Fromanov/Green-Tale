using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomBackground : MonoBehaviour
{
    public Sprite[] backgrounds;

    private Image tempBack;

    // Start is called before the first frame update
    void Start()
    {
        tempBack = GetComponent<Image>();

        if(backgrounds.Length > 0)
        {
            tempBack.sprite = backgrounds[Random.Range(0, backgrounds.Length)];
        }        
    }    
}
