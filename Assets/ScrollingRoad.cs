using UnityEngine;

public class ScrollingRoad : MonoBehaviour
{
    public float speed = 2f;  // Inspector se adjust karo (zyada = fast scroll)

    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        float offset = Mathf.Repeat(Time.time * speed, 1f);  // 0 se 1 loop
        rend.material.mainTextureOffset = new Vector2(0, offset);  // Vertical scroll (Y)
    }
}