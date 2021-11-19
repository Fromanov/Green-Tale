using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoalPanel : MonoBehaviour
{
    public Image goalImage;
    public Sprite goalSprite;
    public TMP_Text goalText;
    public string goalString;    

    void Start()
    {
        SetUp();
    }

    void SetUp()
    {
        goalImage.sprite = goalSprite;
        goalText.text = goalString;        
    }     
}
