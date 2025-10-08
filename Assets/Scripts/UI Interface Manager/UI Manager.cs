using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

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
        // Tüm alt nesneleri tarar
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
        foreach (TMP_Text text in texts)
        {
            switch (text.name)
            {
                case "CoinsLeftUI":
                    coinsLeftText = text;
                    break;
                case "CoinsDoneUI":
                    coinsDoneText = text;
                    break;
                case "ArrowLeftUI":
                    arrowsLeftText = text;
                    break;
            }
        }

        Image[] images = GetComponentsInChildren<Image>(true);
        foreach (Image img in images)
        {
            if (img.name == "HealthUI")
                healthBarImage = img;
        }

        // Debug log, null kalmadığından emin olmak için
        Debug.Log("coinsLeftText: " + coinsLeftText);
        Debug.Log("coinsDoneText: " + coinsDoneText);
        Debug.Log("arrowsLeftText: " + arrowsLeftText);
        Debug.Log("healthBarImage: " + healthBarImage);
    }

    public void UpdateCoins(int coins)
    {
        if (coinsLeftText != null)
            coinsLeftText.text = "Coins Left: " + coins;

        if (coinsDoneText != null)
        {
            bool showDone = coins <= 0;
            coinsDoneText.gameObject.SetActive(showDone);

            if (showDone)
                StartCoroutine(HideCoinsDoneAfterDelay(1f)); // 1 saniye sonra gizle
        }
    }

    private IEnumerator HideCoinsDoneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (coinsDoneText != null)
            coinsDoneText.gameObject.SetActive(false);
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
