using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameBoard : MonoBehaviour
{
    public int width = 8;
    public int height = 8;
    public GameObject[] gamePieces;
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public GameObject endGamePanel;
    public TMP_Text endGameScoreText;
    public TMP_Text highScoreText;
    public Button mainMenuButton;

    private int score = 0;
    private float gameTime = 60f; 
    private GameObject[,] allPieces;
    private GamePiece selectedPiece;
    private bool isSwapping = false;

    private int highScore = 0;

    void Start()
    {
        allPieces = new GameObject[width, height];
        SetupBoard();
        UpdateScore(0);
        endGamePanel.SetActive(false); 
        mainMenuButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
        StartCoroutine(GameTimer());

        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    void Update()
    {
        if (gameTime > 0)
        {
            gameTime -= Time.deltaTime;
            timerText.text = "Time: " + Mathf.RoundToInt(gameTime).ToString();
        }
        else
        {
            EndGame();
        }
    }

    void SetupBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 position = new Vector2(x, y);
                int pieceToUse = Random.Range(0, gamePieces.Length);
                while (HasInitialMatch(x, y, gamePieces[pieceToUse]))
                {
                    pieceToUse = Random.Range(0, gamePieces.Length);
                }
                GameObject piece = Instantiate(gamePieces[pieceToUse], position, Quaternion.identity);
                piece.transform.SetParent(this.transform);
                piece.transform.localScale = Vector3.one * 0.1f;
                allPieces[x, y] = piece;
            }
        }

        StartCoroutine(CheckAndResolveMatches());
    }

    bool HasInitialMatch(int x, int y, GameObject piece)
    {
        if (x >= 2 && allPieces[x - 1, y] != null && allPieces[x - 2, y] != null)
        {
            if (allPieces[x - 1, y].tag == piece.tag && allPieces[x - 2, y].tag == piece.tag)
            {
                return true;
            }
        }
        if (y >= 2 && allPieces[x, y - 1] != null && allPieces[x, y - 2] != null)
        {
            if (allPieces[x, y - 1].tag == piece.tag && allPieces[x, y - 2].tag == piece.tag)
            {
                return true;
            }
        }
        return false;
    }

    public void SelectPiece(GamePiece piece)
    {
        if (selectedPiece == null)
        {
            selectedPiece = piece;
        }
        else
        {
            if (!isSwapping && IsAdjacent(selectedPiece, piece))
            {
                StartCoroutine(SwapPieces(selectedPiece, piece));
                selectedPiece = null;
            }
            else
            {
                selectedPiece = piece;
            }
        }
    }

    bool IsAdjacent(GamePiece piece1, GamePiece piece2)
    {
        Vector2Int pos1 = GetPieceIndex(piece1.transform.position);
        Vector2Int pos2 = GetPieceIndex(piece2.transform.position);

        return (Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y)) == 1;
    }

    IEnumerator SwapPieces(GamePiece piece1, GamePiece piece2)
    {
        isSwapping = true;

        Vector3 pos1 = piece1.transform.position;
        Vector3 pos2 = piece2.transform.position;

        piece1.transform.position = pos2;
        piece2.transform.position = pos1;

        Vector2Int posIndex1 = GetPieceIndex(pos1);
        Vector2Int posIndex2 = GetPieceIndex(pos2);

        allPieces[posIndex1.x, posIndex1.y] = piece2.gameObject;
        allPieces[posIndex2.x, posIndex2.y] = piece1.gameObject;

        yield return new WaitForSeconds(0.2f);

        if (HasMatches())
        {
            yield return StartCoroutine(CheckAndResolveMatches());
        }
        else
        {
            piece1.transform.position = pos1;
            piece2.transform.position = pos2;

            allPieces[posIndex1.x, posIndex1.y] = piece1.gameObject;
            allPieces[posIndex2.x, posIndex2.y] = piece2.gameObject;
        }

        isSwapping = false;
    }

    IEnumerator CheckAndResolveMatches()
    {
        bool foundMatches = false;

        while (HasMatches())
        {
            yield return new WaitForSeconds(0.2f);
            CheckMatches();
            foundMatches = true;
        }

        if (foundMatches)
        {
            yield return FillEmptySpaces();
        }
    }

    void CheckMatches()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allPieces[x, y] != null)
                {
                    if (x < width - 2 && allPieces[x, y].tag == allPieces[x + 1, y].tag && allPieces[x, y].tag == allPieces[x + 2, y].tag)
                    {
                        int matchLength = 3;
                        for (int i = x + 3; i < width && allPieces[x, y].tag == allPieces[i, y].tag; i++)
                        {
                            matchLength++;
                        }
                        DestroyMatchHorizontal(x, y, matchLength);
                    }
                    if (y < height - 2 && allPieces[x, y].tag == allPieces[x, y + 1].tag && allPieces[x, y].tag == allPieces[x, y + 2].tag)
                    {
                        int matchLength = 3;
                        for (int i = y + 3; i < height && allPieces[x, y].tag == allPieces[x, i].tag; i++)
                        {
                            matchLength++;
                        }
                        DestroyMatchVertical(x, y, matchLength);
                    }
                }
            }
        }
    }

    bool HasMatches()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allPieces[x, y] != null)
                {
                    if (x < width - 2 && allPieces[x, y].tag == allPieces[x + 1, y].tag && allPieces[x, y].tag == allPieces[x + 2, y].tag)
                    {
                        return true;
                    }
                    if (y < height - 2 && allPieces[x, y].tag == allPieces[x, y + 1].tag && allPieces[x, y].tag == allPieces[x, y + 2].tag)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    void DestroyMatchHorizontal(int startX, int y, int length)
    {
        for (int x = startX; x < startX + length; x++)
        {
            if (allPieces[x, y] != null)
            {
                allPieces[x, y].GetComponent<GamePiece>().DestroyPiece();
                allPieces[x, y] = null;
            }
        }

        UpdateScore(length);
    }

    void DestroyMatchVertical(int x, int startY, int length)
    {
        for (int y = startY; y < startY + length; y++)
        {
            if (allPieces[x, y] != null)
            {
                allPieces[x, y].GetComponent<GamePiece>().DestroyPiece();
                allPieces[x, y] = null;
            }
        }

        UpdateScore(length);
    }

    IEnumerator FillEmptySpaces()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allPieces[x, y] == null)
                {
                    for (int fillY = y; fillY < height - 1; fillY++)
                    {
                        allPieces[x, fillY] = allPieces[x, fillY + 1];
                        allPieces[x, fillY + 1] = null;
                        if (allPieces[x, fillY] != null)
                        {
                            allPieces[x, fillY].transform.position = new Vector2(x, fillY);
                        }
                    }
                    allPieces[x, height - 1] = Instantiate(gamePieces[Random.Range(0, gamePieces.Length)], new Vector2(x, height - 1), Quaternion.identity);
                    allPieces[x, height - 1].transform.SetParent(this.transform);
                    allPieces[x, height - 1].transform.localScale = Vector3.one * 0.1f;
                }
            }
        }

        yield return new WaitForSeconds(0.2f);
        StartCoroutine(CheckAndResolveMatches());
    }

    void UpdateScore(int points)
    {
        score += points;
        scoreText.text = "Score: " + score;
    }

    IEnumerator GameTimer()
    {
        while (gameTime > 0)
        {
            yield return null;
        }
        EndGame();
    }

    void EndGame()
    {
        endGamePanel.SetActive(true);
        endGameScoreText.text = "Your Score: " + score;

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            highScoreText.text = "New High Score: " + highScore;
        }
        else
        {
            highScoreText.text = "High Score: " + highScore;
        }

        Time.timeScale = 0;
    }

    Vector2Int GetPieceIndex(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x);
        int y = Mathf.RoundToInt(position.y);
        return new Vector2Int(x, y);
    }
}
