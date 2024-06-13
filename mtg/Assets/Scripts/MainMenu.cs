using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{   
    public GameObject highScorePanel; // Панель для отображения рекордов
    public TMP_Text highScoreText; // Текст для отображения рекорда

    private void Start()
    {
        // Убедитесь, что панель рекордов неактивна при старте
        if (highScorePanel != null)
        {
            highScorePanel.SetActive(false);
        }
        else
        {
            Debug.LogError("HighScorePanel is not assigned in the inspector.");
        }
    }

    public void PlayGame()
    {
        Debug.Log("Play button clicked"); // Отладочное сообщение
        SceneManager.LoadScene("GameScene");
    }

    public void ShowHighScores()
    {
        if (highScorePanel != null && highScoreText != null)
        {
            // Отобразите панель рекордов
            highScorePanel.SetActive(true);

            // Обновите текст с рекордом
            int highScore = PlayerPrefs.GetInt("HighScore", 0);
            highScoreText.text = "High Score: " + highScore;
        }
        else
        {
            Debug.LogError("HighScorePanel or HighScoreText is not assigned in the inspector.");
        }
    }

    public void CloseHighScores()
    {
        if (highScorePanel != null)
        {
            // Скрыть панель рекордов
            highScorePanel.SetActive(false);
        }
        else
        {
            Debug.LogError("HighScorePanel is not assigned in the inspector.");
        }
    }

    public void ExitGame()
    {
        Debug.Log("Exit button clicked"); // Отладочное сообщение
        SceneManager.LoadScene("ExitConfirmation");
    }
}
