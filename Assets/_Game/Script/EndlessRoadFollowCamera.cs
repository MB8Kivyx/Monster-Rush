using UnityEngine;

public class EndlessRoadFollowCamera : MonoBehaviour
{
    [Header("Camera Reference")]
    public Transform cam;

    [Header("Road Tiles")]
    public Transform[] roadTiles;

    [Header("Scroll Settings")]
    public float scrollSpeed = 6f;

    private float tileHeight;

    void Start()
    {
        tileHeight = roadTiles[0]
            .GetComponent<SpriteRenderer>()
            .bounds.size.y;
    }

    void Update()
    {
        // Follow camera smoothly
        transform.position = new Vector3(
            transform.position.x,
            cam.position.y,
            transform.position.z
        );

        MoveRoad();
    }

    void MoveRoad()
    {
        foreach (Transform tile in roadTiles)
        {
            tile.Translate(Vector3.down * scrollSpeed * Time.deltaTime);

            if (tile.position.y < cam.position.y - tileHeight)
            {
                RepositionTile(tile);
            }
        }
    }

    void RepositionTile(Transform tile)
    {
        float highestY = GetHighestTileY();

        tile.position = new Vector3(
            tile.position.x,
            highestY + tileHeight,
            tile.position.z
        );
    }

    float GetHighestTileY()
    {
        float highest = roadTiles[0].position.y;

        foreach (Transform tile in roadTiles)
        {
            if (tile.position.y > highest)
                highest = tile.position.y;
        }

        return highest;
    }
}
