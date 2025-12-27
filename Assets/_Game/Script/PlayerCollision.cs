using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // When player touches OBSTACLE
        if (other.CompareTag("Obstacle"))
        {
            // Stop player movement (optional, Time.timeScale will pause anyway)
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

            if (gameManager != null)
            {
                gameManager.GameOver();
            }
        }
    }
}