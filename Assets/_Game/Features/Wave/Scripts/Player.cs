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
    
    [Tooltip("Starting forward speed (Cruising speed).")]
    [SerializeField] private float verticalSpeed = 12f;
    
    [Tooltip("Maximum limit for speed (Boosted speed).")]
    [SerializeField] private float maxVerticalSpeed = 22f;

    [Tooltip("How fast it speeds up when holding (Target: ~1.5s to max).")]
    [SerializeField] private float accelerationForce = 6.5f;
    [Tooltip("How fast it slows down when released (Coasting).")]
    [SerializeField] private float decelerationForce = 5f;

    [Header("Visual Effects")]
    [Tooltip("Maximum tilt angle when turning.")]
    [SerializeField] private float maxRotationAngle = 15f;
    [Tooltip("How fast the car rotates.")]
    [SerializeField] private float carRotationSpeed = 10f;
    
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private GameObject colorChangeEffect;

    private bool isDead;
    private float mapWidth;
    
    // THREE-TRACK MOVEMENT SYSTEM
    [Header("Three-Track System")]
    [SerializeField] private float horizontalMovementSmoothing = 0.08f;
    [SerializeField] private float swipeThreshold = 50f; // Minimum pixels for a swipe
    [SerializeField] private float laneWidth = 2.0f; // Distance between tracks

    private int currentTrackIndex = 1; // 0 = Left, 1 = Center, 2 = Right
    private float targetX;
    private float xVelocity = 0f; 
    private Vector2 swipeStartPosition;
    private bool isInputActive = false;

    // Movement State
    private float currentVerticalSpeed; 

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
            displayManagerComponent = gameManager.GetComponent<DisplayManager>();
        }
        
        if (displayManagerComponent == null)
        {
            displayManagerComponent = FindFirstObjectByType<DisplayManager>();
        }

        if (displayManagerComponent != null)
        {
            mapWidth = displayManagerComponent.GetDisplayWidth();
            // Automatically calculate lane width based on map width
            laneWidth = mapWidth * 0.35f; 
        }
        else
        {
            mapWidth = 5f; 
            laneWidth = 1.75f;
        }
        
        // Start in the Center Track
        currentTrackIndex = 1;
        targetX = 0f; 
        currentVerticalSpeed = verticalSpeed; 
        
        if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // Public Properties for Sound Script
    public float CurrentSpeed => currentVerticalSpeed;
    public float BaseSpeed => verticalSpeed;
    public float MaxSpeed => maxVerticalSpeed;

    private void Update()
    {
        if (isDead) return;

        HandleInput();
        
        // Progressive Speed Boost Logic (Hold anywhere)
        if (Input.GetMouseButton(0))
        {
            // Gradually increase speed towards maxSpeed
            currentVerticalSpeed = Mathf.MoveTowards(currentVerticalSpeed, maxVerticalSpeed, accelerationForce * Time.deltaTime);
        }
        else
        {
            // Gradually return to base verticalSpeed
            currentVerticalSpeed = Mathf.MoveTowards(currentVerticalSpeed, verticalSpeed, decelerationForce * Time.deltaTime);
        }
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            swipeStartPosition = Input.mousePosition;
            isInputActive = true;
        }

        if (Input.GetMouseButtonUp(0) && isInputActive)
        {
            Vector2 swipeEndPosition = Input.mousePosition;
            float deltaX = swipeEndPosition.x - swipeStartPosition.x;

            // Detect Swipe Direction
            if (Mathf.Abs(deltaX) > swipeThreshold)
            {
                if (deltaX > 0)
                {
                    // Swipe Right
                    ChangeTrack(1);
                }
                else
                {
                    // Swipe Left
                    ChangeTrack(-1);
                }
            }
            isInputActive = false;
        }
    }

    private void ChangeTrack(int direction)
    {
        currentTrackIndex += direction;
        currentTrackIndex = Mathf.Clamp(currentTrackIndex, 0, 2);
        
        // map currentTrackIndex to targetX: 0 -> -laneWidth, 1 -> 0, 2 -> laneWidth
        targetX = (currentTrackIndex - 1) * laneWidth;
    }

    private void LateUpdate()
    {
        if (isDead) return;
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        Vector3 currentPos = transform.position;

        // Smoothly interpolate towards targetX for the designated track
        float smoothedX = Mathf.SmoothDamp(currentPos.x, targetX, ref xVelocity, horizontalMovementSmoothing);
        
        currentPos.x = smoothedX;

        // Vertical movement
        currentPos.y += currentVerticalSpeed * Time.deltaTime;

        if(rb != null) rb.MovePosition(currentPos);
        else transform.position = currentPos;
        
        // VISUAL POLISH: Rotation based on movement to target
        float targetZ = 0f;
        float diff = targetX - currentPos.x;
        if (Mathf.Abs(diff) > 0.01f)
        {
            // Lean towards the target track
            targetZ = diff > 0 ? -maxRotationAngle : maxRotationAngle;
        }
        
        float currentZ = transform.eulerAngles.z;
        float newZ = Mathf.MoveTowardsAngle(currentZ, targetZ, carRotationSpeed * Time.deltaTime * 30f);
        transform.rotation = Quaternion.Euler(0, 0, newZ);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Combined logic from PlayerCollision.cs
        if (isInvincible && other.CompareTag("Obstacle")) return;

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
            HandleDeath();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isInvincible && collision.gameObject.CompareTag("Obstacle")) return;

        if (collision.gameObject.CompareTag("Obstacle") && !isDead && !isInvincible)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        isDead = true;
        GameObject fx = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(fx, 0.5f);
        
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            var gm = FindFirstObjectByType<GameManager>();
            if (gm != null) gm.GameOver();
        }

        audioSource.PlayOneShot(deathClip, 1);
    }

    public bool IsInvincible => isInvincible;

    public void RevivePlayer()
    {
        isDead = false;
        
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (playerCollider != null)
        {
            playerCollider.enabled = true;
            // Set to Trigger during invincibility to prevent physical sticking
            playerCollider.isTrigger = true;
        }

        gameObject.SetActive(true);
        
        // RELOCATE SLIGHTLY FORWARD: Clear whatever we just hit
        Vector3 spawnPos = transform.position;
        spawnPos.y += 3.0f; // Move 3 units forward
        transform.position = spawnPos;

        // Reset Lane target to current position to avoid immediate snapping
        targetX = transform.position.x;

        // Start Invincibility
        StartCoroutine(ReviveInvincibility());
    }

    private IEnumerator ReviveInvincibility()
    {
        isInvincible = true;
        
        // Visual indicator (flickering)
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float timer = 4f; // 4 seconds invincibility
        while (timer > 0)
        {
            if (sr != null) sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.1f);
            timer -= 0.1f;
        }
        
        if (sr != null) sr.enabled = true;
        
        // Reset Collider to physical state
        if (playerCollider != null)
        {
            playerCollider.isTrigger = false;
        }

        isInvincible = false;
    }
}
