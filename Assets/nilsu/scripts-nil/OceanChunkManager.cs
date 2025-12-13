using System.Collections.Generic;
using UnityEngine;

// Bu script, geminin konumuna göre okyanus karolarını (tile) yönetir,
// havuzlama (pooling) kullanarak performansı artırır ve içerik (balık, düşman vb.) spawnlar.
public class OceanChunkManager : MonoBehaviour
{
    // =================================================================================
    // YAPISAL AYARLAR (STRUCTS)
    // =================================================================================

    // Balık nadirliğini ve spawn şansını uzaklığa göre ayarlayan yapı
    [System.Serializable]
    public struct FishRaritySettings
    {
        [Tooltip("Bu nadirliğe ait balıkların prefab dizisi.")]
        public GameObject[] prefabs;
        [Tooltip("Bu nadirlik için taban spawn şansı (0-1).")]
        [Range(0f, 1f)] public float baseChance;
        [Tooltip("Bu nadirliğin şansının artmaya başladığı uzaklık (m).")]
        public float distanceStart;
        [Tooltip("Bu nadirliğin maksimum şansa ulaştığı uzaklık (m).")]
        public float distanceMax;
        [Tooltip("Fish bileşenine atanacak nadirlik değeri (Örn: Common, Rare, Legandary).")]
        public string rarityName; 
    }

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

    [Header("4. General Content")]
    // Not: Balık prefabları artık raritySettings içinde yönetiliyor
    public GameObject[] powerUpPrefabs;
    public GameObject[] enemyPrefabs;

    [Header("5. Spawn Chances (0–1)")]
    [Tooltip("Balık spawnlanma olasılığı (Nadirliğe geçmeden önceki ana şans).")]
    public float fishChance = 1f;
    [Tooltip("Power-up spawnlanma olasılığı.")]
    public float powerUpChance = 0f;
    [Tooltip("Düşman spawnlanma olasılığı.")]
    public float enemyChance = 0f;

    [Header("6. Fish Rarity & Distance")]
    [Tooltip("Balık nadirlik seviyelerinin ayarları (Ortak, Nadir, Efsanevi).")]
    public FishRaritySettings[] raritySettings;

    [Header("7. Performance")]
    [Tooltip("Bir Update döngüsünde maksimum kaç yeni karo spawnlanacak.")]
    public int maxSpawnPerFrame = 20;

    // =================================================================================
    // ÖZEL ALANLAR (INTERNAL FIELDS)
    // =================================================================================

    private readonly Dictionary<Vector2Int, GameObject> activeTiles = new();
    private readonly Queue<GameObject> tilePool = new();
    private MaterialPropertyBlock mpb;
    private Vector3 origin;
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
        HashSet<Vector2Int> neededCoords = GetNeededTileCoordinates();
        CleanUpUnusedTiles(neededCoords);
        SpawnMissingTiles(neededCoords);
    }
    
    /// <summary>
    /// Geminin konumuna göre oluşturulması gereken tüm karo koordinatlarını hesaplar.
    /// </summary>
    private HashSet<Vector2Int> GetNeededTileCoordinates()
    {
        HashSet<Vector2Int> needed = new();
        Vector2Int shipCoord = WorldToTileCoord(ship.position);

        for (int z = -backwardTiles; z <= forwardTiles; z++)
        {
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
        GameObject tile = tilePool.Count > 0 ? tilePool.Dequeue() : Instantiate(tilePrefab);
        
        tile.transform.SetParent(transform, false);
        tile.transform.position = TileCoordToWorld(coord);
        tile.SetActive(true);

        ClearContent(tile); 
        ApplyDepthColor(tile); 
        TrySpawnContent(tile); 

        activeTiles[coord] = tile;
    }

    /// <summary>
    /// Karoyu devre dışı bırakır ve havuza geri gönderir.
    /// </summary>
    private void DespawnTile(Vector2Int coord)
    {
        if (!activeTiles.TryGetValue(coord, out var tile)) return;
        
        ClearContent(tile); 
        tile.SetActive(false);
        tilePool.Enqueue(tile);
        activeTiles.Remove(coord);
    }

    // =================================================================================
    // İÇERİK (CONTENT) YÖNETİMİ - GÜNCEL VE NADİRLİK KONTROLLÜ
    // =================================================================================

    /// <summary>
    /// Karo içinde rastgele 0 ile 3 arasında içerik spawnlamayı dener.
    /// Her spawn noktasında sadece bir obje olmasına izin verir.
    /// Balık nadirliği uzaklığa göre hesaplanır.
    /// </summary>
    private void TrySpawnContent(GameObject tile)
    {
        Transform content = tile.transform.Find(CONTENT_CONTAINER);
        if (!content) return;

        // 1. O karoya kaç tane obje spawnlanacağına rastgele karar ver (0, 1, 2 veya 3)
        int spawnCount = Random.Range(0, 4); 

        // 2. Belirlenen sayı kadar spawn döngüsü başlat
        for (int i = 0; i < spawnCount; i++)
        {
            // Düşman öncelikli kontrol
            if (enemyPrefabs.Length > 0 && Random.value < enemyChance)
            {
                Transform point = content.Find(ENEMY_POINT);
                if (point != null && point.childCount == 0)
                {
                    SpawnGenericContentAt(content, ENEMY_POINT, enemyPrefabs);
                    continue;
                }
            }

            // Power-Up kontrolü
            else if (powerUpPrefabs.Length > 0 && Random.value < powerUpChance)
            {
                Transform point = content.Find(POWERUP_POINT);
                if (point != null && point.childCount == 0)
                {
                    SpawnGenericContentAt(content, POWERUP_POINT, powerUpPrefabs);
                    continue;
                }
            }
        
            // Balık kontrolü (Nadirliğe göre spawn eden özel metot çağrılır)
            else if (raritySettings.Length > 0 && Random.value < fishChance)
            {
                Transform point = content.Find(FISH_POINT);
                if (point != null && point.childCount == 0)
                {
                    SpawnFishAt(content);
                    continue;
                }
            }
        }
    }

    /// <summary>
    /// Balık prefabını seçer, nadirliğini hesaplar ve spawn eder.
    /// </summary>
    private void SpawnFishAt(Transform contentContainer)
    {
        Transform point = contentContainer.Find(FISH_POINT);
        if (!point) return;
        
        // 1. Uzaklığı hesapla
        float distance = Vector3.Distance(point.position, origin);
        
        // 2. Nadirlik seçimi için toplam şansı ve zarı belirle
        float totalChance = 0f;
        foreach (var setting in raritySettings)
        {
            float t = Mathf.InverseLerp(setting.distanceStart, setting.distanceMax, distance);
            // Uzaklaştıkça şans artar (0'dan 1'e Lerp)
            float currentChance = Mathf.Lerp(setting.baseChance, 1f, t); 
            totalChance += currentChance;
        }

        // Toplam şansa göre zar atılır
        float roll = Random.Range(0f, totalChance > 0 ? totalChance : 1f); 
        
        GameObject selectedPrefab = null;
        string selectedRarity = "Common"; 
        float cumulativeChance = 0f;
        
        // 3. Hangi nadirliğin seçildiğini bul
        foreach (var setting in raritySettings)
        {
            float t = Mathf.InverseLerp(setting.distanceStart, setting.distanceMax, distance);
            float currentChance = Mathf.Lerp(setting.baseChance, 1f, t);
            
            cumulativeChance += currentChance;
            
            if (roll <= cumulativeChance)
            {
                selectedRarity = setting.rarityName;
                if (setting.prefabs.Length > 0)
                {
                    selectedPrefab = setting.prefabs[Random.Range(0, setting.prefabs.Length)];
                }
                break; 
            }
        }
        
        // 4. Spawn etme ve nadirliği atama
        if (selectedPrefab == null)
        {
            Debug.LogWarning("Balık spawnlanamadı: Prefab bulunamadı veya nadirlik dizisi boş.");
            return;
        }

        GameObject obj = Instantiate(selectedPrefab, point);

        // Nadirliği atama (Fish component'inin 'rarity' adında Rarity Enum'u kullandığı varsayılır)
        fish fishComponent = obj.GetComponent<fish>(); // fish.cs içindeki sınıf adı büyük ihtimalle 'fish' değil, 'Fish' olmalı
        if (fishComponent != null)
        {
            try
            {
                // String'i Rarity Enum'una dönüştürerek atama yap
                System.Type rarityEnumType = System.Type.GetType("Rarity"); // Global Enum'u arama
                if (rarityEnumType == null)
                {
                    // Eğer Rarity Enum'u 'fish' sınıfının içinde tanımlıysa (gönderdiğiniz kodda öyle)
                    rarityEnumType = typeof(Rarity); 
                }

                if (rarityEnumType != null)
                {
                    object newRarity = System.Enum.Parse(rarityEnumType, selectedRarity);
                    fishComponent.rarity = (Rarity)newRarity; 
                }
                else
                {
                    Debug.LogError("Rarity Enum tipi bulunamadı! Lütfen Fish scriptindeki sınıf adını kontrol edin.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Nadirliği atarken hata oluştu. Ayar Adı: {selectedRarity}, Hata: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"Spawnlanan balık prefabında Fish bileşeni bulunamadı: {selectedPrefab.name}");
        }

        // Pozisyonu rastgele kaydırma
        obj.transform.localPosition = Vector3.zero + new Vector3(
            Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f)
        );
        obj.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
    }

    /// <summary>
    /// Düşman ve Power-Up gibi genel içerikleri spawn eder.
    /// </summary>
    private void SpawnGenericContentAt(Transform contentContainer, string pointName, GameObject[] prefabs)
    {
        Transform point = contentContainer.Find(pointName);
        if (!point) return;
        
        GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
        GameObject obj = Instantiate(prefab, point);

        // Pozisyonu rastgele kaydırma
        obj.transform.localPosition = Vector3.zero + new Vector3(
            Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f)
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

        foreach (Transform point in content)
        {
            if (point.childCount > 0)
            {
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
        float t = Mathf.Pow(Mathf.InverseLerp(0f, maxDarkDistance, dist), 1.8f);

        Color depth = Color.Lerp(shallowColor, deepColor, t);
        Color fogged = Color.Lerp(depth, Camera.main.backgroundColor, t * fogStrength);

        Renderer r = tile.GetComponent<Renderer>();
        r.GetPropertyBlock(mpb);
        mpb.SetColor("_BaseColor", fogged);
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
        return new Vector3(coord.x * tileSize, 0f, coord.y * tileSize);
    }
}