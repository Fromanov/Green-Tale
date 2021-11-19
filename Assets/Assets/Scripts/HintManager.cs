using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    public float hintDelay;
    public GameObject hintParticle;
    public GameObject currentHint;

    private Board board;
    private float hintDelaySeconds;

    void Start()
    {
        board = FindObjectOfType<Board>();
        hintDelay = 1.5f;
        hintDelaySeconds = hintDelay;
    }


    void Update()
    {
        hintDelaySeconds -= Time.deltaTime;

        if (hintDelaySeconds <= 0 && currentHint == null && board.currentState == GameState.move)
        {
            MarkHint();
        }

        if (hintDelaySeconds <= 0 && currentHint != null) // сделал постоянные хинты
        {
            DestroyHint();
        }
    }

    List<GameObject> FindAllMatches()
    {
        List<GameObject> possibleMoves = new List<GameObject>();

        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allDots[i, j] != null)
                {
                    if (i < (board.width - 1))
                    {
                        if (board.SwitchAndCheck(i, j, Vector2.right))
                        {
                            possibleMoves.Add(board.allDots[i, j]);
                        }
                    }

                    if (j < (board.height - 1))
                    {
                        if (board.SwitchAndCheck(i, j, Vector2.up))
                        {
                            possibleMoves.Add(board.allDots[i, j]);
                        }
                    }
                }
            }
        }

        return possibleMoves;
    }

    GameObject PickOneRandomly()
    {
        List<GameObject> possibleMoves = new List<GameObject>();
        possibleMoves = FindAllMatches();

        if (possibleMoves.Count > 0)
        {
            int pieceToUse = Random.Range(0, possibleMoves.Count);
            return possibleMoves[pieceToUse];
        }
        return null;
    }

    private void MarkHint()
    {
        GameObject move = PickOneRandomly();

        if (move != null)
        {
            currentHint = Instantiate(hintParticle, move.transform.position, Quaternion.identity);
        }

        hintDelaySeconds = hintDelay;
    }

    public void DestroyHint()
    {
        if (currentHint != null)
        {
            Destroy(currentHint);
            currentHint = null;
            hintDelaySeconds = hintDelay;
        }
    }
}
