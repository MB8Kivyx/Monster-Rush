using UnityEngine;

[DisallowMultipleComponent]
public class Rotate : MonoBehaviour
{
    public float speed = 40f;

    private void Update()
    {
        transform.Rotate(0f, 0f, speed * Time.deltaTime);
    }
}
