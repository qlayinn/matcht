using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitConfirmation : MonoBehaviour
{
    public void ConfirmExit()
    {
        Application.Quit();
    }

    public void CancelExit()
    {
        SceneManager.LoadScene("MainMenu");
    }
}