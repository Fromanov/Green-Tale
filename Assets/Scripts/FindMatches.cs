using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    private Board board;

    public List<GameObject> currentMatches = new List<GameObject>();

    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private List<GameObject> IsRowBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();

        if (dot1.isRowBomb && dot1.isBombNeedsToBeActivated)
        {
            dot1.isBombNeedsToBeActivated = false;
            currentMatches.Union(GetRowPieces(dot1.row));
            board.BombRow(dot1.row);
        }

        if (dot2.isRowBomb && dot2.isBombNeedsToBeActivated)
        {
            dot2.isBombNeedsToBeActivated = false;
            currentMatches.Union(GetRowPieces(dot2.row));
            board.BombRow(dot2.row);
        }

        if (dot3.isRowBomb && dot3.isBombNeedsToBeActivated)
        {
            dot3.isBombNeedsToBeActivated = false;
            currentMatches.Union(GetRowPieces(dot3.row));
            board.BombRow(dot3.row);
        }

        return currentDots;
    }

    private List<GameObject> IsColumnBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();

        if (dot1.isColumnBomb && dot1.isBombNeedsToBeActivated)
        {
            dot1.isBombNeedsToBeActivated = false;
            currentMatches.Union(GetColumnPieces(dot1.column));
            board.BombColumn(dot1.column);
        }

        if (dot2.isColumnBomb && dot2.isBombNeedsToBeActivated)
        {
            dot2.isBombNeedsToBeActivated = false;
            currentMatches.Union(GetColumnPieces(dot2.column));
            board.BombColumn(dot2.column);
        }

        if (dot3.isColumnBomb && dot3.isBombNeedsToBeActivated)
        {
            dot3.isBombNeedsToBeActivated = false;
            currentMatches.Union(GetColumnPieces(dot3.column));
            board.BombColumn(dot3.column);
        }

        return currentDots;
    }

    private List<GameObject> IsAjacentBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();

        if (dot1.isAdjacentBomb && dot1.isBombNeedsToBeActivated)
        {
            dot1.isBombNeedsToBeActivated = false;
            currentMatches.Union(GetAjacentPieces(dot1.column, dot1.row));
        }

        if (dot2.isAdjacentBomb && dot2.isBombNeedsToBeActivated)
        {
            dot2.isBombNeedsToBeActivated = false;
            currentMatches.Union(GetAjacentPieces(dot2.column, dot2.row));
        }

        if (dot3.isAdjacentBomb && dot3.isBombNeedsToBeActivated)
        {
            dot3.isBombNeedsToBeActivated = false;
            currentMatches.Union(GetAjacentPieces(dot3.column, dot3.row));
        }

        return currentDots;
    }

    private void AddToListAndMatch(GameObject dot)
    {
        if (!currentMatches.Contains(dot))
        {
            currentMatches.Add(dot);
        }
        dot.GetComponent<Dot>().isMached = true;
    }

    private void GetNearbyPieces(GameObject dot1, GameObject dot2, GameObject dot3)
    {
        AddToListAndMatch(dot1);
        AddToListAndMatch(dot2);
        AddToListAndMatch(dot3);
    }


    private IEnumerator FindAllMatchesCo()
    {
        yield return null;

        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentDot = board.allDots[i, j];

                if (currentDot != null)
                {
                    Dot currentCurrentDot = currentDot.GetComponent<Dot>();

                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftDot = board.allDots[i - 1, j];
                        GameObject rightDot = board.allDots[i + 1, j];

                        if (leftDot != null && rightDot != null)
                        {
                            Dot rightRightDot = rightDot.GetComponent<Dot>();
                            Dot leftLeftDot = leftDot.GetComponent<Dot>();

                            if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                            {
                                currentMatches.Union(IsRowBomb(leftLeftDot, currentCurrentDot, rightRightDot));
                                currentMatches.Union(IsColumnBomb(leftLeftDot, currentCurrentDot, rightRightDot));
                                currentMatches.Union(IsAjacentBomb(leftLeftDot, currentCurrentDot, rightRightDot));

                                GetNearbyPieces(leftDot, currentDot, rightDot);
                            }
                        }
                    }

                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upDot = board.allDots[i, j + 1];
                        GameObject downDot = board.allDots[i, j - 1];

                        if (upDot != null && downDot != null)
                        {
                            Dot downDownDot = downDot.GetComponent<Dot>();
                            Dot upUpDot = upDot.GetComponent<Dot>();

                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                            {
                                currentMatches.Union(IsRowBomb(upUpDot, currentCurrentDot, downDownDot));
                                currentMatches.Union(IsColumnBomb(upUpDot, currentCurrentDot, downDownDot));
                                currentMatches.Union(IsAjacentBomb(upUpDot, currentCurrentDot, downDownDot));

                                GetNearbyPieces(upDot, currentDot, downDot);
                            }
                        }
                    }
                }
            }
        }
    }

    public void MatchPiecesOfColor(string color)
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allDots[i, j] != null)
                {
                    if (board.allDots[i, j].tag == color)
                    {
                        board.allDots[i, j].GetComponent<Dot>().isMached = true;
                    }
                }
            }
        }
    }

    List<GameObject> GetAjacentPieces(int column, int row)
    {
        List<GameObject> dots = new List<GameObject>();

        for (int i = column - 1; i <= column + 1; i++)
        {
            for (int j = row - 1; j <= row + 1; j++)
            {
                if (i >= 0 && i < board.width && j >= 0 && j < board.height)
                {
                    if (board.allDots[i, j] != null)
                    {
                        Dot dot = board.allDots[i, j].GetComponent<Dot>();

                        if (dot.isRowBomb && dot.isBombNeedsToBeActivated)
                        {
                            dot.isBombNeedsToBeActivated = false;
                            dots.Union(GetRowPieces(i)).ToList();
                        }

                        if (dot.isColumnBomb && dot.isBombNeedsToBeActivated)
                        {
                            dot.isBombNeedsToBeActivated = false;
                            dots.Union(GetColumnPieces(j)).ToList();
                        }

                        if (dot.isAdjacentBomb && dot.isBombNeedsToBeActivated)
                        {
                            dot.isBombNeedsToBeActivated = false;
                            dots.Union(GetAjacentPieces(i, j)).ToList();
                        }

                        dots.Add(board.allDots[i, j]);
                        dot.isMached = true;
                    }
                }
            }
        }
        return dots;
    }

    public List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();

        for (int i = 0; i < board.height; i++)
        {
            if (board.allDots[column, i] != null)
            {
                Dot dot = board.allDots[column, i].GetComponent<Dot>();

                if (dot.isRowBomb && dot.isBombNeedsToBeActivated)
                {
                    dot.isBombNeedsToBeActivated = false;
                    dots.Union(GetRowPieces(i)).ToList();
                }

                if (dot.isAdjacentBomb && dot.isBombNeedsToBeActivated)
                {
                    dot.isBombNeedsToBeActivated = false;
                    dots.Union(GetAjacentPieces(column, i)).ToList();
                }

                dots.Add(board.allDots[column, i]);
                dot.isMached = true;
            }
        }
        return dots;
    }

    public List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.allDots[i, row] != null)
            {
                Dot dot = board.allDots[i, row].GetComponent<Dot>();

                if (dot.isColumnBomb && dot.isBombNeedsToBeActivated)
                {
                    dot.isBombNeedsToBeActivated = false;
                    dots.Union(GetColumnPieces(i)).ToList();
                }

                if (dot.isAdjacentBomb && dot.isBombNeedsToBeActivated)
                {
                    dot.isBombNeedsToBeActivated = false;
                    dots.Union(GetAjacentPieces(i, row)).ToList();
                }

                dots.Add(board.allDots[i, row]);
                dot.isMached = true;
            }
        }
        return dots;
    }
}
