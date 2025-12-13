using System.Collections.Generic;
using UnityEngine;

// Bu script, geminin konumuna göre okyanus karolarını (tile) yönetir,
// havuzlama (pooling) kullanarak performansı artırır ve içerik (balık, düşman vb.) spawnlar.
public class OceanChunkManager : MonoBehaviour
{
    // =================================================================================
    // REFERANSLAR VE AYARLAR
    // =================================================================================

    [Header("1. References")]
    [Tooltip("Takip edilecek geminin Transform bileşeni.")]
    public Transform ship;
    [Tooltip("Okyanus karo (tile) prefabı.")]
    public GameObject tilePrefab;

    [Header("2. Tile Layout & Generation")]
    [Tooltip("Bir karonun dünya koordinatlarındaki boyutu (genellikle 10).")]
    public float tileSize = 10f;
    [Tooltip("Geminin önünde kaç karo oluşturulacak.")]
    public int forwardTiles = 10;
    [Tooltip("Geminin arkasında kaç karo oluşturulacak.")]
    public int backwardTiles = 2;
    [Tooltip("Yana doğru maksimum kaç karo oluşturulacak (ilerledikçe genişler).")]
    public int maxSideTiles = 5;

    [Header("3. Visuals (Depth / Fog)")]
    [Tooltip("Sığ sulardaki renk.")]
    public Color shallowColor = new Color(0.2f, 0.6f, 0.8f);
    [Tooltip("Derin sulardaki renk.")]
    public Color deepColor = new Color(0.02f, 0.08f, 0.2f);
    [Tooltip("Okyanusun tamamen koyu renge ulaştığı mesafe.")]
    public float maxDarkDistance = 300f;
    [Range(0f, 1f)]
    [Tooltip("Sis yoğunluğu.")]
    public float fogStrength = 0.6f;

    [Header("4. Content Prefabs")]
    public GameObject[] fishPrefabs;
    public GameObject[] powerUpPrefabs;
    public GameObject[] enemyPrefabs;

    [Header("5. Spawn Chances (0–1)")]
    [Tooltip("Balık spawnlanma olasılığı (0: yok, 1: her zaman).")]
    public float fishChance = 1f;
    [Tooltip("Power-up spawnlanma olasılığı.")]
    public float powerUpChance = 0f;
    [Tooltip("Düşman spawnlanma olasılığı.")]
    public float enemyChance = 0f;

    [Header("6. Performance")]
    [Tooltip("Bir Update döngüsünde maksimum kaç yeni karo spawnlanacak.")]
    public int maxSpawnPerFrame = 20;

    // =================================================================================
    // ÖZEL ALANLAR (INTERNAL FIELDS)
    // =================================================================================

    // Aktif karoları ve koordinatlarını tutar.
    private readonly Dictionary<Vector2Int, GameObject> activeTiles = new();
    // Yeniden kullanmak üzere pasif karoları tutar (object pooling).
    private readonly Queue<GameObject> tilePool = new();
    // Karoların renklerini değiştirmek için kullanılır.
    private MaterialPropertyBlock mpb;
    // Gemi ilk başladığında alınan başlangıç noktası (Derinlik hesaplaması için).
    private Vector3 origin;
    // SpawnPoint isimlerini sabit tutmak için (Daha az hataya açık).
    private const string FISH_POINT = "FishSpawnPoint";
    private const string POWERUP_POINT = "PowerUpSpawnPoint";
    private const string ENEMY_POINT = "EnemySpawnPoint";
    private const string CONTENT_CONTAINER = "Content";

    // =================================================================================
    // UNITY METOTLARI
    // =================================================================================

    void Awake()
    {
        mpb = new MaterialPropertyBlock();
        // Gemi varsa geminin pozisyonunu, yoksa (0,0,0)'ı başlangıç noktası alır.
        origin = ship != null ? ship.position : Vector3.zero;
    }

    void Update()
    {
        if (ship == null || tilePrefab == null) 
            return;
            
        UpdateOceanChunks();
    }

    // =================================================================================
    // OYUN ALANI YÖNETİMİ
    // =================================================================================

    /// <summary>
    /// Geminin etrafında gerekli olan karoları belirler, gereksizleri yok eder ve eksikleri spawnlar.
    /// </summary>
    private void UpdateOceanChunks()
    {
        // 1. Gerekli koordinatları belirle
        HashSet<Vector2Int> neededCoords = GetNeededTileCoordinates();

        // 2. Gereksiz karoları temizle (Despawn)
        CleanUpUnusedTiles(neededCoords);

        // 3. Eksik karoları oluştur (Spawn)
        SpawnMissingTiles(neededCoords);
    }
    
    /// <summary>
    /// Geminin konumuna göre oluşturulması gereken tüm karo koordinatlarını hesaplar.
    /// </summary>
    /// <returns>Gerekli Vector2Int koordinatlarının kümesi.</returns>
    private HashSet<Vector2Int> GetNeededTileCoordinates()
    {
        HashSet<Vector2Int> needed = new();
        Vector2Int shipCoord = WorldToTileCoord(ship.position);

        for (int z = -backwardTiles; z <= forwardTiles; z++)
        {
            // İlerledikçe yana doğru genişleme için interpolasyon hesaplama
            float t = Mathf.InverseLerp(-backwardTiles, forwardTiles, z);
            int sideCount = Mathf.RoundToInt(Mathf.Lerp(1, maxSideTiles, t));

            for (int x = -sideCount; x <= sideCount; x++)
            {
                needed.Add(new Vector2Int(shipCoord.x + x, shipCoord.y + z));
            }
        }
        return needed;
    }

    /// <summary>
    /// Artık geminin görüş alanında olmayan karoları havuza (pool) geri gönderir.
    /// </summary>
    private void CleanUpUnusedTiles(HashSet<Vector2Int> neededCoords)
    {
        List<Vector2Int> toRemove = new();
        foreach (var kvp in activeTiles)
        {
            if (!neededCoords.Contains(kvp.Key))
            {
                toRemove.Add(kvp.Key);
            }
        }

        foreach (var c in toRemove)
        {
            DespawnTile(c);
        }
    }

    /// <summary>
    /// Gerekli olup da aktif olmayan karoları oluşturur. Performans sınırı koyar.
    /// </summary>
    private void SpawnMissingTiles(HashSet<Vector2Int> neededCoords)
    {
        int spawned = 0;
        foreach (var coord in neededCoords)
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


    // =================================================================================
    // KARO (TILE) YÖNETİMİ
    // =================================================================================

    /// <summary>
    /// Yeni bir karo oluşturur veya havuzdan alır ve ayarlarını yapar.
    /// </summary>
    private void SpawnTile(Vector2Int coord)
    {
        // Havuzdan al veya yeni oluştur (Pooling)
        GameObject tile = tilePool.Count > 0 ? tilePool.Dequeue() : Instantiate(tilePrefab);
        
        // Ayarları yap
        tile.transform.SetParent(transform, false);
        tile.transform.position = TileCoordToWorld(coord);
        tile.SetActive(true);

        // İçeriği ve rengi ayarla
        ClearContent(tile); // Önceki içeriği temizle
        ApplyDepthColor(tile); // Derinlik rengini uygula
        TrySpawnContent(tile); // Yeni içeriği spawnla

        activeTiles[coord] = tile;
    }

    /// <summary>
    /// Karoyu devre dışı bırakır ve havuza geri gönderir.
    /// </summary>
    private void DespawnTile(Vector2Int coord)
    {
        if (!activeTiles.TryGetValue(coord, out var tile)) return;
        
        ClearContent(tile); // İçeriği temizlemeyi unutma
        tile.SetActive(false);
        tilePool.Enqueue(tile);
        activeTiles.Remove(coord);
    }

    // =================================================================================
    // İÇERİK (CONTENT) YÖNETİMİ
    // =================================================================================

    /// <summary>
    /// Karo içinde düşman, power-up veya balık spawnlamayı dener.
    /// </summary>
    private void TrySpawnContent(GameObject tile)
    {
        Transform content = tile.transform.Find(CONTENT_CONTAINER);
        if (!content) return;

        // Düşman öncelikli spawnlanır. Spawnlanırsa geri döner.
        if (enemyPrefabs.Length > 0 && Random.value < enemyChance)
        {
            SpawnAt(content, ENEMY_POINT, enemyPrefabs);
            return;
        }

        // Power-Up spawnlanırsa geri döner.
        if (powerUpPrefabs.Length > 0 && Random.value < powerUpChance)
        {
            SpawnAt(content, POWERUP_POINT, powerUpPrefabs);
            return;
        }

        // Balık (Son şans)
        if (fishPrefabs.Length > 0 && Random.value < fishChance)
        {
            SpawnAt(content, FISH_POINT, fishPrefabs);
            // Debug Log: Balıkların spawnlandığını onaylamak için eklendi.
            // Önceki hatanın tespitinde çok faydalı oldu.
            Debug.Log($"Başarıyla spawnlanıyor: {FISH_POINT} koordinatında: {content.Find(FISH_POINT).position}");
            return;
        }
    }

    /// <summary>
    /// Belirtilen prefabı, belirtilen spawn noktasının altına child olarak oluşturur.
    /// </summary>
    private void SpawnAt(Transform contentContainer, string pointName, GameObject[] prefabs)
    {
        Transform point = contentContainer.Find(pointName);
        if (!point)
        {
            Debug.LogWarning($"SpawnPoint bulunamadı: {pointName}");
            return;
        }

        GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
        GameObject obj = Instantiate(prefab, point);

        // Yerel pozisyonu spawn noktasının çevresinde rastgele kaydırma
        obj.transform.localPosition = Vector3.zero + new Vector3(
            Random.Range(-2f, 2f),
            // Y pozisyonu kasıtlı olarak 0f'de bırakıldı. FishSpawnPoint'in Y'si ayarlanmalı!
            0f, 
            Random.Range(-2f, 2f)
        );

        obj.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
    }

    /// <summary>
    /// Karo havuza geri gönderilmeden veya yeniden kullanılmadan önce içindeki tüm içeriği temizler.
    /// </summary>
    private void ClearContent(GameObject tile)
    {
        Transform content = tile.transform.Find(CONTENT_CONTAINER);
        if (!content) return;

        // Content altındaki tüm spawn noktalarını kontrol et
        foreach (Transform point in content)
        {
            if (point.childCount > 0)
            {
                // Spawn noktasının altındaki objeyi yok et (DestroyImmediate değil!)
                Destroy(point.GetChild(0).gameObject); 
            }
        }
    }

    // =================================================================================
    // GÖRSEL AYARLAR (VISUALS)
    // =================================================================================

    /// <summary>
    /// Karo pozisyonunun başlangıç noktasına olan uzaklığına göre derinlik rengini hesaplar ve uygular.
    /// </summary>
    private void ApplyDepthColor(GameObject tile)
    {
        float dist = Vector3.Distance(tile.transform.position, origin);
        // Lerp'i yumuşatmak için bir üst alma (power) kullanıldı.
        float t = Mathf.Pow(Mathf.InverseLerp(0f, maxDarkDistance, dist), 1.8f);

        // Derinlik rengi (sığdan derine)
        Color depth = Color.Lerp(shallowColor, deepColor, t);
        // Sis efekti (derinlik rengini kamera arka plan rengine yaklaştırır)
        Color fogged = Color.Lerp(depth, Camera.main.backgroundColor, t * fogStrength);

        Renderer r = tile.GetComponent<Renderer>();
        // Renderer'a özel, paylaşılan materyali değiştirmeyen özellik bloğu (Property Block) kullanılır.
        r.GetPropertyBlock(mpb);
        mpb.SetColor("_BaseColor", fogged); // Universal RP'de _BaseColor varsayılır.
        r.SetPropertyBlock(mpb);
    }

    // =================================================================================
    // YARDIMCI METOTLAR (UTILITIES)
    // =================================================================================

    /// <summary>
    /// Dünya pozisyonunu karo koordinatlarına (Vector2Int) çevirir.
    /// </summary>
    private Vector2Int WorldToTileCoord(Vector3 pos)
    {
        // Z eksenini Y koordinatı olarak kullanır.
        return new Vector2Int(
            Mathf.FloorToInt(pos.x / tileSize),
            Mathf.FloorToInt(pos.z / tileSize)
        );
    }

    /// <summary>
    /// Karo koordinatlarını dünya pozisyonuna (Vector3) çevirir.
    /// </summary>
    private Vector3 TileCoordToWorld(Vector2Int coord)
    {
        // Y pozisyonu daima 0'dır (Deniz yüzeyi varsayımı).
        return new Vector3(coord.x * tileSize, 0f, coord.y * tileSize);
    }
}