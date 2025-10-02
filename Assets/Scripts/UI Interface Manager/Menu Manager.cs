using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public GameObject playButton;
    public GameObject howToPlayButton;
    public GameObject exitButton;

    public GameObject panel;
    public GameObject closePanelButton;
    public TextMeshProUGUI titleText;

    public AudioClip buttonclickSound;

    public void OnPlayButtonClicked()
    {
        AudioSource.PlayClipAtPoint(buttonclickSound, Camera.main.transform.position);
        SceneManager.LoadScene("Field1");
    }

    public void OnHowToPlayButtonClicked()
    {
        AudioSource.PlayClipAtPoint(buttonclickSound, Camera.main.transform.position);
        panel.SetActive(true);
        closePanelButton.SetActive(true);
        titleText.gameObject.SetActive(true);
        playButton.SetActive(false);
        howToPlayButton.SetActive(false);
        exitButton.SetActive(false);
    }
    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void OnClosePanelButtonClicked()
    {
        AudioSource.PlayClipAtPoint(buttonclickSound, Camera.main.transform.position);
        panel.SetActive(false);
        closePanelButton.SetActive(false);
        titleText.gameObject.SetActive(false);
        playButton.SetActive(true);
        howToPlayButton.SetActive(true);
        exitButton.SetActive(true);
    }
}
