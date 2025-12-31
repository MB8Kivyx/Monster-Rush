using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>(); 
        if(gameManager == null) {
            Debug.LogError("PlayerCollision: GameManager not found in scene!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // For Trigger Colliders
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("PlayerCollision: Hit Trigger Obstacle: " + other.name);
            HandleDeath();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // For Solid Colliders
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("PlayerCollision: Hit Solid Obstacle: " + collision.gameObject.name);
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        // Stop player movement
        if (GetComponent<Rigidbody2D>() != null)
        {
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            GetComponent<Rigidbody2D>().angularVelocity = 0f;
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic; // Stop physics interactions
        }

        if (gameManager != null)
        {
            Debug.Log("PlayerCollision: Calling GameOver on GameManager.");
            gameManager.GameOver();
        }
        else
        {
            Debug.LogError("PlayerCollision: Cannot call GameOver - GameManager is null!");
        }
    }
}