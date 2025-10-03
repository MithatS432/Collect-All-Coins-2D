using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("UI Elements")]
    public TextMeshProUGUI leftCoinsText;
    public TextMeshProUGUI coinsDoneText;
    public TextMeshProUGUI leftArrowsText;
    public Image healthBar;

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
        }
    }

    public void UpdateCoins(int coinsLeft)
    {
        leftCoinsText.text = "Coins Left: " + coinsLeft;
        if (coinsLeft <= 0)
        {
            coinsDoneText.gameObject.SetActive(true);
            leftCoinsText.gameObject.SetActive(false);
        }
    }

    public void UpdateArrows(int arrowsLeft)
    {
        leftArrowsText.text = arrowsLeft > 0 ? "Arrows Left: " + arrowsLeft : "None";
    }

    public void UpdateHealth(float fillAmount)
    {
        healthBar.fillAmount = fillAmount;
    }

    public void HideCoinsDoneText(float delay)
    {
        Invoke(nameof(HideCoinsText), delay);
    }

    private void HideCoinsText()
    {
        coinsDoneText.gameObject.SetActive(false);
    }
}
