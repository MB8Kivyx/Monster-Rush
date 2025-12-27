

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
    
    private int obstacleIndex = 0; // Index to track the number of obstacles instantiated
    private int distanceBetweenObstacles = 65; // Distance between consecutive obstacles

    private int playerPositionCheckpoint = -1; // Checkpoint to track player's position for obstacle instantiation
    private bool bubbleTesseractSpawned;


    private void Start()
    {
        SpawnObstacle();
    }

    private void Update()
    {
        // When the player's y-position increases, spawn a new obstacle
        int currentCheckpoint = (int)player.transform.position.y / 25;
        
        if (playerPositionCheckpoint != currentCheckpoint)
        {
            SpawnObstacle();
            playerPositionCheckpoint = currentCheckpoint;
        }
    }


    // Spawns an obstacle based on the player's score
    private void SpawnObstacle()
    {
        if (disableObstaclesForTesting)
        {
            obstacleIndex++;
            return;
        }

        if (!bubbleTesseractSpawned)
        {
            GameObject forcedPrefab = FindBubbleTesseractInArray(easyObstacleArray);
            if (forcedPrefab)
            {
                InstantiateObstaclePrefab(forcedPrefab);
                bubbleTesseractSpawned = true;
                obstacleIndex++;
                return;
            }

            bubbleTesseractSpawned = true;
        }

        int score = ScoreManager.Instance.GetScore(); // Get the current score from the ScoreManager
        
        // Choose obstacle difficulty based on score
        if (score < 20)
        {
            int randomIndex = Random.Range(0, easyObstacleArray.Length);
            InstantiateObstaclePrefab(easyObstacleArray[randomIndex]);
        }
       

        obstacleIndex++;
    }

    private void InstantiateObstaclePrefab(GameObject obstaclePrefab)
    {
        if (!obstaclePrefab)
        {
            Debug.LogWarning("ObstacleManager attempted to spawn a null obstacle prefab.", this);
            return;
        }

        GameObject newObstacle = Instantiate(obstaclePrefab, new Vector3(0, obstacleIndex * distanceBetweenObstacles), Quaternion.identity);
        newObstacle.transform.SetParent(transform);
    }

    
    private static GameObject FindBubbleTesseractInArray(GameObject[] obstacleArray)
    {
        if (obstacleArray == null) return null;

        foreach (GameObject obstacle in obstacleArray)
        {
            if (IsBubbleTesseractPrefab(obstacle)) return obstacle;
        }

        return null;
    }

    private static bool IsBubbleTesseractPrefab(GameObject obstacle)
    {
        return obstacle && obstacle.name == "BT_Soft4D";
    }

}

