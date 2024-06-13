using UnityEngine;

public class GamePiece : MonoBehaviour
{
    private GameBoard gameBoard;
    private Vector3 originalScale;

    void Start()
    {
        gameBoard = Object.FindFirstObjectByType<GameBoard>();
        originalScale = transform.localScale;
    }

    void OnMouseDown()
    {
        gameBoard.SelectPiece(this);
    }

    void OnMouseEnter()
    {
        transform.localScale = originalScale * 1.1f;
    }

    void OnMouseExit()
    {
        transform.localScale = originalScale;
    }

    public void DestroyPiece()
    {
        Destroy(gameObject);
    }
}


