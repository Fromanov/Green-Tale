using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public int hitPoints;

    private SpriteRenderer sprite;
    private GoalManager goalManager;
    private float alphaStep = .5f;


    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        goalManager = FindObjectOfType<GoalManager>();
    }

    void Update()
    {
        if(this.gameObject.tag == "Jelly" && hitPoints <= 0 || this.gameObject.tag == "Locked" && hitPoints <= 0 || this.gameObject.tag == "Concrete" && hitPoints <= 0)
        {
            if(goalManager != null)
            {
                goalManager.CompareGoals(this.gameObject.tag);
                goalManager.UpdateGoals();
            }

            Destroy(this.gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        MakeLighter();
    }

    void MakeLighter()
    {
        Color color = sprite.color;
          
        float newAlpha = color.a * alphaStep;
        sprite.color = new Color(color.r, color.g, color.b, newAlpha);
    }
}
