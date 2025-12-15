using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The ObstacleParent class periodically checks the distance between the player and the obstacle.
/// If the obstacle is too far from the player, it destroys itself to optimize game performance.
/// </summary>
public class ObstacleParent : MonoBehaviour
{

    [SerializeField] private Player player; // Optional reference to the player component
    [SerializeField] private float cullDistance = 60f; // Distance behind the player before an obstacle is culled

    private static readonly WaitForSeconds CheckInterval = new WaitForSeconds(1f);

    private Transform playerTransform;

    private void Awake()
    {
        CachePlayerTransform(player);
    }

    
    private void Start()
    {
        if (playerTransform == null)
        {
            CachePlayerTransform(Player.Instance);
        }

        if (playerTransform == null)
        {
            GameObject fallback = GameObject.Find("player");
            if (fallback != null)
            {
                playerTransform = fallback.transform;
            }
        }

        if (playerTransform == null)
        {
            Debug.LogWarning("ObstacleParent could not locate a player transform. Disabling culling.", this);
            enabled = false;
            return;
        }

		StartCoroutine(CheckDistanceToPlayer()); // Start the distance check coroutine
    }
    

    // Coroutine to periodically check the distance between the player and this obstacle
    private IEnumerator CheckDistanceToPlayer()
    {
        while (true)
        {
            if (playerTransform == null)
            {
                yield break;
            }

            // If the obstacle is more than 60 units behind the player, destroy the obstacle
            if (playerTransform.position.y - transform.position.y > cullDistance)
            {
                Destroy(gameObject);
                yield break;
            }
            yield return CheckInterval;
        }
    }

    private void CachePlayerTransform(Player target)
    {
        if (target != null)
        {
            playerTransform = target.transform;
        }
    }


}
