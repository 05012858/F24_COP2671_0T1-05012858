using UnityEngine;

public class Coin : MonoBehaviour
{
    private Vector3 originalPosition;

    private void Start()
    {
        // Store the original position of the coin when it first spawns
        originalPosition = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.AddCoins(1); // Call GameManager to add coins
            CoinPool coinPool = FindObjectOfType<CoinPool>(); // Reference to the CoinPool
            coinPool.ReturnCoin(gameObject); // Return this coin to the pool
            Debug.Log("Coin Collected!");
        }
    }

    public void ResetPosition()
    {
        // Reset coin to its original position
        transform.position = originalPosition;
    }

    // Optionally, expose the original position for debugging or logic control
    public Vector3 GetOriginalPosition()
    {
        return originalPosition;
    }
}
