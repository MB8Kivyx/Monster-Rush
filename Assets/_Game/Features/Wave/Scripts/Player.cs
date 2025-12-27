// using UnityEngine;

// /// <summary>
// ///     The Player class handles the player's movement, interactions with obstacles and items, and player state management (alive or dead).
// ///     It controls the player's movement based on input, applies forces for acceleration and deceleration, and handles collisions.
// /// </summary>
// public class Player : MonoBehaviour {

//     public static Player Instance { get; private set; } // Global reference for lightweight lookups

//     [SerializeField] private GameObject gameManager; // Reference to the GameManager


//     [SerializeField] private float horizontalSpeed; // Speed of horizontal movement
//     [SerializeField] private float verticalSpeed; // Speed of vertical movement
//     [SerializeField] private int maxVerticalSpeed; // Maximum vertical speed
//     [SerializeField] private int accelerationForce; // Force applied to accelerate upwards
//     [SerializeField] private int decelerationForce; // Force applied to decelerate downwards

//     [SerializeField] private GameObject deathEffect; // Effect displayed when the player dies
//     [SerializeField] private GameObject colorChangeEffect; // Effect displayed when the player collects a color change item


//     private bool isDead; // Boolean to track if the player is dead

//     private float mapWidth; // Width of the map for movement boundaries
//     private float movementAngle; // Angle used for side-to-side movement

//     private Rigidbody2D rb; // Reference to the player's Rigidbody2D component


//     private AudioSource audioSource; // Audio source for playing sound effects
//     [SerializeField] private AudioClip deathClip; // Audio clip played when the player dies
//     [SerializeField] private AudioClip itemClip; // Audio clip played when the player collects an item

//     private void Awake()
//     {
//         if (Instance != null && Instance != this)
//         {
//             Debug.LogWarning("Duplicate Player detected. Destroying redundant instance.", this);
//             Destroy(gameObject);
//             return;
//         }

//         Instance = this;
//     }

//     private void Start()
//     {
//         rb = GetComponent<Rigidbody2D>();
//         audioSource = GetComponent<AudioSource>();

//         mapWidth = gameManager.GetComponent<DisplayManager>().GetDisplayWidth();
//     }

//     private void OnDestroy()
//     {
//         if (Instance == this)
//         {
//             Instance = null;
//         }
//     }

//     private void Update()
//     {
//         if (isDead) return;

//         MovePlayer();
//     }


//     // Handles the player's movement
//     private void MovePlayer()
//     {
//         Vector2 position = transform.position;

//         // Move from side to side.
//         position.x = Mathf.Cos(movementAngle) * (mapWidth * 0.45f);

//         // Move forward gradually
//         position.y += verticalSpeed * Time.deltaTime;

//         transform.position = position;

//         // Increase the movement angle for continuous horizontal movement
//         movementAngle += Time.deltaTime * horizontalSpeed;

//         // Apply force for vertical movement based on input
//         if (Input.GetMouseButton(0))
//         {
//             if (rb.linearVelocity.y < maxVerticalSpeed) rb.AddForce(new Vector2(0, accelerationForce));
//         }
//         else
//         {
//             if (rb.linearVelocity.y > 0)
//                 rb.AddForce(new Vector2(0, -decelerationForce));
//             else
//                 rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
//         }
//     }

//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         if (other.gameObject.CompareTag("Item_ColorChange"))
//         {
//             // Display item effect and destroy after 0.5 seconds
//             GameObject itemFxGameObject = Instantiate(colorChangeEffect, other.gameObject.transform.position, Quaternion.identity);
//             Destroy(itemFxGameObject, 0.5f);

//             // Destroy the parent game object of the collected item
//             Destroy(other.gameObject.transform.parent.gameObject);


//             // Add score
//             ScoreManager.Instance.IncrementScore();

//             // Play item collection sound
//             audioSource.PlayOneShot(itemClip, 1);
//         }

//         if (other.gameObject.CompareTag("Obstacle") && isDead == false)
//         {
//             isDead = true;

//             // Display death effect and destroy after 0.5 seconds
//             GameObject deadFx = Instantiate(deathEffect, transform.position, Quaternion.identity);
//             Destroy(deadFx, 0.5f);

//             // Stop the player
//             rb.linearVelocity = new Vector2(0, 0);
//             rb.isKinematic = true;

//             // Set game over
//             gameManager.GetComponent<GameManager>().GameOver();

//             // Play death sound
//             audioSource.PlayOneShot(deathClip, 1);
//         }
//     }

// }









// using UnityEngine;

// public class Player : MonoBehaviour {

//     public static Player Instance { get; private set; }

//     [SerializeField] private GameObject gameManager;

//     [SerializeField] private float horizontalSpeed;
//     [SerializeField] private float verticalSpeed;
//     [SerializeField] private int maxVerticalSpeed;
//     [SerializeField] private int accelerationForce;
//     [SerializeField] private int decelerationForce;

//     [SerializeField] private GameObject deathEffect;
//     [SerializeField] private GameObject colorChangeEffect;

//     private bool isDead;
//     private float mapWidth;
//     private float movementAngle;
//     private Rigidbody2D rb;

//     private AudioSource audioSource;
//     [SerializeField] private AudioClip deathClip;
//     [SerializeField] private AudioClip itemClip;

//     private Collider2D playerCollider;

//     private void Awake()
//     {
//         if (Instance != null && Instance != this)
//         {
//             Destroy(gameObject);
//             return;
//         }
//         Instance = this;
//     }

//     private void Start()
//     {
//         rb = GetComponent<Rigidbody2D>();
//         audioSource = GetComponent<AudioSource>();
//         playerCollider = GetComponent<Collider2D>();
//         mapWidth = gameManager.GetComponent<DisplayManager>().GetDisplayWidth();
//     }

//     private void OnDestroy()
//     {
//         if (Instance == this)
//             Instance = null;
//     }

//     private void Update()
//     {
//         if (isDead) return;
//         MovePlayer();
//     }

//     private void MovePlayer()
//     {
//         Vector2 position = transform.position;
//         position.x = Mathf.Cos(movementAngle) * (mapWidth * 0.45f);
//         position.y += verticalSpeed * Time.deltaTime;
//         transform.position = position;
//         movementAngle += Time.deltaTime * horizontalSpeed;

//         if (Input.GetMouseButton(0))
//         {
//             if (rb.linearVelocity.y < maxVerticalSpeed) rb.AddForce(new Vector2(0, accelerationForce));
//         }
//         else
//         {
//             if (rb.linearVelocity.y > 0)
//                 rb.AddForce(new Vector2(0, -decelerationForce));
//             else
//                 rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
//         }
//     }

//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         if (other.gameObject.CompareTag("Item_ColorChange"))
//         {
//             GameObject fx = Instantiate(colorChangeEffect, other.transform.position, Quaternion.identity);
//             Destroy(fx, 0.5f);
//             Destroy(other.gameObject.transform.parent.gameObject);
//             ScoreManager.Instance.IncrementScore();
//             audioSource.PlayOneShot(itemClip, 1);
//         }

//         if (other.gameObject.CompareTag("Obstacle") && !isDead)
//         {
//             isDead = true;
//             GameObject fx = Instantiate(deathEffect, transform.position, Quaternion.identity);
//             Destroy(fx, 0.5f);
//             rb.linearVelocity = Vector2.zero;
//             rb.isKinematic = true;
//             gameManager.GetComponent<GameManager>().GameOver();
//             audioSource.PlayOneShot(deathClip, 1);
//         }
//     }

//     // 🔹 New Method: Revive the player
//     public void RevivePlayer()
//     {
//         isDead = false;
//         rb.isKinematic = false;
//         rb.linearVelocity = Vector2.zero;
//         if (playerCollider != null)
//             playerCollider.enabled = true;
//         gameObject.SetActive(true);
//     }
// }






using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [SerializeField] private GameObject gameManager;

    private GameManager gameManagerComponent;
    private DisplayManager displayManagerComponent;
    [SerializeField] private float horizontalSpeed;
    [SerializeField] private float verticalSpeed;
    [SerializeField] private int maxVerticalSpeed;
    [SerializeField] private int accelerationForce;
    [SerializeField] private int decelerationForce;

    [SerializeField] private GameObject deathEffect;
    [SerializeField] private GameObject colorChangeEffect;

    private bool isDead;
    private bool freezeHorizontal;  // ⭐ new
    private float mapWidth;
    private float movementAngle;

    private Rigidbody2D rb;

    private AudioSource audioSource;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip itemClip;

    private Collider2D playerCollider;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        playerCollider = GetComponent<Collider2D>();

        if (gameManager != null)
        {
            gameManagerComponent = gameManager.GetComponent<GameManager>();
            displayManagerComponent = gameManager.GetComponent<DisplayManager>();
        }

        if (displayManagerComponent != null)
        {
            mapWidth = displayManagerComponent.GetDisplayWidth();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Update()
    {
        if (isDead) return;

        HandleInput();
        MovePlayer();
    }

    private void HandleInput()
    {
        // Tap pressed → go straight up
        if (Input.GetMouseButton(0))
        {
            freezeHorizontal = true;   // ⭐ freeze left-right

            if (rb.linearVelocity.y < maxVerticalSpeed)
                rb.AddForce(Vector2.up * accelerationForce);
        }
        else
        {
            freezeHorizontal = false;  // ⭐ resume left-right

            if (rb.linearVelocity.y > 0)
                rb.AddForce(Vector2.down * decelerationForce);
            else
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        }
    }

    private void MovePlayer()
    {
        Vector2 position = transform.position;

        // ⭐ If NOT pressed → left-right movement enabled
        if (!freezeHorizontal)
        {
            position.x = Mathf.Cos(movementAngle) * (mapWidth * 0.45f);
            movementAngle += Time.deltaTime * horizontalSpeed;
        }

        // ⭐ Always move upward slowly (your original design)
        position.y += verticalSpeed * Time.deltaTime;

        transform.position = position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item_ColorChange"))
        {
            GameObject fx = Instantiate(colorChangeEffect, other.transform.position, Quaternion.identity);
            Destroy(fx, 0.5f);
            Destroy(other.transform.parent.gameObject);
            ScoreManager.Instance.IncrementScore();
            audioSource.PlayOneShot(itemClip, 1);
        }

        if (other.CompareTag("Obstacle") && !isDead)
        {
            isDead = true;
            GameObject fx = Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(fx, 0.5f);
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
            if (gameManagerComponent != null)
            {
                gameManagerComponent.GameOver();
            }
            audioSource.PlayOneShot(deathClip, 1);
        }
    }

    public void RevivePlayer()
    {
        isDead = false;
        rb.isKinematic = false;
        rb.linearVelocity = Vector2.zero;

        if (playerCollider != null)
            playerCollider.enabled = true;

        gameObject.SetActive(true);
    }
}

