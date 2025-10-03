using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Field1MenuManager : MonoBehaviour
{
    public Button continueButton;
    public Button mainMenuButton;
    public void PauseGame()
    {
        Time.timeScale = 0f;
        continueButton.gameObject.SetActive(true);
        mainMenuButton.gameObject.SetActive(true);
    }
    public void ContinueGame()
    {
        Time.timeScale = 1f;
        continueButton.gameObject.SetActive(false);
        mainMenuButton.gameObject.SetActive(false);
    }
    public void BackMainMenu()
    {
        continueButton.gameObject.SetActive(false);
        mainMenuButton.gameObject.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
