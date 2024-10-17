using System.Collections;
using TMPro;
using UnityEngine;

// Move respective code into ShopManager
public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton instance
    public int playerCoins = 0; // Player's coin count
    public int totalCoins = 0; // Total coins in the level
    public int playerTime = 1;   // Default player time
    public TMP_Text coinText;     // Reference to the UI Text for coins
    public TMP_Text shopCoinText; // Reference to the UI Text for coins in the shop
    public TMP_Text playerTimeText;
    public TMP_Text priceTimerText;    // Reference to the UI Text for price
    public TMP_Text priceSpeedText;
    public TMP_Text priceJumpText;
    public GameObject mainMenuCanvas;
    public GameObject tutorialCanvas;
    public GameObject mainGameCanvas;
    public GameObject shopCanvas;
    public GameObject youLoseCanvas;
    public AudioSource gameOverMusic;
    public AudioSource backgroundMusic;
    public AudioSource shopMusic;
    public AudioSource coinSound; // Coin sound reference

    // Reference to the player's transform
    public Transform player; // Assign in the Inspector
    private Vector3 originalPlayerPosition;

    // Store the additional time purchased by the player
    private int additionalTime = 0; // Track additional time
    private int currentPrice = 5;    // Initial price for buying time
    private int currentPriceJump = 3;

    // Player speed upgrade variables
    private int speedUpgradeCost = 10; // Initial cost for speed upgrade
    private int speedUpgradeIncrement = 5; // Increment for next speed upgrade cost
    public float playerSpeed = 5f; // Base player speed

    public float jumpPower = 32.0f;

    private void Awake()
    {
        // Ensure there's only one instance of GameManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keep GameManager across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate GameManager
        }
    }

    void Start()
    {
        ShowMainMenu();
        originalPlayerPosition = player.position; // Store original position
        UpdateCoinUI(); // Update UI initially
        UpdatePriceUI(); // Update price UI initially
    }

    private void Update()
    {
        CheckWinCondition();


    }

    public void YouLost()
    {
        StopAllCoroutines();
        mainMenuCanvas.SetActive(false);
        tutorialCanvas.SetActive(false);
        mainGameCanvas.SetActive(false);
        shopCanvas.SetActive(false);
        youLoseCanvas.SetActive(true);


        backgroundMusic.Stop();
        gameOverMusic.Play();



    }

    public void ShowMainMenu()
    {
        CoinPool coinPool = FindObjectOfType<CoinPool>();
        if (coinPool != null)
        {
            foreach (GameObject coin in coinPool.coins)
            {
                if (coin.activeInHierarchy)
                {
                    coin.SetActive(false);
                }
            }
        }

        backgroundMusic.Play();
        mainMenuCanvas.SetActive(true);
        tutorialCanvas.SetActive(false);
        mainGameCanvas.SetActive(false);
        shopCanvas.SetActive(false);
        youLoseCanvas.SetActive(false);
    }

    public void StartTutorial()
    {
        player.position = originalPlayerPosition;

        mainMenuCanvas.SetActive(false);
        tutorialCanvas.SetActive(true);
        mainGameCanvas.SetActive(false);
        shopCanvas.SetActive(false);
        youLoseCanvas.SetActive(false);
    }

    public void StartGame()
    {
        shopMusic.Stop();
        backgroundMusic.Play();

        mainMenuCanvas.SetActive(false);
        tutorialCanvas.SetActive(false);
        mainGameCanvas.SetActive(true);
        shopCanvas.SetActive(false);
        youLoseCanvas.SetActive(false);

        player.position = originalPlayerPosition; // Reset player's position
        playerTime = 1 + additionalTime; // Set player time to default + additional time
        StartCoroutine(CountdownTimer());

        if (PlayerMovement.instance.playerHearts <= 0)
        {
            YouLost();
        }

    }

    public void GoToShop()
    {
        CameraFollow.instance.ResetCamera();

        mainMenuCanvas.SetActive(false);
        tutorialCanvas.SetActive(false);
        mainGameCanvas.SetActive(false);
        shopCanvas.SetActive(true);
        youLoseCanvas.SetActive(false);

        shopMusic.Play();
        backgroundMusic.Stop();

        // Deactivate all active coins when entering the shop
        CoinPool coinPool = FindObjectOfType<CoinPool>();
        if (coinPool != null)
        {
            foreach (GameObject coin in coinPool.coins)
            {
                if (coin.activeInHierarchy)
                {
                    coin.SetActive(false);
                }
            }
        }

        // Update the shop's coin UI when entering the shop
        shopCoinText.text = "Coins: " + playerCoins.ToString(); // Update the shop coin UI

        UpdatePriceUI(); // Update the price UI when going to the shop
    }

    public void ContinueGame()
    {
        CameraFollow.instance.ResetCamera();

        // Reset playerTime to 5 + additional time purchased
        playerTime = 5 + additionalTime;

        // Reactivate coins when continuing the game
        ReactivateCoins();

        StartGame(); // Start the game from the original position
    }

    public void AddCoins(int amount)
    {
        playerCoins += amount; // Increase coin count
        totalCoins += amount; // Keep track of total coins collected
        UpdateCoinUI(); // Update the main UI
        shopCoinText.text = "Coins: " + playerCoins.ToString(); // Update the shop UI if it's active
        coinSound.Play(); // Play coin sound effect

        // Check for win condition
        CheckWinCondition();
    }

    private void UpdateCoinUI()
    {
        coinText.text = "Coins: " + playerCoins.ToString(); // Update the UI Text
    }

    private void UpdatePriceUI()
    {
        // Update the price for the timer
        priceTimerText.text = "Price: " + currentPrice.ToString();

        // Update the price for the jump upgrade
        priceJumpText.text = "Price: " + currentPriceJump.ToString();

        // Update the price for the speed upgrade
        priceSpeedText.text = "Price: " + speedUpgradeCost.ToString();
    }

    private IEnumerator CountdownTimer()
    {
        while (playerTime > 0)
        {
            playerTimeText.text = "Time: " + playerTime.ToString();
            yield return new WaitForSeconds(1);
            playerTime--;
        }


        GoToShop(); // Go to shop when time runs out
    }

    public void BuyTimeButton()
    {
        if (playerCoins >= currentPrice)
        {
            additionalTime += 2; // Add 2 seconds to additional time
            playerCoins -= currentPrice; // Deduct the current price from coins
            UpdateCoinUI(); // Update coin UI
            shopCoinText.text = "Coins: " + playerCoins.ToString(); // Update the shop UI
            UpdatePriceUI(); // Update price UI

            currentPrice += 5; // Increase the price by 5 for the next purchase
        }
        else
        {
            Debug.Log("You don't have enough coins for this");
        }
    }

    public void BuyJumpButton()
    {
        if (playerCoins >= currentPriceJump)
        {
            jumpPower += 2; // Increase the player's jump power by 5
            playerCoins -= currentPriceJump; // Deduct the cost of the upgrade
            currentPriceJump += 6; // Increase the cost for the next jump upgrade

            UpdateCoinUI(); // Refresh the coin display
            shopCoinText.text = "Coins: " + playerCoins.ToString(); // Update the shop UI
            UpdatePriceUI(); // Update price UI

            Debug.Log("Jump power upgraded! Current jump power: " + jumpPower);
        }
        else
        {
            Debug.Log("Not enough coins to upgrade jump.");
        }
    }

    public void ReactivateCoins() // Reactivate all coins from the pool
    {
        CoinPool coinPool = FindObjectOfType<CoinPool>();

        foreach (GameObject coin in coinPool.coins)
        {
            if (!coin.activeInHierarchy)
            {
                coin.SetActive(true); // Reactivate each coin
            }
        }
    }

    public void UpgradePlayerSpeed()
    {
        if (playerCoins >= speedUpgradeCost)
        {
            playerSpeed += 1; // Increase player speed by 2
            playerCoins -= speedUpgradeCost; // Deduct the upgrade cost
            speedUpgradeCost += speedUpgradeIncrement; // Increment cost for next upgrade

            UpdateCoinUI(); // Refresh coin display
            shopCoinText.text = "Coins: " + playerCoins.ToString(); // Update the shop UI
            UpdatePriceUI();

            Debug.Log("Player speed upgraded! Current speed: " + playerSpeed);
        }
        else
        {
            Debug.Log("Not enough coins to upgrade speed.");
        }
    }

    // Implement Health System
    // Implement Reward System
    //      *     Rewarded upgrades are based on how fast you finish the level
    // Implement Shooting System


    private void CheckWinCondition()
    {
        if (playerCoins >= totalCoins)
        {
            WinGame();
        }
    }

    private void WinGame()
    {
        Debug.Log("All coins collected! You win!");
        // Implement your win condition logic here, like loading a win scene or showing a win UI
    }
}
