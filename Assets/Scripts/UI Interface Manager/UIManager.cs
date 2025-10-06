using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("UI Elements")]
    public TextMeshProUGUI leftCoinsText;
    public TextMeshProUGUI coinsDoneText;
    public TextMeshProUGUI leftArrowsText;
    public Button continueButton;
    public Button mainMenuButton;
    public Image healthBar;

    [Header("Pause Menu")]
    public GameObject pauseMenuCanvas;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        // Pause menü veya UI sadece oyun sahnelerinde aktif olsun
        if (newScene.name == "MainMenu")
        {
            if (pauseMenuCanvas != null) pauseMenuCanvas.SetActive(false);
            gameObject.SetActive(false); // UIManager MainMenu’de gözükmesin
            Time.timeScale = 1f; // Oyun hızını resetle
        }
        else
        {
            gameObject.SetActive(true);
            Time.timeScale = 1f; 
            FindPauseMenuInScene();
        }
    }

    private void FindPauseMenuInScene()
    {
        if (pauseMenuCanvas == null)
            pauseMenuCanvas = GameObject.Find("PauseMenuCanvas");

        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(false);
            SetupButtonEvents();
        }
    }

    private void SetupButtonEvents()
    {
        if (pauseMenuCanvas == null) return;

        Button continueBtn = pauseMenuCanvas.transform.Find("ContinueButton")?.GetComponent<Button>();
        if (continueBtn != null)
        {
            continueBtn.onClick.RemoveAllListeners();
            continueBtn.onClick.AddListener(ContinueGame);
        }

        Button mainMenuBtn = pauseMenuCanvas.transform.Find("MainMenuButton")?.GetComponent<Button>();
        if (mainMenuBtn != null)
        {
            mainMenuBtn.onClick.RemoveAllListeners();
            mainMenuBtn.onClick.AddListener(BackMainMenu);
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        if (pauseMenuCanvas != null) pauseMenuCanvas.SetActive(true);
    }

    public void ContinueGame()
    {
        Time.timeScale = 1f;
        if (pauseMenuCanvas != null) pauseMenuCanvas.SetActive(false);
    }

    public void BackMainMenu()
    {
        Time.timeScale = 1f;
        if (pauseMenuCanvas != null) pauseMenuCanvas.SetActive(false);
        SceneManager.LoadScene("MainMenu");
    }

    public void UpdateCoins(int coinsLeft)
    {
        if (leftCoinsText != null) leftCoinsText.text = "Coins Left: " + coinsLeft;
        if (coinsLeft <= 0 && coinsDoneText != null)
        {
            coinsDoneText.gameObject.SetActive(true);
            if (leftCoinsText != null) leftCoinsText.gameObject.SetActive(false);
        }
    }

    public void HideCoinsDoneText(float delay) => StartCoroutine(HideCoinsTextAfterDelay(delay));

    private IEnumerator HideCoinsTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (coinsDoneText != null) coinsDoneText.gameObject.SetActive(false);
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
