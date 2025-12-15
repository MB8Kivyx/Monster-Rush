using UnityEngine;

/// <summary>
///     The Player class handles the player's movement, interactions with obstacles and items, and player state management (alive or dead).
///     It controls the player's movement based on input, applies forces for acceleration and deceleration, and handles collisions.
/// </summary>
public class Player : MonoBehaviour {

    public static Player Instance { get; private set; } // Global reference for lightweight lookups

    [SerializeField] private GameObject gameManager; // Reference to the WaveGameManager


    [SerializeField] private float horizontalSpeed; // Speed of horizontal movement
    [SerializeField] private float verticalSpeed; // Speed of vertical movement
    [SerializeField] private int maxVerticalSpeed; // Maximum vertical speed
    [SerializeField] private int accelerationForce; // Force applied to accelerate upwards
    [SerializeField] private int decelerationForce; // Force applied to decelerate downwards

    [SerializeField] private GameObject deathEffect; // Effect displayed when the player dies
    [SerializeField] private GameObject colorChangeEffect; // Effect displayed when the player collects a color change item


    private bool isDead; // Boolean to track if the player is dead

    private float mapWidth; // Width of the map for movement boundaries
    private float movementAngle; // Angle used for side-to-side movement

    private Rigidbody2D rb; // Reference to the player's Rigidbody2D component


    private AudioSource audioSource; // Audio source for playing sound effects
    [SerializeField] private AudioClip deathClip; // Audio clip played when the player dies
    [SerializeField] private AudioClip itemClip; // Audio clip played when the player collects an item

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate Player detected. Destroying redundant instance.", this);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        mapWidth = gameManager.GetComponent<DisplayManager>().GetDisplayWidth();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        if (isDead) return;

        MovePlayer();
    }


    // Handles the player's movement
    private void MovePlayer()
    {
        Vector2 position = transform.position;

        // Move from side to side.
        position.x = Mathf.Cos(movementAngle) * (mapWidth * 0.45f);

        // Move forward gradually
        position.y += verticalSpeed * Time.deltaTime;

        transform.position = position;

        // Increase the movement angle for continuous horizontal movement
        movementAngle += Time.deltaTime * horizontalSpeed;

        // Apply force for vertical movement based on input
        if (Input.GetMouseButton(0))
        {
            if (rb.linearVelocity.y < maxVerticalSpeed) rb.AddForce(new Vector2(0, accelerationForce));
        }
        else
        {
            if (rb.linearVelocity.y > 0)
                rb.AddForce(new Vector2(0, -decelerationForce));
            else
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Item_ColorChange"))
        {
            // Display item effect and destroy after 0.5 seconds
            GameObject itemFxGameObject = Instantiate(colorChangeEffect, other.gameObject.transform.position, Quaternion.identity);
            Destroy(itemFxGameObject, 0.5f);

            // Destroy the parent game object of the collected item
            Destroy(other.gameObject.transform.parent.gameObject);

            // Change the background color
            BackgroundColorManager.Instance.ApplyBackgroundColor();

            // Add score
            ScoreManager.Instance.IncrementScore();

            // Play item collection sound
            audioSource.PlayOneShot(itemClip, 1);
        }

        if (other.gameObject.CompareTag("Obstacle") && isDead == false)
        {
            isDead = true;

            // Display death effect and destroy after 0.5 seconds
            GameObject deadFx = Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(deadFx, 0.5f);

            // Stop the player
            rb.linearVelocity = new Vector2(0, 0);
            rb.isKinematic = true;

            // Set game over
            gameManager.GetComponent<WaveGameManager>().GameOver();

            // Play death sound
            audioSource.PlayOneShot(deathClip, 1);
        }
    }

}
