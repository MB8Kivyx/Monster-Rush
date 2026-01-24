using UnityEngine;

public class TyreRotation : MonoBehaviour
{
    [Header("Tyre Objects")]
    public Transform frontLeftTyre;
    public Transform frontRightTyre;
    public Transform backLeftTyre;
    public Transform backRightTyre;

    [Header("Rotation Settings")]
    public float rotationSpeed = 300f;

    void Start()
    {
        // AUTO-DISCOVERY: Try to find tyres if not assigned
        if (frontLeftTyre == null) frontLeftTyre = FindTyre("FrontLeft", "FL", "Wheel_1");
        if (frontRightTyre == null) frontRightTyre = FindTyre("FrontRight", "FR", "Wheel_2");
        if (backLeftTyre == null) backLeftTyre = FindTyre("BackLeft", "BL", "Wheel_3");
        if (backRightTyre == null) backRightTyre = FindTyre("BackRight", "BR", "Wheel_4");

        // Fallback: Just grab any children with "Wheel" or "Tyre" in name
        if (frontLeftTyre == null && frontRightTyre == null)
        {
            foreach (Transform child in GetComponentsInChildren<Transform>())
            {
                string name = child.name.ToLower();
                if (name.Contains("wheel") || name.Contains("tyre") || name.Contains("tire"))
                {
                    // Assign to the first empty slot we find
                    if (frontLeftTyre == null) frontLeftTyre = child;
                    else if (frontRightTyre == null) frontRightTyre = child;
                    else if (backLeftTyre == null) backLeftTyre = child;
                    else if (backRightTyre == null) backRightTyre = child;
                }
            }
        }
    }

    Transform FindTyre(params string[] names)
    {
        // Helper to find a child by multiple possible names
        foreach (string n in names)
        {
            Transform t = transform.FindDeepChild(n); // Using a helper extension or recursive search
            if (t != null) return t;
        }
        return null;
    }

    void Update()
    {
        RotateTyres();
    }

    // void RotateTyres()
    // {
    //     // scale by Time.deltaTime so it stops when Time.timeScale is 0 (Game Over)
    //     float rotationAmount = rotationSpeed * Time.deltaTime;

    //     // Rotate all tyres on Z-axis (top-down view)
    //     if (frontLeftTyre != null) frontLeftTyre.Rotate(0, 0, -rotationAmount);
    //     if (frontRightTyre != null) frontRightTyre.Rotate(0, 0, -rotationAmount);
    //     if (backLeftTyre != null) backLeftTyre.Rotate(0, 0, -rotationAmount);
    //     if (backRightTyre != null) backRightTyre.Rotate(0, 0, -rotationAmount);
    // }

    void RotateTyres()
{
    float speedFactor = 1f;
    if (Player.Instance != null)
    {
        // Scale rotation by how much faster we are than base speed
        speedFactor = Player.Instance.CurrentSpeed / Player.Instance.BaseSpeed;
    }
    float rotationAmount = rotationSpeed * speedFactor * Time.deltaTime;

    if (frontLeftTyre != null) frontLeftTyre.Rotate(rotationAmount, 0, 0);
    if (frontRightTyre != null) frontRightTyre.Rotate(rotationAmount, 0, 0);
    if (backLeftTyre != null) backLeftTyre.Rotate(rotationAmount, 0, 0);
    if (backRightTyre != null) backRightTyre.Rotate(rotationAmount, 0, 0);
}

}

// Extension for recursive find
public static class TransformDeepChildExtension
{
    public static Transform FindDeepChild(this Transform aParent, string aName)
    {
        foreach(Transform child in aParent)
        {
            if(child.name.Contains(aName) || child.name == aName) return child;
            var result = child.FindDeepChild(aName);
            if (result != null) return result;
        }
        return null;
    }
}
