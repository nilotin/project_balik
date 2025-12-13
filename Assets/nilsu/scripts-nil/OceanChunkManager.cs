using System.Collections.Generic;
using UnityEngine;

public class OceanChunkManager : MonoBehaviour
{
    [Header("References")]
    public Transform ship;
    public GameObject tilePrefab;

    [Header("Tile Layout")]
    public float tileSize = 10f;
    public int forwardTiles = 10;
    public int backwardTiles = 2;
    public int maxSideTiles = 5;

    [Header("Depth / Fog")]
    public Color shallowColor = new Color(0.2f, 0.6f, 0.8f);
    public Color deepColor = new Color(0.02f, 0.08f, 0.2f);
    public float maxDarkDistance = 300f;
    [Range(0f, 1f)] public float fogStrength = 0.6f;

    [Header("Performance")]
    public int maxSpawnPerFrame = 20;

    // internal
    private readonly Dictionary<Vector2Int, GameObject> activeTiles = new();
    private readonly Queue<GameObject> pool = new();
    private MaterialPropertyBlock mpb;
    private Vector3 origin;

    void Awake()
    {
        mpb = new MaterialPropertyBlock();
        origin = ship != null ? ship.position : Vector3.zero;
    }

    void Update()
    {
        if (!ship || !tilePrefab) return;
        UpdateOcean();
    }

    // ================= CORE =================

    void UpdateOcean()
    {
        HashSet<Vector2Int> needed = new();

        Vector2Int shipCoord = WorldToTileCoord(ship.position);

        for (int z = -backwardTiles; z <= forwardTiles; z++)
        {
            float t = Mathf.InverseLerp(-backwardTiles, forwardTiles, z);
            int sideCount = Mathf.RoundToInt(Mathf.Lerp(1, maxSideTiles, t));

            for (int x = -sideCount; x <= sideCount; x++)
            {
                needed.Add(new Vector2Int(
                    shipCoord.x + x,
                    shipCoord.y + z
                ));
            }
        }

        // remove unused
        List<Vector2Int> toRemove = new();
        foreach (var kvp in activeTiles)
            if (!needed.Contains(kvp.Key))
                toRemove.Add(kvp.Key);

        foreach (var c in toRemove)
            DespawnTile(c);

        // spawn missing
        int spawned = 0;
        foreach (var coord in needed)
        {
            if (!activeTiles.ContainsKey(coord))
            {
                SpawnTile(coord);
                spawned++;
                if (spawned >= maxSpawnPerFrame)
                    break;
            }
        }
    }

    // ================= TILE =================

    void SpawnTile(Vector2Int coord)
    {
        GameObject tile = pool.Count > 0 ? pool.Dequeue() : Instantiate(tilePrefab);
        tile.transform.SetParent(transform, false);

        Vector3 worldPos = TileCoordToWorld(coord);
        tile.transform.position = worldPos;
        tile.SetActive(true);

        ApplyDepthColor(tile, worldPos);

        activeTiles[coord] = tile;
    }

    void DespawnTile(Vector2Int coord)
    {
        if (!activeTiles.TryGetValue(coord, out var tile)) return;
        activeTiles.Remove(coord);
        tile.SetActive(false);
        pool.Enqueue(tile);
    }

    // ================= VISUAL =================

    void ApplyDepthColor(GameObject tile, Vector3 worldPos)
    {
        float dist = Vector3.Distance(worldPos, origin);
        float t = Mathf.InverseLerp(0f, maxDarkDistance, dist);
        t = Mathf.Pow(t, 1.8f); // yumuşak eğri

        Color depth = Color.Lerp(shallowColor, deepColor, t);
        Color fogged = Color.Lerp(depth, Camera.main.backgroundColor, t * fogStrength);

        Renderer r = tile.GetComponent<Renderer>();
        r.GetPropertyBlock(mpb);
        mpb.SetColor("_BaseColor", fogged);
        r.SetPropertyBlock(mpb);
    }

    // ================= UTILS =================

    Vector2Int WorldToTileCoord(Vector3 pos)
    {
        return new Vector2Int(
            Mathf.FloorToInt(pos.x / tileSize),
            Mathf.FloorToInt(pos.z / tileSize)
        );
    }

    Vector3 TileCoordToWorld(Vector2Int coord)
    {
        return new Vector3(
            coord.x * tileSize,
            0f,
            coord.y * tileSize
        );
    }
}
