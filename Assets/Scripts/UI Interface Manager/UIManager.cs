using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections; // Bunu eklemeyi unutma!

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("UI Elements")]
    public TextMeshProUGUI leftCoinsText;
    public TextMeshProUGUI coinsDoneText;
    public TextMeshProUGUI leftArrowsText;
    public Image healthBar;

    [Header("Pause Menu")]
    public GameObject pauseMenuCanvas;

    void Awake()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Destroy(gameObject);
            return;
        }

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.activeSceneChanged += OnSceneChanged;
            
            if (pauseMenuCanvas != null)
                pauseMenuCanvas.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        if (newScene.name == "MainMenu")
        {
            Destroy(gameObject);
            return;
        }
        FindPauseMenuInScene();
    }

    private void FindPauseMenuInScene()
    {
        pauseMenuCanvas = GameObject.Find("PauseMenuCanvas");
        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(false);
    }

    public void ShowPauseMenu()
    {
        if (pauseMenuCanvas == null)
            FindPauseMenuInScene();

        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(true);
    }

    public void HidePauseMenu()
    {
        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(false);
    }

    // COINS İLE İLGİLİ METODLARI EKLE:
    public void UpdateCoins(int coinsLeft)
    {
        if (leftCoinsText != null)
            leftCoinsText.text = "Coins Left: " + coinsLeft;
        
        if (coinsLeft <= 0 && coinsDoneText != null)
        {
            coinsDoneText.gameObject.SetActive(true);
            if (leftCoinsText != null) 
                leftCoinsText.gameObject.SetActive(false);
        }
    }

    // BU METODU EKLE:
    public void HideCoinsDoneText(float delay)
    {
        StartCoroutine(HideCoinsTextAfterDelay(delay));
    }

    // COROUTINE METODU:
    private IEnumerator HideCoinsTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (coinsDoneText != null)
            coinsDoneText.gameObject.SetActive(false);
    }

    public void UpdateArrows(int arrowsLeft)
    {
        if (leftArrowsText != null)
            leftArrowsText.text = arrowsLeft > 0 ? "Arrows Left: " + arrowsLeft : "None";
    }

    public void UpdateHealth(float fillAmount)
    {
        if (healthBar != null)
            healthBar.fillAmount = fillAmount;
    }
}