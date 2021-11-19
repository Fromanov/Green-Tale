using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Board Variables")]
    public int column;
    public int row;
    public int previusColumn;
    public int previusRow;
    public int targetX;
    public int targetY;
    public bool isMached = false;
    public bool isFloorDamage = false;
    public GameObject otherDot;

    private EndGameManager endGameManager;
    private HintManager hintManager;
    private FindMatches findMatches;
    private BoosterManager boosterManager;
    private Board board;
    private Vector2 firstTouchPosition = Vector2.zero;
    private Vector2 finalTouchPosition = Vector2.zero;
    private Vector2 tempPosition;

    [Header("Swipe Stuff")]
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    [Header("PowerUp Stuff")]
    public bool isAdjacentBomb;
    public bool isColorBomb;
    public bool isColumnBomb;
    public bool isRowBomb;
    public bool isBombNeedsToBeActivated = false;

    public GameObject rowArrow;
    public GameObject columnArrow;
    public GameObject colorBomb;
    public GameObject adjacentMarker;
    public Sprite nullSprite;

    void Start()
    {
        isColumnBomb = false;
        isRowBomb = false;
        isColorBomb = false;
        isAdjacentBomb = false;

        endGameManager = FindObjectOfType<EndGameManager>();
        hintManager = FindObjectOfType<HintManager>();
        boosterManager = FindObjectOfType<BoosterManager>();

        board = GameObject.FindWithTag("Board").GetComponent<Board>();

        findMatches = FindObjectOfType<FindMatches>();
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        row = targetY;
        column = targetX;
        previusRow = row;
        previusColumn = column;
    }


    void Update()
    {
        targetX = column;
        targetY = row;

        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
                findMatches.FindAllMatches();
            }
        }
        else
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;

        }

        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
                findMatches.FindAllMatches();
            }
        }
        else
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
        }
    }

    private void OnMouseDown()
    {
        if (hintManager != null)
        {
            hintManager.DestroyHint();
        }

        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (boosterManager.isBoosterPressed)
        {
            boosterManager.isBoosterPressed = false;

            if (boosterManager.boosterType == BoosterType.TransformColorBomb)
            {
                isRowBomb = false;
                isColumnBomb = false;
                isAdjacentBomb = false;
                MakeColorBomb();
            }

            else

            {
                if (boosterManager.boosterType == BoosterType.DeleteColumn)
                {
                    findMatches.GetColumnPieces(column);
                }

                if (boosterManager.boosterType == BoosterType.DeleteRow)

                {
                    findMatches.GetRowPieces(row);
                }

                isMached = true;

                board.DestroyMatches();
            }

        }

    }

    private void OnMouseUp()
    {
        if (board.currentState == GameState.move)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }

        if (board.currentState == GameState.booster)
        {
            board.currentState = GameState.move;
        }
    }

    private void CalculateAngle()
    {
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            board.currentState = GameState.wait;
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * Mathf.Rad2Deg;
            MovePieces();
            board.currentDot = this;
        }
        else
        {
            board.currentState = GameState.move;
        }
    }

    void MovePiecesActual(Vector2 direction)
    {
        otherDot = board.allDots[column + (int)direction.x, row + (int)direction.y];
        previusRow = row;
        previusColumn = column;

        if (board.lockTiles[column, row] == null && board.lockTiles[column + (int)direction.x, row + (int)direction.y] == null)

        {
            if (otherDot != null)
            {
                otherDot.GetComponent<Dot>().column += -1 * (int)direction.x;
                otherDot.GetComponent<Dot>().row += -1 * (int)direction.y;
                column += (int)direction.x;
                row += (int)direction.y;
                StartCoroutine(CheckMoveCo());
            }
            else
            {
                board.currentState = GameState.move;
            }

            board.soundManager.PlayRandomMovePieceNoise();

        }
        else
        {
            board.currentState = GameState.move;
        }
    }

    void MovePieces()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1) // старое сравнение выдавало out of range
        {
            //Right Swipe            
            MovePiecesActual(Vector2.right);
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1) // верхнее
        {
            //Up Swipe
            MovePiecesActual(Vector2.up);
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            // Left Swipe
            MovePiecesActual(Vector2.left);
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            // Down Swipe
            MovePiecesActual(Vector2.down);
        }
        else // только если мы не сдвинули тогда можно двигатся
        {
            board.currentState = GameState.move;
        }
    }

    public IEnumerator CheckMoveCo()
    {
        if (isColorBomb)
        {
            findMatches.MatchPiecesOfColor(otherDot.tag);
            isMached = true;
        }
        else if (otherDot.GetComponent<Dot>().isColorBomb)
        {
            findMatches.MatchPiecesOfColor(this.tag);
            otherDot.GetComponent<Dot>().isMached = true;
        }

        yield return new WaitForSeconds(.5f);

        if (otherDot != null)
        {
            if (!isMached && !otherDot.GetComponent<Dot>().isMached)
            {
                board.soundManager.PlayRandomMovePieceNoise();
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                row = previusRow;
                column = previusColumn;
                yield return new WaitForSeconds(.5f);
                board.currentDot = null;
                board.currentState = GameState.move;
            }
            else
            {
                if (endGameManager != null && endGameManager.requirments.gameType == GameType.Moves)
                {
                    endGameManager.DecreseCounterValue();
                }

                board.DestroyMatches();
            }
        }
    }

    public void MakeRowBomb()
    {
        if (!isColumnBomb && !isColorBomb && !isAdjacentBomb)
        {
            isRowBomb = true;
            isFloorDamage = true;
            isBombNeedsToBeActivated = true;
            GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = this.transform;
            this.GetComponent<SpriteRenderer>().sprite = nullSprite;

        }
    }
    public void MakeColumnBomb()
    {
        if (!isRowBomb && !isColorBomb && !isAdjacentBomb)
        {
            isColumnBomb = true;
            isFloorDamage = true;
            isBombNeedsToBeActivated = true;
            GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = this.transform;
            this.GetComponent<SpriteRenderer>().sprite = nullSprite;
        }
    }

    public void MakeColorBomb()
    {
        if (!isRowBomb && !isColumnBomb && !isAdjacentBomb)
        {
            isColorBomb = true;
            isFloorDamage = true;
            isBombNeedsToBeActivated = true;
            GameObject color = Instantiate(colorBomb, transform.position, Quaternion.identity);
            color.transform.parent = this.transform;
            this.gameObject.tag = "ColorBomb";
            this.GetComponent<SpriteRenderer>().sprite = nullSprite;
        }
    }

    public void MakeAdjacentBomb()
    {
        if (!isRowBomb && !isColumnBomb && !isColorBomb)
        {
            isAdjacentBomb = true;
            isFloorDamage = true;
            isBombNeedsToBeActivated = true;
            GameObject marker = Instantiate(adjacentMarker, transform.position, Quaternion.identity);
            marker.transform.parent = this.transform;
            this.GetComponent<SpriteRenderer>().sprite = nullSprite;
        }
    }
}
