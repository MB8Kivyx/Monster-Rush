using UnityEngine;

/// <summary>
/// EndlessBackground class creates an endless scrolling background effect.
/// It uses two background sprites that loop seamlessly when the player moves upward.
/// The background follows the player's position to ensure it always stays aligned.
/// </summary>
public class EndlessBackground : MonoBehaviour
{
    [Header("Background Settings")]
    [SerializeField] private Transform background1; // First background sprite
    [SerializeField] private Transform background2; // Second background sprite (for seamless looping)
    
    [Header("Auto Setup")]
    [SerializeField] private bool autoFindBackgrounds = true; // Automatically find background children if not assigned
    
    private Camera mainCamera;
    private float backgroundHeight;
    private float cameraVerticalExtent;
    private float lastPlayerY;
    private float lastCameraY;
    
    private void Awake()
    {
        mainCamera = Camera.main;
        
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found! EndlessBackground requires a camera tagged as MainCamera.");
            enabled = false;
            return;
        }
        
        // Calculate camera's vertical extent
        cameraVerticalExtent = mainCamera.orthographicSize;
        
        // Auto-setup if backgrounds are not assigned
        if (autoFindBackgrounds)
        {
            SetupBackgrounds();
        }
    }
    
    private void Start()
    {
        // Validate that backgrounds are assigned
        if (background1 == null || background2 == null)
        {
            Debug.LogError("Background transforms not assigned! Please assign background1 and background2 in the inspector.");
            enabled = false;
            return;
        }
        
        // Calculate background height based on sprite renderer bounds
        CalculateBackgroundHeight();
        
        // Position the second background above the first one
        PositionBackgrounds();
        
        // Set background render order to be behind everything
        SetBackgroundRenderOrder();
        
        // Initialize tracking positions
        if (Player.Instance != null)
        {
            lastPlayerY = Player.Instance.transform.position.y;
        }
        if (mainCamera != null)
        {
            lastCameraY = mainCamera.transform.position.y;
        }
    }
    
    /// <summary>
    /// Sets the background render order to be behind all game objects
    /// </summary>
    private void SetBackgroundRenderOrder()
    {
        if (background1 != null)
        {
            SpriteRenderer bg1Renderer = background1.GetComponent<SpriteRenderer>();
            if (bg1Renderer != null)
            {
                bg1Renderer.sortingLayerName = "Background";
                bg1Renderer.sortingOrder = 0;
            }
        }
        
        if (background2 != null)
        {
            SpriteRenderer bg2Renderer = background2.GetComponent<SpriteRenderer>();
            if (bg2Renderer != null)
            {
                bg2Renderer.sortingLayerName = "Background";
                bg2Renderer.sortingOrder = 0;
            }
        }
    }
    
    private void SetupBackgrounds()
    {
        // Try to find background GameObjects as children
        Transform[] children = GetComponentsInChildren<Transform>();
        
        foreach (Transform child in children)
        {
            if (child != transform && child.gameObject.name.ToLower().Contains("bg"))
            {
                if (background1 == null)
                {
                    background1 = child;
                }
                else if (background2 == null)
                {
                    background2 = child;
                    break;
                }
            }
        }
        
        // If only one background found, create a duplicate
        if (background1 != null && background2 == null)
        {
            GameObject bg2Clone = Instantiate(background1.gameObject, background1.parent);
            bg2Clone.name = background1.name + "_Clone";
            background2 = bg2Clone.transform;
        }
    }
    
    private void CalculateBackgroundHeight()
    {
        SpriteRenderer spriteRenderer = background1.GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            backgroundHeight = spriteRenderer.bounds.size.y;
        }
        else
        {
            // Fallback: use camera height * 2 for seamless coverage
            backgroundHeight = cameraVerticalExtent * 2f;
            Debug.LogWarning("Could not determine background height from sprite. Using camera height * 2.");
        }
    }
    
    private void PositionBackgrounds()
    {
        if (background1 == null || background2 == null) return;
        
        // Get reference position (prefer player, fallback to camera)
        float referenceY = 0f;
        
        if (Player.Instance != null)
        {
            referenceY = Player.Instance.transform.position.y;
        }
        else if (mainCamera != null)
        {
            referenceY = mainCamera.transform.position.y;
        }
        
        // Position backgrounds to cover the camera view
        float bottomY = referenceY - cameraVerticalExtent;
        background1.position = new Vector3(0, bottomY, background1.position.z);
        background2.position = new Vector3(0, bottomY + backgroundHeight, background2.position.z);
    }
    
    private void Update()
    {
        if (background1 == null || background2 == null || mainCamera == null) return;
        
        // Get current player and camera positions
        float currentPlayerY = 0f;
        float currentCameraY = mainCamera.transform.position.y;
        bool playerMoved = false;
        bool cameraMoved = false;
        
        if (Player.Instance != null)
        {
            currentPlayerY = Player.Instance.transform.position.y;
            playerMoved = Mathf.Abs(currentPlayerY - lastPlayerY) > 0.01f;
        }
        
        cameraMoved = Mathf.Abs(currentCameraY - lastCameraY) > 0.01f;
        
        // If player or camera moved, update background positions
        if (playerMoved || cameraMoved)
        {
            // Calculate how much the player/camera moved
            float deltaY = 0f;
            
            if (Player.Instance != null)
            {
                deltaY = currentPlayerY - lastPlayerY;
                lastPlayerY = currentPlayerY;
            }
            else
            {
                deltaY = currentCameraY - lastCameraY;
            }
            
            lastCameraY = currentCameraY;
            
            // Move backgrounds to follow player/camera
            // Backgrounds move opposite to player movement (scroll effect)
            background1.position += new Vector3(0, -deltaY, 0);
            background2.position += new Vector3(0, -deltaY, 0);
            
            // Ensure backgrounds stay aligned with camera view
            MaintainBackgroundCoverage();
        }
    }
    
    private void MaintainBackgroundCoverage()
    {
        if (background1 == null || background2 == null || mainCamera == null) return;
        
        float cameraY = mainCamera.transform.position.y;
        float cameraTop = cameraY + cameraVerticalExtent;
        float cameraBottom = cameraY - cameraVerticalExtent;
        
        // Check if we need to reposition backgrounds
        // Background should be above camera bottom
        float bg1Top = background1.position.y + (backgroundHeight * 0.5f);
        float bg1Bottom = background1.position.y - (backgroundHeight * 0.5f);
        float bg2Top = background2.position.y + (backgroundHeight * 0.5f);
        float bg2Bottom = background2.position.y - (backgroundHeight * 0.5f);
        
        // If background1 is too far below, move it above background2
        if (bg1Top < cameraBottom)
        {
            float newY = background2.position.y + backgroundHeight;
            background1.position = new Vector3(background1.position.x, newY, background1.position.z);
        }
        
        // If background2 is too far below, move it above background1
        if (bg2Top < cameraBottom)
        {
            float newY = background1.position.y + backgroundHeight;
            background2.position = new Vector3(background2.position.x, newY, background2.position.z);
        }
        
        // If background1 is too far above, move it below background2
        if (bg1Bottom > cameraTop && background2.position.y < background1.position.y)
        {
            float newY = background2.position.y - backgroundHeight;
            background1.position = new Vector3(background1.position.x, newY, background1.position.z);
        }
        
        // If background2 is too far above, move it below background1
        if (bg2Bottom > cameraTop && background1.position.y < background2.position.y)
        {
            float newY = background1.position.y - backgroundHeight;
            background2.position = new Vector3(background2.position.x, newY, background2.position.z);
        }
    }
    
    // Public method to reset background positions (useful when restarting game)
    public void ResetBackgrounds()
    {
        PositionBackgrounds();
        if (Player.Instance != null)
        {
            lastPlayerY = Player.Instance.transform.position.y;
        }
        if (mainCamera != null)
        {
            lastCameraY = mainCamera.transform.position.y;
        }
    }
}
