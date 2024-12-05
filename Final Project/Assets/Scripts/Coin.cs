using UnityEngine;

public class Coin : MonoBehaviour
{
    private Vector3 originalPosition;
    public ParticleSystem coinCollectedParticles; // Reference to the particle system prefab

    private void Start()
    {
        // Store the original position of the coin when it first spawns
        originalPosition = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Trigger particle effect
            if (coinCollectedParticles != null)
            {
                ParticleSystem instantiatedParticles = Instantiate(coinCollectedParticles, transform.position, Quaternion.identity);
                instantiatedParticles.Play();
                Destroy(instantiatedParticles.gameObject, instantiatedParticles.main.duration);
                Debug.Log("Particle system instantiated at: " + transform.position);
            }
            else
            {
                Debug.LogWarning("coinCollectedParticles is not assigned!");
            }


            // Add coins and return the coin to the pool
            GameManager.instance.AddCoins(1);
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
