

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The ObstacleManager class is responsible for spawning obstacles at regular intervals based on the player's position and score.
/// It manages two sets of obstacles (easy and normal) and increases the difficulty of obstacles as the player's score increases.
/// </summary>
public class ObstacleManager : MonoBehaviour
{

    [SerializeField] private GameObject player;
    
    // obstacle array 
    [SerializeField] private GameObject[] easyObstacleArray;
    [SerializeField] private bool disableObstaclesForTesting = false; // Skip obstacle spawning when enabled
    [SerializeField] private float distanceBetweenObstacles = 65f;
    [SerializeField] private float safeZoneDistance = 40f; 
    [SerializeField] private float spawnDelay = 1.5f; 

    private float lastSpawnY; 
    private float spawnTimer = 0f;
    private bool canSpawn = false;

    private void Start()
    {
        // 1. Force the first spawn to be beyond the safe zone
        if (player != null)
        {
            lastSpawnY = player.transform.position.y + safeZoneDistance - distanceBetweenObstacles;
        }
        else
        {
            lastSpawnY = -distanceBetweenObstacles;
        }

        // 2. Set the delay timer
        spawnTimer = spawnDelay;
        canSpawn = false;
    }

    private void Update()
    {
        if (player == null) return;

        // 3. Spawning Delay Logic
        if (!canSpawn)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0)
            {
                canSpawn = true;
            }
            return; // Don't spawn yet
        }

        // Spawn a new obstacle when the player gets close to the last spawn point
        // Keeping obstacles 120 units ahead of the player
        if (player.transform.position.y + 120f > lastSpawnY + distanceBetweenObstacles)
        {
            SpawnObstacle();
        }
    }

    private void SpawnObstacle()
    {
        if (disableObstaclesForTesting) return;

        int score = ScoreManager.Instance.GetScore();
        
        // Difficulty Logic
        if (score < 20)
        {
            int randomIndex = Random.Range(0, easyObstacleArray.Length);
            InstantiateObstaclePrefab(easyObstacleArray[randomIndex]);
        }
        else
        {
            // Fallback for higher scores if normalObstacleArray is ever added
            int randomIndex = Random.Range(0, easyObstacleArray.Length);
            InstantiateObstaclePrefab(easyObstacleArray[randomIndex]);
        }
    }

    private void InstantiateObstaclePrefab(GameObject obstaclePrefab)
    {
        if (!obstaclePrefab) return;

        float spawnY = lastSpawnY + distanceBetweenObstacles;
        GameObject newObstacle = Instantiate(obstaclePrefab, new Vector3(0, spawnY, 0), Quaternion.identity);
        newObstacle.transform.SetParent(transform);
        lastSpawnY = spawnY;
    }
}

