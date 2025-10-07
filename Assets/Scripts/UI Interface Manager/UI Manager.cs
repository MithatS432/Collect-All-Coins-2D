using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI References - Inspector'dan ata")]
    public TMP_Text coinsLeftText;
    public TMP_Text coinsDoneText;
    public TMP_Text arrowsLeftText;
    public Image healthBarImage;

    void Awake()
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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "MainMenu")
        {
            FindUIElements();

            // Soldier'ı bul ve UI'yı güncelle
            Soldier player = FindAnyObjectByType<Soldier>(); // Güncellenmiş metod
            if (player != null)
            {
                player.ForceUIUpdate(); // Bu metodu Soldier'a ekleyeceğiz
            }
        }
    }

    private void FindUIElements()
    {
        GameObject coinsLeftObj = GameObject.Find("CoinsLeftUI");
        GameObject coinsDoneObj = GameObject.Find("CoinsDoneUI");
        GameObject arrowsLeftObj = GameObject.Find("ArrowLeftUI");
        GameObject healthBarObj = GameObject.Find("HealthUI");

        if (coinsLeftObj != null) coinsLeftText = coinsLeftObj.GetComponent<TMP_Text>();
        if (coinsDoneObj != null) coinsDoneText = coinsDoneObj.GetComponent<TMP_Text>();
        if (arrowsLeftObj != null) arrowsLeftText = arrowsLeftObj.GetComponent<TMP_Text>();
        if (healthBarObj != null) healthBarImage = healthBarObj.GetComponent<Image>();

        Debug.Log("UI Elements Found - Coins: " + (coinsLeftText != null) +
                 ", Arrows: " + (arrowsLeftText != null) +
                 ", Health: " + (healthBarImage != null));
    }

    public void UpdateCoins(int coins)
    {
        if (coinsLeftText != null)
        {
            coinsLeftText.text = "Coins Left: " + coins;
            Debug.Log("Coins UI Updated: " + coins);
        }

        if (coinsDoneText != null)
        {
            coinsDoneText.gameObject.SetActive(coins <= 0);
        }
    }

    public void UpdateArrows(int arrows)
    {
        if (arrowsLeftText != null)
        {
            if (arrows <= 0)
                arrowsLeftText.text = "NONE";
            else
                arrowsLeftText.text = "Arrows Left: " + arrows;

            Debug.Log("Arrows UI Updated: " + arrows);
        }
    }

    public void UpdateHealth(float healthPercent)
    {
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = healthPercent;
            Debug.Log("Health UI Updated: " + healthPercent);
        }
    }
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}