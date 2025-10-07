using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Header UI Elements")]
    public TextMeshProUGUI coinsLeftText;
    public TextMeshProUGUI arrowsLeftText;
    public Image healthBar;
    public GameObject coinsDoneUI;
    public GameObject backMenuUI;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.activeSceneChanged += OnSceneChanged;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        HandleVisibility();
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        HandleVisibility();
    }

    private void HandleVisibility()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }


    public void UpdateCoins(int coins)
    {
        if (coinsLeftText != null)
            coinsLeftText.text = "Coins Left: " + coins.ToString();

        if (coins <= 0 && coinsDoneUI != null)
        {
            coinsDoneUI.SetActive(true);
            Invoke(nameof(HideCoinsDoneUI), 1f);
        }
        else if (coinsDoneUI != null)
            coinsDoneUI.SetActive(false);
    }
    private void HideCoinsDoneUI()
    {
        if (coinsDoneUI != null)
            coinsDoneUI.SetActive(false);
    }

    public void UpdateArrows(int arrows)
    {
        if (arrowsLeftText != null)
            arrowsLeftText.text = arrows > 0 ? "Arrows Left: " + arrows.ToString() : "NONE";
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (healthBar != null)
            healthBar.fillAmount = currentHealth / maxHealth;
    }
    public void BackMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    public void ShowBackMenu(bool show)
    {
        if (backMenuUI != null)
            backMenuUI.SetActive(show);
    }
}
