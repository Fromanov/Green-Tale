using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DotTags
{
    Dot0,
    Dot1,
    Dot2,
    Dot3,
    Dot4,
    Dot5,
    Dot6,
    Dot7,
    Jelly,
    Concrete,
    Blank
}

[System.Serializable]
public class GoalBlank
{
    public int numberNeeded;
    public int numberCollected;
    public Sprite goalSprite;
    public DotTags matchValue;
}

public class GoalManager : MonoBehaviour
{
    public GoalBlank[] levelGoals;
    public GameObject goalPrafab;
    public GameObject goalBlankPrafab;
    public GameObject goalIntroParent;
    public GameObject goalGameParent;
    public List<GoalPanel> currentGoals = new List<GoalPanel>();

    private EndGameManager endGame;
    private Board board;    

    void Start()
    {
        board = FindObjectOfType<Board>();
        endGame = FindObjectOfType<EndGameManager>();

        SetGoalsFromLevelObj();
        SetUpGoals();        
    }

    void SetGoalsFromLevelObj()
    {
        if (board.world != null)
        {
            if(board.level < board.world.levels.Length)
            {
                if (board.world.levels[board.level] != null)
                {
                    for(int i = 0; i < board.world.levels[board.level].levelGoals.Length; i++)
                    {
                        board.world.levels[board.level].levelGoals[i].numberCollected = 0;
                    }
                    
                    levelGoals = board.world.levels[board.level].levelGoals;
                }
            }            
        }
    }

    void SetUpGoals()
    {
        for(int i = 0; i < levelGoals.Length; i++)
        {
            if(levelGoals[i].matchValue != DotTags.Blank)
            {
                GameObject goal = Instantiate(goalPrafab); //, goalIntroParent.transform.position, Quaternion.identity
                goal.transform.SetParent(goalIntroParent.transform);
                goal.transform.localScale = new Vector3(1,1,1);

                GoalPanel panel = goal.GetComponent<GoalPanel>();
                panel.goalSprite = levelGoals[i].goalSprite;                
                panel.goalString = "0 / " + levelGoals[i].numberNeeded;

                GameObject gameGoal = Instantiate(goalPrafab); //, goalGameParent.transform.position, Quaternion.identity
                gameGoal.transform.SetParent(goalGameParent.transform);
                gameGoal.transform.localScale = new Vector3(1, 1, 1);

                panel = gameGoal.GetComponent<GoalPanel>();
                currentGoals.Add(panel);
                panel.goalSprite = levelGoals[i].goalSprite;
                panel.goalString = "0 / " + levelGoals[i].numberNeeded;
            }
            else 
            {
                GameObject goal = Instantiate(goalBlankPrafab, goalIntroParent.transform.position, Quaternion.identity);
                goal.transform.SetParent(goalIntroParent.transform);
                goal.transform.localScale = new Vector3(1, 1, 1);

                GoalPanel panel = goal.GetComponent<GoalPanel>();
                panel.goalSprite = levelGoals[i].goalSprite;
                panel.goalString = "";

                GameObject gameGoal = Instantiate(goalBlankPrafab, goalGameParent.transform.position, Quaternion.identity);
                gameGoal.transform.SetParent(goalGameParent.transform);
                gameGoal.transform.localScale = new Vector3(1, 1, 1);

                panel = gameGoal.GetComponent<GoalPanel>();
                currentGoals.Add(panel);
                panel.goalSprite = levelGoals[i].goalSprite;
                panel.goalString = "";
            }
        }
    }

    public void UpdateGoals()
    {
        int goalsCompleted = 0;        

        for (int  i = 0; i < levelGoals.Length; i++)
        {
            if (levelGoals[i].matchValue == DotTags.Blank)
            {
                currentGoals[i].goalText.text = ""; // 
            }
            else
            {
                currentGoals[i].goalText.text = "" + levelGoals[i].numberCollected + " / " + levelGoals[i].numberNeeded;
            }

            if (levelGoals[i].numberCollected >= levelGoals[i].numberNeeded )
            {
                goalsCompleted++;

                if (levelGoals[i].matchValue == DotTags.Blank)
                {
                    currentGoals[i].goalText.text = "";
                }
                else
                {
                    currentGoals[i].goalText.text = "V"; // "\u2713"
                }                 
            }
        }

        if(goalsCompleted >= levelGoals.Length)
        {

            if (endGame != null) 
            {
                endGame.WinGame(); 
            }
        }
    }

    public void CompareGoals(string goalToCompare)
    {
        for(int i = 0; i < levelGoals.Length; i++)
        {
            if(goalToCompare == levelGoals[i].matchValue.ToString())
            {
                levelGoals[i].numberCollected++;
            }
        }
    }   
}
