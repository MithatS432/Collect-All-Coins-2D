using UnityEngine;
using UnityEngine.SceneManagement;

public class Field1MenuManager : MonoBehaviour
{
    public void PauseGame()
    {
        Time.timeScale = 0f;
        UIManager.instance.ShowPauseMenu(); 
    }

    public void ContinueGame()
    {
        Time.timeScale = 1f; 
        UIManager.instance.HidePauseMenu(); 
    }

    public void BackMainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu");
    }
}