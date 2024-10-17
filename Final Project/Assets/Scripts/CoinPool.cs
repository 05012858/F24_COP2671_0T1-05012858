using System.Collections.Generic;
using UnityEngine;

public class CoinPool : MonoBehaviour
{
    public GameObject coinPrefab; // Assign your coin prefab here
    public int poolSize = 20; // Number of coins to pool
    public Transform[] spawnPoints; // List of spawn points for coins

    public List<GameObject> coins; // Made public for accessibility
    public Dictionary<GameObject, Vector3> coinPositions; // Made public for accessibility

    private void Start()
    {
        coins = new List<GameObject>();
        coinPositions = new Dictionary<GameObject, Vector3>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject coin = Instantiate(coinPrefab);
            coin.SetActive(false); // Start as inactive

            // Assign a spawn position to the coin from the list of spawnPoints
            Vector3 spawnPosition = spawnPoints[i % spawnPoints.Length].position; // Loop over available spawn points
            coin.transform.position = spawnPosition;
            coinPositions.Add(coin, spawnPosition); // Store the original spawn position

            coins.Add(coin);
        }
    }

    public GameObject GetCoin()
    {
        foreach (var coin in coins)
        {
            if (!coin.activeInHierarchy)
            {
                coin.SetActive(true); // Activate the coin
                coin.transform.position = coinPositions[coin]; // Reset to its original position
                return coin;
            }
        }
        return null; // No available coin
    }

    public void ReturnCoin(GameObject coin)
    {
        coin.SetActive(false); // Deactivate the coin
    }

    public void ReactivateCoins()
    {
        foreach (var coin in coins)
        {
            if (!coin.activeInHierarchy)
            {
                coin.transform.position = coinPositions[coin]; // Reactivate at its original position
                coin.SetActive(true); // Activate the coin
            }
        }
    }
}
