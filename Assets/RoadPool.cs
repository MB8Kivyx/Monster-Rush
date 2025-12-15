// using UnityEngine;

// public class EndlessRoad : MonoBehaviour
// {
//     [Header("Assign in Inspector")]
//     public Transform road1;
//     public Transform road2;
//     public float speed = 3f;  // Slow rakho testing ke liye

//     private float roadHeight;
//     private Camera cam;  // Camera bounds ke liye

//     void Start()
//     {
//         if (road1 == null || road2 == null)
//         {
//             Debug.LogError("Road1 or Road2 NOT ASSIGNED! Inspector check karo.");
//             return;
//         }

//         // Accurate height (scale consider karta hai)
//         roadHeight = road1.GetComponent<SpriteRenderer>().bounds.size.y;
//         Debug.Log("Road Height: " + roadHeight);  // Console mein dekho!

//         // Road2 ko perfectly upar set
//         road2.position = new Vector3(road2.position.x, road1.position.y + roadHeight, road2.position.z);
//         Debug.Log("Road2 new Y: " + road2.position.y);

//         cam = Camera.main;
//     }

//     void Update()
//     {
//         // Scroll down (player forward simulate)
//         Vector3 move = Vector3.down * speed * Time.deltaTime;
//         road1.Translate(move, Space.World);
//         road2.Translate(move, Space.World);

//         // Reset logic: Jab fully off-screen (camera se bahar)
//         if (road1.position.y + roadHeight/2 < cam.transform.position.y - cam.orthographicSize)
//         {
//             road1.position = new Vector3(road1.position.x, road2.position.y + roadHeight, road1.position.z);
//             Debug.Log("Road1 RESET!");
//         }

//         if (road2.position.y + roadHeight/2 < cam.transform.position.y - cam.orthographicSize)
//         {
//             road2.position = new Vector3(road2.position.x, road1.position.y + roadHeight, road2.position.z);
//             Debug.Log("Road2 RESET!");
//         }
//     }
// }












using System.Collections.Generic;
using UnityEngine;

public class RoadPool : MonoBehaviour
{
    [Header("Setup")]
    public GameObject roadPrefab;  // Road prefab drag
    public Transform player;       // Player drag (auto-find bhi)

    [Header("Tuning")]
    public int poolSize = 5;       // 5 roads enough
    public float spawnAhead = 15f; // Jab player itna close end pe, spawn next (roadLength * 1.5)
    public float destroyBehind = 25f; // Behind destroy/recycle

    private Queue<GameObject> roadPool = new Queue<GameObject>();
    private List<GameObject> activeRoads = new List<GameObject>();
    private float roadLength, halfLength;

    void Start()
    {
        if (roadPrefab == null) { Debug.LogError("Road Prefab assign!"); return; }

        // Auto measure length
        GameObject tempRoad = Instantiate(roadPrefab);
        roadLength = tempRoad.GetComponent<SpriteRenderer>().bounds.size.y;
        halfLength = roadLength * 0.5f;
        DestroyImmediate(tempRoad);
        Debug.Log("✅ Road Length: " + roadLength);

        // Auto player
        if (player == null) player = GameObject.FindWithTag("Player").transform;
        if (player == null) { Debug.LogError("Player tag 'Player' lagao!"); return; }

        // Pool banao
        for (int i = 0; i < poolSize; i++)
        {
            GameObject road = Instantiate(roadPrefab, Vector3.zero, Quaternion.identity, transform);
            road.SetActive(false);
            roadPool.Enqueue(road);
        }

        // Initial 3 roads spawn (player ke neeche + agay)
        SpawnInitialRoads();
        Debug.Log("✅ Pool Ready! Active Roads: " + activeRoads.Count);
    }

    void SpawnInitialRoads()
    {
        // First under player
        Vector3 pos1 = new Vector3(0, player.position.y - halfLength, 10);
        GetRoad(pos1);

        // Second ahead
        Transform last = activeRoads[^1].transform;
        SpriteRenderer lastSR = last.GetComponent<SpriteRenderer>();
        Vector3 pos2 = new Vector3(0, lastSR.bounds.max.y - halfLength, 10);
        GetRoad(pos2);

        // Third ahead
        last = activeRoads[^1].transform;
        lastSR = last.GetComponent<SpriteRenderer>();
        Vector3 pos3 = new Vector3(0, lastSR.bounds.max.y - halfLength, 10);
        GetRoad(pos3);
    }

    void Update()
    {
        if (activeRoads.Count == 0 || player == null) return;

        // Next spawn jab player last road ke end ke paas
        Transform lastRoad = activeRoads[^1].transform;
        SpriteRenderer lastSR = lastRoad.GetComponent<SpriteRenderer>();
        if (player.position.y > lastSR.bounds.max.y - spawnAhead)
        {
            Vector3 nextPos = new Vector3(0, lastSR.bounds.max.y - halfLength, 10);
            GetRoad(nextPos);
            Debug.Log("🔥 NEW ROAD SPAWNED at Y=" + nextPos.y);
        }

        // Recycle behind wale
        for (int i = activeRoads.Count - 1; i >= 0; i--)
        {
            GameObject roadGO = activeRoads[i];
            SpriteRenderer sr = roadGO.GetComponent<SpriteRenderer>();
            if (sr.bounds.max.y < player.position.y - destroyBehind)
            {
                ReturnRoad(roadGO);
                Debug.Log("♻️ Road RECYCLED");
            }
        }
    }

    GameObject GetRoad(Vector3 position)
    {
        GameObject road;
        if (roadPool.Count > 0)
        {
            road = roadPool.Dequeue();
        }
        else
        {
            road = Instantiate(roadPrefab, position, Quaternion.identity, transform);
        }
        road.transform.position = position;
        road.SetActive(true);
        activeRoads.Add(road);
        return road;
    }

    void ReturnRoad(GameObject road)
    {
        road.SetActive(false);
        roadPool.Enqueue(road);
        activeRoads.Remove(road);
    }
}