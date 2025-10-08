using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI References")]
    public TMP_Text coinsLeftText;
    public TMP_Text coinsDoneText;
    public TMP_Text arrowsLeftText;
    public Image healthBarImage;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            gameObject.SetActive(false); // Canvas gizlenir
        }
        else
        {
            gameObject.SetActive(true); // Oyun sahnelerinde görünür
            FindUIElements();

            Soldier player = FindAnyObjectByType<Soldier>();
            if (player != null)
                player.ForceUIUpdate();
        }
    }

    private void FindUIElements()
    {
        coinsLeftText = GameObject.Find("CoinsLeftUI")?.GetComponent<TMP_Text>();
        coinsDoneText = GameObject.Find("CoinsDoneUI")?.GetComponent<TMP_Text>();
        arrowsLeftText = GameObject.Find("ArrowLeftUI")?.GetComponent<TMP_Text>();
        healthBarImage = GameObject.Find("HealthUI")?.GetComponent<Image>();
    }

    public void UpdateCoins(int coins)
    {
        if (coinsLeftText != null)
            coinsLeftText.text = "Coins Left: " + coins;

        if (coinsDoneText != null)
            coinsDoneText.gameObject.SetActive(coins <= 0);
    }

    public void UpdateArrows(int arrows)
    {
        if (arrowsLeftText != null)
            arrowsLeftText.text = arrows > 0 ? $"Arrows Left: {arrows}" : "NONE";
    }

    public void UpdateHealth(float healthPercent)
    {
        if (healthBarImage != null)
            healthBarImage.fillAmount = healthPercent;
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
