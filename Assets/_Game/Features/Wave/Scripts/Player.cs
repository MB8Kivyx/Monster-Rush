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
    public Sprite normalSprite;
    public Sprite damageStage1Sprite;
    public Sprite damageStage2Sprite;
    
    [Header("Automated Visual Management")]
    [Tooltip("Assign the GameObject for normal driving. If left empty, I will try to find a child named 'Normal'.")]
    public GameObject normalVisual;
    [Tooltip("Assign the GameObject for crash state. If left empty, I will try to find a child named 'Crashed'.")]
    public GameObject crashedVisual;
    
    public SpriteRenderer spriteRenderer;
    private bool isDead;
    private float mapWidth;
    
    [Header("Collision Settings")]
    [Tooltip("Distance the car slides back upon hitting an obstacle.")]
    [SerializeField] private float impactSlideDistance = 2f;
    [Tooltip("Duration of the backward slide.")]
    [SerializeField] private float impactSlideDuration = 0.5f;
    
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
    private Vector2 normalSpriteSize; // To keep world-size consistent

    [Header("Tyre System")]
    [Tooltip("Assign your tyre objects here.")]
    public Transform[] tyres;
    [Tooltip("Base speed for tyre rotation.")]
    [SerializeField] private float baseTyreRotationSpeed = 500f;
    [Tooltip("Multiplier for how much car speed affects tyre spin.")]
    [SerializeField] private float tyreSpeedMultiplier = 20f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (normalSprite != null) normalSpriteSize = normalSprite.bounds.size;

        // 1. Setup the dual-visual hierarchy automatically
        SetupAutomatedVisuals();

        // 2. Initial state
        UpdateDamageVisual(3);
    }

    private void Start()
    {
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
        // Tyres should always rotate (even when crashed)
        HandleTyreRotation();
        
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

        // Visual swap: Show crashed object
        UpdateDamageVisual(1);

        // Trigger recoil slide
        StartCoroutine(ImpactSlideRoutine());

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
        
        // Visual swap: Show normal object
        UpdateDamageVisual(3);
        
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
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
        

    }

    private IEnumerator ReviveInvincibility()
    {
        // isInvincible is now set true in RevivePlayer
        
        // Visual indicator (flickering)
        float timer = 4f; // 4 seconds invincibility
        while (timer > 0)
        {
            // Toggle all active renderers in children
            foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
            {
                if (sr != null) sr.enabled = !sr.enabled;
            }
            yield return new WaitForSeconds(0.1f);
            timer -= 0.1f;
        }
        
        // Ensure all renderers are back on
        foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
        {
            if (sr != null) sr.enabled = true;
        }
        
        // Reset Collider to physical state
        if (playerCollider != null)
        {
            playerCollider.isTrigger = false;
        }

        isInvincible = false;
    }

    /// <summary>
    /// Updates the car's visual appearance based on remaining lives.
    /// Toggles objects and automatically syncs their scales to match the original car size.
    /// </summary>
    public void UpdateDamageVisual(int currentLives)
    {
        Sprite targetSprite = null;
        GameObject activeObj = null;

        switch (currentLives)
        {
            case 3:
                targetSprite = normalSprite;
                activeObj = normalVisual;
                break;
            case 2:
                targetSprite = damageStage1Sprite;
                activeObj = normalVisual;
                break;
            case 1:
            case 0:
            default:
                targetSprite = damageStage2Sprite;
                activeObj = crashedVisual;
                break;
        }

        // Toggle visibility
        if (normalVisual != null) normalVisual.SetActive(activeObj == normalVisual);
        if (crashedVisual != null) crashedVisual.SetActive(activeObj == crashedVisual);

        // Apply sprite and sync scale to prevent "Giant Car" or "Mini Car" issues
        if (activeObj != null && targetSprite != null)
        {
            SpriteRenderer sr = activeObj.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = targetSprite;
                SyncVisualScale(activeObj, targetSprite);
            }
        }

        // Refresh tyre references for the currently active visual
        FindTyresInActiveVisual(activeObj);

        Debug.Log($"[Player] Auto-Visual Swap: {(activeObj != null ? activeObj.name : "None")} with {targetSprite?.name}");
    }

    private void SetupAutomatedVisuals()
    {
        // Auto-assign from naming convention if empty
        if (normalVisual == null) normalVisual = transform.Find("Normal")?.gameObject;
        if (crashedVisual == null) crashedVisual = transform.Find("Crashed")?.gameObject;

        // Fallback: If user has a flat structure, we'll suggest setup
        if (normalVisual == null || crashedVisual == null)
        {
            Debug.LogWarning("[Player] Dual-visual system needs child GameObjects named 'Normal' and 'Crashed'. Please organize your hierarchy for best results.");
        }
        
        // Ensure starting scales are clean
        if (normalVisual != null) normalVisual.transform.localScale = Vector3.one;
        if (crashedVisual != null) crashedVisual.transform.localScale = Vector3.one;
    }

    private void SyncVisualScale(GameObject visualObj, Sprite currentSprite)
    {
        if (currentSprite == null || normalSprite == null) return;

        // Calculate the ratio to keep the world-size identical to the normal sprite
        float ratioX = normalSpriteSize.x / currentSprite.bounds.size.x;
        float ratioY = normalSpriteSize.y / currentSprite.bounds.size.y;

        // Apply this ONLY to the child visual object
        visualObj.transform.localScale = new Vector3(ratioX, ratioY, 1f);
    }

    private void FindTyresInActiveVisual(GameObject activeObj)
    {
        // PRIORITIZE MANUAL ASSIGNMENT:
        // If the user has already assigned tyres in the Inspector, don't overwrite them.
        if (tyres != null && tyres.Length > 0)
        {
            Debug.Log("[Player] Using manual tyre assignments from Inspector.");
            return;
        }

        var foundTyres = new System.Collections.Generic.List<Transform>();
        
        // 1. Search in active visual specifically (Preferred)
        if (activeObj != null)
        {
            FindTyresRecursively(activeObj.transform, foundTyres);
        }
        
        // 2. Fallback: If no tyres in the visual (like a simple crash sprite), 
        // search the entire player object so we don't lose the rotation logic.
        if (foundTyres.Count == 0)
        {
            FindTyresRecursively(transform, foundTyres);
        }
        
        tyres = foundTyres.ToArray();
        
        if (tyres.Length > 0) 
            Debug.Log($"[Player] Automated Setup: {tyres.Length} tyres found for rotation.");
    }

    

    private void HandleTyreRotation()
    {
        if (tyres == null || tyres.Length == 0) return;

        // Calculate rotation speed based on vertical speed
        // Base rotation + (Current Speed * Multiplier)
        float currentRotSpeed = baseTyreRotationSpeed + (currentVerticalSpeed * tyreSpeedMultiplier);
        
        float rotationAmount = currentRotSpeed * Time.deltaTime;

        foreach (Transform tyre in tyres)
        {
            if (tyre != null)
            {
                // Assuming tyres forward is Z or they rotate around X. 
                // Standard for Unity 2D (if top down) is usually rotation around Z or X depending on setup.
                // Based on previous script, it was Rotate(amount, 0, 0) [X axis]
                tyre.Rotate(rotationAmount, 0, 0);
            }
        }
    }

    [ContextMenu("Auto Find Tyres")]
    public void FindTyres()
    {
        var foundTyres = new System.Collections.Generic.List<Transform>();
        FindTyresRecursively(transform, foundTyres);
        tyres = foundTyres.ToArray();
        Debug.Log($"[Player] Auto-found {tyres.Length} tyres.");
    }

    private void FindTyresRecursively(Transform parent, System.Collections.Generic.List<Transform> list)
    {
        foreach (Transform child in parent)
        {
            string n = child.name.ToLower();
            if (n.Contains("wheel") || n.Contains("tyre") || n.Contains("tire"))
            {
                list.Add(child);
            }
            FindTyresRecursively(child, list);
        }
    }

    private IEnumerator ImpactSlideRoutine()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos - new Vector3(0, impactSlideDistance, 0);

        while (elapsed < impactSlideDuration)
        {
            // Simple EaseOut slide
            float t = elapsed / impactSlideDuration;
            t = Mathf.Sin(t * Mathf.PI * 0.5f); // EaseOutSine

            // Only affect Y, keep X consistent with current lane (or where it was)
            Vector3 newPos = Vector3.Lerp(startPos, targetPos, t);
            newPos.x = transform.position.x; // Ensure X isn't chemically altered by Lerp if something weird happens (safety)
            transform.position = newPos;

            elapsed += Time.unscaledDeltaTime; // Use unscaled incase we pause timescale
            yield return null;
        }
    }
}

