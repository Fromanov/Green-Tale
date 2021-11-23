using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollManager : MonoBehaviour
{
    private GameData gameData;
    private ScrollRect scroll;

    void Start()
    {
        scroll = FindObjectOfType<ScrollRect>();
        gameData = FindObjectOfType<GameData>();

        if (scroll != null)
        {
            scroll.verticalNormalizedPosition = gameData.saveData.scrollPos;
        }
    }    
    
    public void ScrollRectVectorSave()
    {
        gameData.saveData.scrollPos = scroll.verticalNormalizedPosition;
    }
}
