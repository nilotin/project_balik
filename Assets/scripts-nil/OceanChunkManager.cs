using System.Collections.Generic;
using UnityEngine;

public class OceanChunkManager : MonoBehaviour
{
    [Header("References")]
    public Transform target;          // Gemi
    public GameObject tilePrefab;     // SeaTile prefab

    [Header("Tiling")]
    public float tileSize = 10f;      // Unity Plane default 10
    public int radiusInTiles = 4;     // 4 => (2*4+1)=9 tile genişliği

    [Header("Performance")]
    public int maxTilesPerFrame = 20; // spike olmasın

    // aktif tile’lar (grid koordinatı -> tile objesi)
    private readonly Dictionary<Vector2Int, GameObject> activeTiles = new();
    private readonly Queue<GameObject> pool = new();

    private Vector2Int lastCenterCoord = new(int.MinValue, int.MinValue);

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("OceanChunkManager: target (ship) is not assigned.");
            enabled = false;
            return;
        }
        if (tilePrefab == null)
        {
            Debug.LogError("OceanChunkManager: tilePrefab is not assigned.");
            enabled = false;
            return;
        }

        ForceRefresh();
    }

    void Update()
    {
        Vector2Int centerCoord = WorldToTileCoord(target.position);

        // Gemi yeni tile’a geçtiyse güncelle
        if (centerCoord != lastCenterCoord)
        {
            lastCenterCoord = centerCoord;
            UpdateTiles(centerCoord);
        }
    }

    public void ForceRefresh()
    {
        lastCenterCoord = WorldToTileCoord(target.position);
        UpdateTiles(lastCenterCoord, force: true);
    }

    Vector2Int WorldToTileCoord(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / tileSize);
        int z = Mathf.FloorToInt(worldPos.z / tileSize);
        return new Vector2Int(x, z);
    }

    Vector3 TileCoordToWorld(Vector2Int coord)
    {
        return new Vector3(coord.x * tileSize, 0f, coord.y * tileSize);
    }

    void UpdateTiles(Vector2Int centerCoord, bool force = false)
    {
        // 1) Hedef tile setini hesapla
        HashSet<Vector2Int> needed = new();
        for (int dz = -radiusInTiles; dz <= radiusInTiles; dz++)
        {
            for (int dx = -radiusInTiles; dx <= radiusInTiles; dx++)
            {
                needed.Add(new Vector2Int(centerCoord.x + dx, centerCoord.y + dz));
            }
        }

        // 2) Gerekmeyenleri kaldır (pool’a at)
        // Not: dictionary üzerinde iterate ederken remove etmeyelim diye liste yapıyoruz.
        List<Vector2Int> toRemove = new();
        foreach (var kvp in activeTiles)
        {
            if (!needed.Contains(kvp.Key))
                toRemove.Add(kvp.Key);
        }

        foreach (var coord in toRemove)
        {
            DespawnTile(coord);
        }

        // 3) Eksik olanları spawn et (frame’e yay)
        int spawnedThisFrame = 0;
        foreach (var coord in needed)
        {
            if (!activeTiles.ContainsKey(coord))
            {
                SpawnTile(coord);
                spawnedThisFrame++;
                if (!force && spawnedThisFrame >= maxTilesPerFrame)
                    break;
            }
        }
    }

    void SpawnTile(Vector2Int coord)
    {
        GameObject tile = (pool.Count > 0) ? pool.Dequeue() : Instantiate(tilePrefab);

        tile.transform.SetParent(transform, true);

        Vector3 pos = TileCoordToWorld(coord);
        tile.transform.position = pos;

        // Eğer tilePrefab plane ise, y = 0’da dursun; istersen su seviyeni burada ayarla
        // tile.transform.position = new Vector3(pos.x, waterY, pos.z);

        tile.SetActive(true);
        activeTiles[coord] = tile;
    }

    void DespawnTile(Vector2Int coord)
    {
        if (!activeTiles.TryGetValue(coord, out var tile)) return;

        activeTiles.Remove(coord);
        tile.SetActive(false);
        pool.Enqueue(tile);
    }
}
