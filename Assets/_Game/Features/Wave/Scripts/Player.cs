using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

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
    [SerializeField] private float verticalSpeed = 10f;
    
    [Tooltip("Maximum limit for speed (Boosted speed).")]
    [SerializeField] private float maxVerticalSpeed = 22f;

    [Tooltip("How fast it speeds up when holding (Target: ~2s to max).")]
    [SerializeField] private float accelerationForce = 5f;
    [Tooltip("How fast it slows down when released (Coasting).")]
    [SerializeField] private float decelerationForce = 4f;

    [Header("Visual Effects")]
    [Tooltip("Maximum tilt angle when turning.")]
    [SerializeField] private float maxRotationAngle = 15f;
    [Tooltip("How fast the car rotates.")]
    [SerializeField] private float carRotationSpeed = 10f;
    
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private GameObject colorChangeEffect;

    [Header("Damage Visuals")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite damageStage1Sprite;
    [SerializeField] private Sprite damageStage2Sprite;

    private SpriteRenderer spriteRenderer;
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
    private Collider2D playerCollider;
    private Vector3 initialScale;

    [Header("Components")]
    [SerializeField] private TyreController tyreController;

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
        playerCollider = GetComponent<Collider2D>();

        if (gameManager != null)
        {
            displayManagerComponent = gameManager.GetComponent<DisplayManager>();
        }
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        
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
        
        initialScale = transform.localScale;

        // Auto-find TyreController if missing
        if (tyreController == null) tyreController = GetComponent<TyreController>();
        // If not on this object, check children
        if (tyreController == null) tyreController = GetComponentInChildren<TyreController>();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // Public Properties for Sound Script
    public float CurrentSpeed => currentVerticalSpeed;
    public float BaseSpeed => verticalSpeed;
    public float MaxSpeed => maxVerticalSpeed;

    /// <summary>
    /// Returns 0 to 1 based on current speed relative to base/max speed.
    /// 0 = Idle/Base Speed, 1 = Max Speed.
    /// </summary>
    public float NormalizedSpeed
    {
        get
        {
            if (maxVerticalSpeed <= verticalSpeed) return 0f;
            return Mathf.Clamp01((currentVerticalSpeed - verticalSpeed) / (maxVerticalSpeed - verticalSpeed));
        }
    }

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

        if (other.CompareTag("Item_ColorChange") || other.CompareTag("Coin") || other.CompareTag("Item"))
        {
            // Check for specific effects if needed, otherwise just score
            if (other.CompareTag("Item_ColorChange"))
            {
                GameObject fx = Instantiate(colorChangeEffect, other.transform.position, Quaternion.identity);
                Destroy(fx, 0.5f);
            }

            Destroy(other.transform.parent != null ? other.transform.parent.gameObject : other.gameObject);
            
            if (ScoreManager.Instance != null) ScoreManager.Instance.IncrementScore();
            
            if (CarSoundController.Instance != null)
                CarSoundController.Instance.PlayItemSound();
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
        if (isDead) return;

        // Play car crash sound immediately via centralized controller
        if (CarSoundController.Instance != null)
        {
            CarSoundController.Instance.PlayCrashSound();
        }
        else
        {
            Debug.LogWarning("Player: CarSoundController.Instance missing!");
        }

        isDead = true;

        GameObject fx = Instantiate(deathEffect, transform.position, Quaternion.identity);
        if (fx != null) Destroy(fx, 0.5f);
        
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Trigger the LifeSystem via GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerEliminated();
        }
        else
        {
            var gm = FindFirstObjectByType<GameManager>();
            if (gm != null) gm.OnPlayerEliminated();
        }
    }

    public bool IsInvincible => isInvincible;

    public void RevivePlayer()
    {
        // Set invincibility first to prevent immediate re-triggering
        isInvincible = true;
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
            playerCollider.isTrigger = true;
        }

        gameObject.SetActive(true);
        
        // RELOCATE SLIGHTLY FORWARD & CENTER: Clear whatever we just hit and reset lane
        Vector3 spawnPos = transform.position;
        spawnPos.y += 3.0f;
        spawnPos.x = 0f; // Force center alignment
        transform.position = spawnPos;

        // Reset Lane target to Center
        currentTrackIndex = 1;
        targetX = 0f;

        StartCoroutine(ReviveInvincibility());
        
        if (tyreController != null) tyreController.StartRotation();
    }

    private IEnumerator ReviveInvincibility()
    {
        // isInvincible is now set true in RevivePlayer
        
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

    /// <summary>
    /// Updates the car's visual appearance based on remaining lives.
    /// Expected progression: 3 lives (Normal), 2 lives (Stage 1), 1 life (Stage 2).
    /// </summary>
    public void UpdateDamageVisual(int currentLives)
    {
        if (spriteRenderer == null) return;

        Sprite targetSprite = null;

        switch (currentLives)
        {
            case 3:
                targetSprite = normalSprite;
                break;
            case 2:
                targetSprite = damageStage1Sprite;
                break;
            case 1:
                targetSprite = damageStage2Sprite;
                break;
            default:
                if (currentLives <= 0) targetSprite = damageStage2Sprite;
                break;
        }

        if (targetSprite != null)
        {
            spriteRenderer.sprite = targetSprite;
            MatchSpriteSize(targetSprite);
        }

        // Ensure tyres keep spinning unless we dictate otherwise (e.g. 0 lives)
        if (currentLives > 0 && tyreController != null)
        {
            tyreController.StartRotation();
        }
    }

    private void MatchSpriteSize(Sprite newSprite)
    {
        if (newSprite == null || normalSprite == null) return;

        // Calculate the ratio of the normal sprite's size to the new sprite's size
        // This ensures the new sprite occupies the same amount of world space as the original
        float ratioX = normalSprite.bounds.size.x / newSprite.bounds.size.x;
        float ratioY = normalSprite.bounds.size.y / newSprite.bounds.size.y;

        transform.localScale = new Vector3(initialScale.x * ratioX, initialScale.y * ratioY, initialScale.z);
    }
}
