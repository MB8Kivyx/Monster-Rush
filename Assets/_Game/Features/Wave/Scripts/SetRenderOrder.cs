using UnityEngine;

/// <summary>
/// SetRenderOrder ensures game objects (player, obstacles, coins) are rendered in front of the background.
/// This script automatically sets the sorting layer and order for proper visibility.
/// </summary>
public class SetRenderOrder : MonoBehaviour
{
    [Header("Render Settings")]
    [SerializeField] private int sortingOrder = 10; // Higher value = rendered in front
    [SerializeField] private string sortingLayerName = "Gameplay"; // Sorting layer name
    
    [Header("Auto Setup")]
    [SerializeField] private bool setOnStart = true; // Automatically set on Start
    
    private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // If no SpriteRenderer on this object, try to find it in children
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }
    
    private void Start()
    {
        if (setOnStart)
        {
            SetRenderLayer();
        }
    }
    
    /// <summary>
    /// Sets the render layer and sorting order for this object
    /// </summary>
    public void SetRenderLayer()
    {
        if (spriteRenderer == null)
        {
            Debug.LogWarning($"SetRenderOrder: No SpriteRenderer found on {gameObject.name}", this);
            return;
        }
        
        // Set sorting layer
        if (!string.IsNullOrEmpty(sortingLayerName))
        {
            spriteRenderer.sortingLayerName = sortingLayerName;
        }
        
        // Set sorting order
        spriteRenderer.sortingOrder = sortingOrder;
    }
    
    /// <summary>
    /// Sets render layer for all SpriteRenderers in children
    /// </summary>
    public void SetRenderLayerForAllChildren()
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        
        foreach (SpriteRenderer renderer in renderers)
        {
            if (!string.IsNullOrEmpty(sortingLayerName))
            {
                renderer.sortingLayerName = sortingLayerName;
            }
            renderer.sortingOrder = sortingOrder;
        }
    }
}

