using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadePanelController : MonoBehaviour
{
    public Animator panelFadeAnim;
    public Animator gameInfoAnim;

    private float oneSec = 1f;

    public void ButtonOK()
    {
        if (panelFadeAnim != null && gameInfoAnim != null)
        {
            panelFadeAnim.SetBool("Out", true);
            gameInfoAnim.SetBool("Out", true);
            StartCoroutine(GameStartCo());
        }
    }

    public void ButtonRetry()
    {
        if (panelFadeAnim != null && gameInfoAnim != null)
        {
            panelFadeAnim.SetBool("Out", true);
            gameInfoAnim.SetBool("Out", true);
        }
    }

    public void GameOver()
    {
        panelFadeAnim.SetBool("Out", false);
        panelFadeAnim.SetBool("Game Over", true);
    }

    IEnumerator GameStartCo()
    {
        yield return new WaitForSeconds(oneSec);

        Board board = FindObjectOfType<Board>();

        if (board != null)
        {
            board.currentState = GameState.move;
        }
    }
}
