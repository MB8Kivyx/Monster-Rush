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






using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [SerializeField] private GameObject gameManager;
    private DisplayManager displayManagerComponent;

    private bool isInvincible = false;

    [Header("Movement Settings")]
    [Tooltip("How fast the car moves Left/Right.")]
    [SerializeField] private float horizontalSpeed;
    
    [Tooltip("Starting forward speed.")]
    [SerializeField] private float verticalSpeed;
    
    [Tooltip("Maximum limit for speed (if needed).")]
    [SerializeField] private int maxVerticalSpeed;
    
    [Tooltip("Speed added per second automatically.")]
    [SerializeField] private float speedIncreaseRate = 0.5f; 

    // Legacy physics forces (unused now but kept to avoid broken refs if used elsewhere)
    [Tooltip("How fast it speeds up when holding the player.")]
    [SerializeField] private int accelerationForce;
    [Tooltip("How fast it slows down when released.")]
    [SerializeField] private int decelerationForce;

    [Header("Visual Effects")]
    [Tooltip("Maximum tilt angle when turning.")]
    [SerializeField] private float maxRotationAngle = 15f;
    [Tooltip("How fast the car rotates.")]
    [SerializeField] private float carRotationSpeed = 10f;
    
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private GameObject colorChangeEffect;

    private bool isDead;
    private float mapWidth;
    
    // 3-LANE LOGIC
    private int currentLane = 0; 
    private float laneDistance; 
    [SerializeField] private float laneSwitchSpeed = 10f;
    private float targetX;

    // Movement State
    private float currentVerticalSpeed; // Actual speed being applied

    private Rigidbody2D rb;

    private AudioSource audioSource;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip itemClip;

    // ... (Awake, Start, etc. remain the same until MovePlayer)

// ...

    // ... (Awake, Start, etc. remain the same)

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
            displayManagerComponent = gameManager.GetComponent<DisplayManager>();
        }
        
        // Fallback: If not assigned, try to find by type
        if (displayManagerComponent == null)
        {
            displayManagerComponent = FindFirstObjectByType<DisplayManager>();
        }

        if (displayManagerComponent != null)
        {
            mapWidth = displayManagerComponent.GetDisplayWidth();
            // Calculate lane distance based on map width (e.g., 30% of width)
            laneDistance = mapWidth * 0.4f;
    }
        else
        {
            laneDistance = 2f; // Fallback
        }
        
        targetX = transform.position.x; // Start at current
        currentVerticalSpeed = verticalSpeed; // Start at base speed
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    // Public Property for Sound Script
    public float CurrentSpeed => currentVerticalSpeed;

    private void Update()
    {
        if (isDead) return;

        // PROGRESSION: Increase base speed over time
        verticalSpeed += speedIncreaseRate * Time.deltaTime;
        
        // Sync current speed (since we removed manual boost)
        currentVerticalSpeed = verticalSpeed;

        HandleInput();
        MovePlayer();
    }

    // Swipe Inputs
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private float minSwipeDistance = 50f; // Minimum distance for a swipe to register

    private void HandleInput()
    {
        // 1. Swipe Detection (Lane Switch)
        if (Input.GetMouseButtonDown(0))
        {
            startTouchPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            endTouchPosition = Input.mousePosition;
            DetectSwipe();
        }

        // 2. Hold Anywhere to Boost (Gas Pedal)
        // User request: "when user tap on screen the speed should increase"
        if (Input.GetMouseButton(0))
        {
            // Boost Speed towards Max
            currentVerticalSpeed = Mathf.MoveTowards(currentVerticalSpeed, maxVerticalSpeed, accelerationForce * Time.deltaTime);
        }
        else
        {
            // Decelerate back to normal Progression Speed (verticalSpeed)
            currentVerticalSpeed = Mathf.MoveTowards(currentVerticalSpeed, verticalSpeed, decelerationForce * Time.deltaTime);
        }
    }

    private void DetectSwipe()
    {
        float distanceX = endTouchPosition.x - startTouchPosition.x;
        float distanceY = endTouchPosition.y - startTouchPosition.y;

        // Check if horizontal swipe is long enough and dominant over vertical swipe
        if (Mathf.Abs(distanceX) > minSwipeDistance && Mathf.Abs(distanceX) > Mathf.Abs(distanceY))
        {
            if (distanceX > 0)
            {
                // Swipe Right
                MoveLane(1);
            }
            else
            {
                // Swipe Left
                MoveLane(-1);
            }
        }
    }
    
    private void MoveLane(int direction)
    {
        currentLane += direction;
        currentLane = Mathf.Clamp(currentLane, -1, 1);
        targetX = currentLane * laneDistance;
    }

    private void MovePlayer()
    {
        Vector2 position = transform.position;

        // Lane Switching
        position.x = Mathf.MoveTowards(position.x, targetX, laneSwitchSpeed * Time.deltaTime);

        // Vertical Movement with Boost
        position.y += currentVerticalSpeed * Time.deltaTime;

        transform.position = position;
        
        // ROTATION LOGIC
        // Determine direction of movement relative to target
        float diff = targetX - position.x;
        float targetZ = 0f;

        // If we are still distant from the target lane, rotate
        if (Mathf.Abs(diff) > 0.05f)
        {
            // Moving Right (diff > 0) -> Rotate Right (-Z)
            // Moving Left  (diff < 0) -> Rotate Left  (+Z)
            targetZ = diff > 0 ? -maxRotationAngle : maxRotationAngle;
        }
        
        // Smooth Rotation
        // Use MoveTowardsAngle for non-springy, reliable rotation towards target
        float currentZ = transform.eulerAngles.z;
        float newZ = Mathf.MoveTowardsAngle(currentZ, targetZ, carRotationSpeed * Time.deltaTime * 20f);
        
        transform.rotation = Quaternion.Euler(0, 0, newZ);

        if(rb.bodyType != RigidbodyType2D.Kinematic) rb.linearVelocity = Vector2.zero; 
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

        if (other.CompareTag("Obstacle") && !isDead && !isInvincible)
        {
            isDead = true;
            GameObject fx = Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(fx, 0.5f);
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
            else
            {
                // Last resort
                var gm = FindFirstObjectByType<GameManager>();
                if (gm != null) gm.GameOver();
            }
            
            audioSource.PlayOneShot(deathClip, 1);
        }
    }

    public bool IsInvincible => isInvincible;

    public void RevivePlayer()
    {
        isDead = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = Vector2.zero;

        if (playerCollider != null)
            playerCollider.enabled = true;

        gameObject.SetActive(true);
        
        // Reset Lane on revive
        targetX = transform.position.x;

        // Start Invincibility
        StartCoroutine(ReviveInvincibility());
    }

    private IEnumerator ReviveInvincibility()
    {
        isInvincible = true;
        
        // Visual indicator (optional flickering)
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float timer = 5f; // 5 seconds invincibility
        while (timer > 0)
        {
            if (sr != null) sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.1f);
            timer -= 0.1f;
        }
        
        if (sr != null) sr.enabled = true;
        isInvincible = false;
    }
}

