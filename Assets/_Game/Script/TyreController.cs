using UnityEngine;
using System.Collections.Generic;

public class TyreController : MonoBehaviour
{
    [Header("Specific Tyre References")]
    public Transform frontLeftTyre;
    public Transform frontRightTyre;
    public Transform backLeftTyre;
    public Transform backRightTyre;

    [Header("Rotation Settings")]
    [SerializeField] private float baseRotationSpeed = 300f;
    [SerializeField] private bool rotateOnY = false; 
    [SerializeField] private bool rotateOnZ = false; 
    [SerializeField] private bool rotateOnX = true;  
    
    private bool isRotating = false;

    private void Awake()
    {
        // Auto-discovery if fields are empty
        if (frontLeftTyre == null || frontRightTyre == null || backLeftTyre == null || backRightTyre == null)
        {
            FindTyresRecursively(transform);
        }
    }

    private void FindTyresRecursively(Transform parent)
    {
        foreach (Transform child in parent)
        {
            string name = child.name.ToLower();
            
            // Simple heuristic mapping
            if (name.Contains("fl") || (name.Contains("front") && name.Contains("left")) || name.Contains("wheel_1"))
            {
                if (frontLeftTyre == null) frontLeftTyre = child;
            }
            else if (name.Contains("fr") || (name.Contains("front") && name.Contains("right")) || name.Contains("wheel_2"))
            {
                if (frontRightTyre == null) frontRightTyre = child;
            }
            else if (name.Contains("bl") || (name.Contains("back") && name.Contains("left")) || name.Contains("wheel_3"))
            {
                if (backLeftTyre == null) backLeftTyre = child;
            }
            else if (name.Contains("br") || (name.Contains("back") && name.Contains("right")) || name.Contains("wheel_4"))
            {
                if (backRightTyre == null) backRightTyre = child;
            }
            // Fallback for generic "Wheel" names if not assigned above is harder without specific naming conventions, 
            // but this covers most standard vehicle setups.

            // Recurse
            FindTyresRecursively(child);
        }
    }

    public void StartRotation()
    {
        isRotating = true;
    }

    public void StopRotation()
    {
        isRotating = false;
    }
    
    // Explicitly set rotation state
    public void SetRotationState(bool state)
    {
        isRotating = state;
    }

    private void Update()
    {
        if (!isRotating) return;

        // Determine speed multiplier
        float speedMultiplier = 1f;
        
        // If Player instance exists, scale by its normalized speed or actual speed
        if (Player.Instance != null)
        {
             // Uses normalized speed (0 to 1) + base mod, or just standard multiplier
            // If NormalizedSpeed is exposed:
            float norm = Player.Instance.NormalizedSpeed;
            // Map 0..1 to 0..2 (e.g., idle spin to fast spin) or similar
            // Or just use a direct multiplier if available
            speedMultiplier = 0.5f + (norm * 1.5f); 
        }

        // Calculate rotation for this frame
        float rotationAmount = baseRotationSpeed * speedMultiplier * Time.deltaTime;

        // Apply to all tyres
        // Apply to specific tyres
        RotateTyre(frontLeftTyre, rotationAmount);
        RotateTyre(frontRightTyre, rotationAmount);
        RotateTyre(backLeftTyre, rotationAmount);
        RotateTyre(backRightTyre, rotationAmount);
    }

    private void RotateTyre(Transform tyre, float amount)
    {
        if (tyre == null) return;
        if (rotateOnX) tyre.Rotate(amount, 0, 0);
        if (rotateOnY) tyre.Rotate(0, amount, 0);
        if (rotateOnZ) tyre.Rotate(0, 0, amount);
    }
}
