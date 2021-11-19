using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move,
    win,
    lose,
    pause,
    booster
}

public enum TileKind
{
    Jelly,
    Blank,
    Locked,
    Concrete,
    Normal
}

public enum BoosterType
{
    DeleteSingle,
    DeleteRow,
    DeleteColumn,
    TransformColorBomb
}

[System.Serializable]
public class MatchType
{
    public int type;
    public string color;
}

[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind tileKind;
}



public class Board : MonoBehaviour
{
    [Header("Scriptable Object Stuff")]
    public World world;
    public int level;

    public GameState currentState = GameState.move;

    [Header("Board Dimensions")]
    public int width;
    public int height;
    public int offSet;

    [Header("Prefabs")]
    public GameObject tilePrefab;
    public GameObject[] dots;
    public GameObject[,] allDots;
    public GameObject destroyEffect;
    public GameObject JellyTilePrefab;
    public GameObject LockedTilePrefab;
    public GameObject ConcreteTilePrefab;

    [Header("Layout")]
    public TileType[] boardLayout;
    public BackgroundTile[,] lockTiles;

    private bool[,] blankSpaces;
    private BackgroundTile[,] breakableTiles;
    private BackgroundTile[,] concreteTiles;

    [Header("Match Stuff")]
    public Dot currentDot;
    public int basePieceValue = 20;
    public float refillDelay = 0.5f;
    public int[] scoreGoals;
    public MatchType matchType;
    public SoundManager soundManager;

    private int streakValue = 1;
    private FindMatches findMatches;
    private ScoreManager scoreManager;
    private GoalManager goalManager;

    [Header("Interface Stuff")]
    public GameObject reshufflePanel;
    public Animator reshufflePanelAnimator;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("Current Level"))
        {
            level = PlayerPrefs.GetInt("Current Level");
        }

        if (world != null)
        {
            if (level < world.levels.Length)
            {
                if (world.levels[level] != null)
                {
                    width = world.levels[level].width;
                    height = world.levels[level].height;
                    dots = world.levels[level].dots;
                    boardLayout = world.levels[level].boardLayout;
                    scoreGoals = world.levels[level].scoreGoals;
                }
            }
        }
    }

    void Start()
    {
        goalManager = FindObjectOfType<GoalManager>();
        soundManager = FindObjectOfType<SoundManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        findMatches = FindObjectOfType<FindMatches>();

        blankSpaces = new bool[width, height];
        breakableTiles = new BackgroundTile[width, height];
        lockTiles = new BackgroundTile[width, height];
        concreteTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];

        SetUpBoard();


        while (IsDeadLocked()) // странная хуйня с дедлоками - может это поможет - while не помог - а всё заруинил
        {
            Debug.Log("Dead Locked!");
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (allDots[i, j] != null)
                    {
                        Destroy(allDots[i, j]);
                    }
                }
            }
            GenerateDots();
        }

        //yield return new WaitForSeconds(refillDelay);

        currentState = GameState.pause;
    }

    public void GenerateBlankSpaces()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Blank)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }

    public void GenerateBreakableSpaces()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Jelly)
            {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(JellyTilePrefab, tempPosition, Quaternion.identity);
                breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    private void GenerateLockedSpaces()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Locked)
            {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(LockedTilePrefab, tempPosition, Quaternion.identity);
                lockTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    private void GenerateConcreteSpaces()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Concrete)
            {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(ConcreteTilePrefab, tempPosition, Quaternion.identity);
                concreteTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    private void GenerateDots()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j] && !concreteTiles[i, j])
                {

                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    Vector2 tilePosition = new Vector2(i, j);

                    GameObject backgroundTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
                    backgroundTile.transform.parent = this.transform;
                    backgroundTile.name = "(" + "bg " + i + ", " + j + ")";

                    int dotToUse = Random.Range(0, dots.Length);
                    int maxIterations = 0;

                    while (MatchiesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                    {
                        dotToUse = Random.Range(0, dots.Length);
                        maxIterations++;
                    }

                    GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    dot.GetComponent<Dot>().row = j;
                    dot.GetComponent<Dot>().column = i;
                    dot.transform.parent = this.transform;
                    dot.name = "(" + "dt " + i + ", " + j + ")";
                    allDots[i, j] = dot;
                }
            }
        }
    }

    private void SetUpBoard()
    {
        GenerateBlankSpaces();
        GenerateBreakableSpaces();
        GenerateLockedSpaces();
        GenerateConcreteSpaces();
        GenerateDots();
    }

    private bool MatchiesAt(int column, int row, GameObject piece)
    {
        if (row > 1)
        {
            if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
            {
                if (piece.CompareTag(allDots[column, row - 1].tag) && piece.CompareTag(allDots[column, row - 2].tag))
                {
                    return true;
                }
            }
        }
        if (column > 1)
        {
            if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
            {
                if (piece.CompareTag(allDots[column - 1, row].tag) && piece.CompareTag(allDots[column - 2, row].tag))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private MatchType ColumnOrRow()
    {
        List<GameObject> matchCopy = findMatches.currentMatches as List<GameObject>;

        matchType.type = 0;
        matchType.color = "";

        for (int i = 0; i < matchCopy.Count; i++)
        {
            Dot thisDot = matchCopy[i].GetComponent<Dot>();
            string color = matchCopy[i].tag;

            int columnMatch = 0;
            int rowMatch = 0;

            for (int j = 0; j < matchCopy.Count; j++)
            {
                Dot nextDot = matchCopy[j].GetComponent<Dot>();

                if (nextDot == thisDot)
                {
                    continue;
                }

                if (nextDot.column == thisDot.column && nextDot.CompareTag(color))
                {
                    columnMatch++;
                }

                if (nextDot.row == thisDot.row && nextDot.CompareTag(color))
                {
                    rowMatch++;
                }
            }

            if (columnMatch == 4 || rowMatch == 4)
            {
                matchType.type = 1;
                matchType.color = color;

                return matchType;
            }
            else if (columnMatch == 2 && rowMatch == 2)
            {
                matchType.type = 2;
                matchType.color = color;

                return matchType;
            }
            else if (columnMatch == 3 || rowMatch == 3)
            {
                matchType.type = 3;
                matchType.color = color;

                return matchType;
            }
        }

        matchType.type = 0;
        matchType.color = "";

        return matchType;
    }

    private bool CheckTag(List<GameObject> _dots)
    {
        for (int i = 1; i <= _dots.Count - 1; i++)
        {
            if (!_dots[i].CompareTag(_dots[i - 1].tag))
            {
                return false;
            }
        }

        for (int i = 0; i <= _dots.Count - 1; i++)
        {
            if (CheckIsDotBonus(_dots[i]))
            {
                return false;
            }
        }

        return true;
    }

    private bool CheckIsDotBonus(GameObject _dot)
    {
        Dot _ddot = _dot.GetComponent<Dot>();

        return _ddot.isAdjacentBomb || _ddot.isColumnBomb || _ddot.isRowBomb || _ddot.isColorBomb;
    }

    private void CheckToMakeColorBomb()
    {

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (i >= 2 && i <= width - 3)
                {
                    List<GameObject> dotsToCheck = new List<GameObject>();

                    for (int k = i - 2; k <= i + 2; k++)
                    {
                        if ((allDots[k, j]) != null)
                        {
                            dotsToCheck.Add(allDots[k, j]);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (dotsToCheck.Count == 5 && CheckTag(dotsToCheck))
                    {
                        allDots[i, j].GetComponent<Dot>().isMached = false;
                        allDots[i, j].GetComponent<Dot>().MakeColorBomb();
                    }
                }

                if (j >= 2 && j <= height - 3)
                {
                    List<GameObject> dotsToCheck = new List<GameObject>();

                    for (int l = j - 2; l <= j + 2; l++)
                    {
                        if ((allDots[i, l]) != null)
                        {
                            dotsToCheck.Add(allDots[i, l]);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (dotsToCheck.Count == 5 && CheckTag(dotsToCheck))
                    {
                        allDots[i, j].GetComponent<Dot>().isMached = false;
                        allDots[i, j].GetComponent<Dot>().MakeColorBomb();
                    }
                }
            }
        }
    }

    private void CheckToMakeAdjacentBomb()
    {

        bool[,] isRowMatched = new bool[width, height];
        bool[,] isColumnMatched = new bool[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (i >= 1 && i <= width - 2)
                {
                    List<GameObject> dotsToCheck = new List<GameObject>();

                    for (int k = i - 1; k <= i + 1; k++)
                    {
                        if ((allDots[k, j]) != null)
                        {
                            dotsToCheck.Add(allDots[k, j]);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (dotsToCheck.Count == 3 && CheckTag(dotsToCheck))
                    {
                        isRowMatched[i, j] = true;
                    }
                    else
                    {
                        isRowMatched[i, j] = false;
                    }
                }
                else
                {
                    isRowMatched[i, j] = false;
                }

                if (j >= 1 && j <= height - 2)
                {
                    List<GameObject> dotsToCheck = new List<GameObject>();

                    for (int l = j - 1; l <= j + 1; l++)
                    {
                        if ((allDots[i, l]) != null)
                        {
                            dotsToCheck.Add(allDots[i, l]);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (dotsToCheck.Count == 3 && CheckTag(dotsToCheck))
                    {
                        isColumnMatched[i, j] = true;
                    }
                    else
                    {
                        isColumnMatched[i, j] = false;
                    }
                }
                else
                {
                    isColumnMatched[i, j] = false;
                }
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                bool _transform = false;

                if (i > 0 && j > 0)
                {
                    if (isRowMatched[i - 1, j] && isColumnMatched[i, j - 1])
                    {
                        _transform = true;
                    }
                }

                if (i > 0 && j < height - 1)
                {
                    if (isRowMatched[i - 1, j] && isColumnMatched[i, j + 1])
                    {
                        _transform = true;
                    }
                }

                if (i < width - 1 && j > 0)
                {
                    if (isRowMatched[i + 1, j] && isColumnMatched[i, j - 1])
                    {
                        _transform = true;
                    }
                }

                if (i < width - 1 && j < height - 1)
                {
                    if (isRowMatched[i + 1, j] && isColumnMatched[i, j + 1])
                    {
                        _transform = true;
                    }
                }

                if (i > 0)
                {
                    if (isRowMatched[i - 1, j] && isColumnMatched[i, j])
                    {
                        _transform = true;
                    }
                }

                if (i < width - 1)
                {
                    if (isRowMatched[i + 1, j] && isColumnMatched[i, j])
                    {
                        _transform = true;
                    }
                }

                if (j > 0)
                {
                    if (isRowMatched[i, j] && isColumnMatched[i, j - 1])
                    {
                        _transform = true;
                    }
                }

                if (j < height - 1)
                {
                    if (isRowMatched[i, j] && isColumnMatched[i, j + 1])
                    {
                        _transform = true;
                    }
                }




                if (_transform)
                {
                    allDots[i, j].GetComponent<Dot>().isMached = false;
                    allDots[i, j].GetComponent<Dot>().MakeAdjacentBomb();
                }

            }
        }
    }

    private void CheckToMakeRowColumnBomb()
    {

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (i >= 2 && i <= width - 2)
                {
                    List<GameObject> dotsToCheck = new List<GameObject>();

                    for (int k = i - 2; k <= i + 1; k++)
                    {
                        if ((allDots[k, j]) != null)
                        {
                            dotsToCheck.Add(allDots[k, j]);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (dotsToCheck.Count == 4 && CheckTag(dotsToCheck))
                    {
                        int _leftOrRightDot = Random.Range(0, 1);
                        allDots[i - _leftOrRightDot, j].GetComponent<Dot>().isMached = false;
                        allDots[i - _leftOrRightDot, j].GetComponent<Dot>().MakeRowBomb();
                    }
                }

                if (j >= 2 && j <= height - 2)
                {
                    List<GameObject> dotsToCheck = new List<GameObject>();

                    for (int l = j - 2; l <= j + 1; l++)
                    {
                        if ((allDots[i, l]) != null)
                        {
                            dotsToCheck.Add(allDots[i, l]);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (dotsToCheck.Count == 4 && CheckTag(dotsToCheck))
                    {
                        int _upOrDownDot = Random.Range(0, 1);
                        allDots[i, j - _upOrDownDot].GetComponent<Dot>().isMached = false;
                        allDots[i, j - _upOrDownDot].GetComponent<Dot>().MakeColumnBomb();
                    }
                }
            }
        }
    }

    private void CheckToMakeBombs()
    {
        if (findMatches.currentMatches.Count >= 5)
        {
            CheckToMakeColorBomb();
            CheckToMakeAdjacentBomb();
        }

        if (findMatches.currentMatches.Count >= 4)
        {
            CheckToMakeRowColumnBomb();
        }
    }

    public void DestroyConcreteTile(int i, int j)
    {
        concreteTiles[i, j] = null;

        GameObject backgroundTile = Instantiate(tilePrefab, new Vector2(i, j), Quaternion.identity) as GameObject;
        backgroundTile.transform.parent = this.transform;
        backgroundTile.name = "(" + "bg " + i + ", " + j + ")";

    }
    public void BombRow(int row)
    {
        for (int i = 0; i < width; i++)
        {
            if (concreteTiles[i, row])
            {
                concreteTiles[i, row].TakeDamage(1);

                if (concreteTiles[i, row].hitPoints <= 0)
                {
                    DestroyConcreteTile(i, row);
                }
            }
        }
    }

    public void BombColumn(int column)
    {
        for (int i = 0; i < width; i++)
        {
            if (concreteTiles[column, i])
            {
                concreteTiles[column, i].TakeDamage(1);

                if (concreteTiles[column, i].hitPoints <= 0)
                {
                    DestroyConcreteTile(column, i);
                }
            }
        }
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if (allDots[column, row].GetComponent<Dot>().isMached || allDots[column, row].GetComponent<Dot>().isFloorDamage)
        {
            if (breakableTiles[column, row] != null)
            {
                breakableTiles[column, row].TakeDamage(1);

                if (breakableTiles[column, row].hitPoints <= 0)
                {
                    breakableTiles[column, row] = null;
                }
            }

            if (lockTiles[column, row] != null)
            {
                lockTiles[column, row].TakeDamage(1);

                if (lockTiles[column, row].hitPoints <= 0)
                {
                    lockTiles[column, row] = null;
                }
            }

            DamageConcrete(column, row);

            allDots[column, row].GetComponent<Dot>().isFloorDamage = false;

            if (allDots[column, row].GetComponent<Dot>().isMached)
            {
                if (goalManager != null)
                {
                    goalManager.CompareGoals(allDots[column, row].tag.ToString());
                    goalManager.UpdateGoals();
                }

                if (soundManager != null)
                {
                    soundManager.PlayRandomDestroyNoise();
                }

                GameObject particle = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);

                Destroy(particle, .5f);
                Destroy(allDots[column, row]);
                scoreManager.IncreaseScore(basePieceValue * streakValue);
                allDots[column, row] = null;
            }
        }
    }

    public void DestroyMatches()
    {
        if (findMatches.currentMatches.Count >= 4)
        {
            CheckToMakeBombs();
        }

        findMatches.currentMatches.Clear();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }

        StartCoroutine(DecreaseRowCo());
    }

    private void DamageConcrete(int column, int row)
    {
        if (column > 0)
        {
            if (concreteTiles[column - 1, row])
            {
                concreteTiles[column - 1, row].TakeDamage(1);

                if (concreteTiles[column - 1, row].hitPoints <= 0)
                {
                    DestroyConcreteTile(column - 1, row);
                }
            }
        }

        if (column < width - 1)
        {
            if (concreteTiles[column + 1, row])
            {
                concreteTiles[column + 1, row].TakeDamage(1);

                if (concreteTiles[column + 1, row].hitPoints <= 0)
                {
                    DestroyConcreteTile(column + 1, row);
                }
            }
        }

        if (row > 0)
        {
            if (concreteTiles[column, row - 1])
            {
                concreteTiles[column, row - 1].TakeDamage(1);

                if (concreteTiles[column, row - 1].hitPoints <= 0)
                {
                    DestroyConcreteTile(column, row - 1);
                }
            }
        }

        if (row < height - 1)
        {
            if (concreteTiles[column, row + 1])
            {
                concreteTiles[column, row + 1].TakeDamage(1);

                if (concreteTiles[column, row + 1].hitPoints <= 0)
                {
                    DestroyConcreteTile(column, row + 1);
                }
            }
        }
    }

    private IEnumerator DecreaseRowCo()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j] && allDots[i, j] == null && !concreteTiles[i, j])
                {
                    for (int k = j + 1; k < height; k++)
                    {
                        if (lockTiles[i, k])
                        {
                            break;
                        }
                        else if (allDots[i, k] != null)
                        {
                            allDots[i, k].GetComponent<Dot>().row = j;
                            allDots[i, k] = null;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null && !blankSpaces[i, j] && !concreteTiles[i, j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int dotToUse = Random.Range(0, dots.Length);
                    int maxIterations = 0;

                    while (MatchiesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                    {
                        maxIterations++;
                        dotToUse = Random.Range(0, dots.Length);
                    }

                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    piece.name = "(" + "dt " + i + ", " + j + ")";
                    allDots[i, j] = piece;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        findMatches.FindAllMatches();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (allDots[i, j].GetComponent<Dot>().isMached)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        findMatches.FindAllMatches();
        RefillBoard();

        yield return new WaitForSeconds(refillDelay);


        while (MatchesOnBoard())
        {
            streakValue++;
            DestroyMatches();
            yield break;
        }

        currentDot = null;

        if (IsDeadLocked()) // странная хуйня с дедлоками - может это поможет - while не помог - а всё заруинил
        {
            Debug.Log("Dead Locked!");
            StartCoroutine(ShuffleBoard());
        }

        yield return new WaitForSeconds(refillDelay);
        currentState = GameState.move;
        streakValue = 1;
    }

    private void SwitchPieces(int column, int row, Vector2 direction) // правильно column
    {
        if (allDots[column + (int)direction.x, row + (int)direction.y] != null && lockTiles[column, row] == null && lockTiles[column + (int)direction.x, row + (int)direction.y] == null)
        {
            GameObject holder = allDots[column + (int)direction.x, row + (int)direction.y] as GameObject;
            allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row];
            allDots[column, row] = holder;
        }
    }

    private bool CheckForMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (i < (width - 2))
                    {
                        if (allDots[i + 1, j] != null && allDots[i + 2, j] != null)
                        {
                            if (allDots[i, j].CompareTag(allDots[i + 1, j].tag) && allDots[i, j].CompareTag(allDots[i + 2, j].tag))
                            {
                                return true;
                            }
                        }
                    }

                    if (j < (height - 2))
                    {
                        if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                        {
                            if (allDots[i, j].CompareTag(allDots[i, j + 1].tag) && allDots[i, j].CompareTag(allDots[i, j + 2].tag))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    public bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);

        if (CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;
    }

    private bool IsDeadLocked()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (i < (width - 1))
                    {
                        if (SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }

                    if (j < (height - 1))
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    private IEnumerator ShuffleBoard()
    {
        reshufflePanel.SetActive(true);
        reshufflePanelAnimator.SetBool("isShuffle", true);

        yield return new WaitForSeconds(refillDelay * 2);

        reshufflePanelAnimator.SetBool("isShuffle", false);
        yield return new WaitForSeconds(refillDelay * 3);
        reshufflePanel.SetActive(false);

        List<GameObject> newBoard = new List<GameObject>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null && lockTiles[i, j] == null) //Нужно, чтобы он не перемешивал фишки под замком надеюсь сработает (Фроман)
                {
                    newBoard.Add(allDots[i, j]);
                }

            }
        }

        yield return new WaitForSeconds(refillDelay);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j] && !concreteTiles[i, j])
                {
                    if (lockTiles[i, j] == null)
                    {
                        int pieceToUse = Random.Range(0, newBoard.Count);

                        int maxIterations = 0;

                        while (MatchiesAt(i, j, newBoard[pieceToUse]) && maxIterations < 100)
                        {
                            pieceToUse = Random.Range(0, newBoard.Count);
                            maxIterations++;
                        }

                        Dot piece = newBoard[pieceToUse].GetComponent<Dot>();
                        piece.column = i;
                        piece.row = j;
                        allDots[i, j] = newBoard[pieceToUse];
                        newBoard.Remove(newBoard[pieceToUse]);
                    }
                }
            }
        }
        if (IsDeadLocked()) //для починки дедлоков - while не помог - думаем дальше
        {
            StartCoroutine(ShuffleBoard());  //корутин всё починил - а почему - не знаю ._.
        }
    }
}