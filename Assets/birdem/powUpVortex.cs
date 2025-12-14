using UnityEngine;

public class powUpVortex : MonoBehaviour
{
    [Header("Pickup")]
    public Collider col;
    public GameObject spriteObject;

    [Header("Spawn")]
    public GameObject vortexPrefab;   // VortexField olan prefab
    public float vortexDuration = 3.0f;
    public float spawnYOffset = 0f;

    private void Reset()
    {
        if (col == null) col = GetComponent<Collider>();
        if (spriteObject == null && transform.childCount > 0) spriteObject = transform.GetChild(0).gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("ship")) return;

        if (col == null) col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
        if (spriteObject != null) spriteObject.SetActive(false);

        SpawnVortex(other.transform.position);
        Destroy(gameObject, 0.1f); // pickup temizlensin
    }

    private void SpawnVortex(Vector3 shipPos)
    {
        if (vortexPrefab == null)
        {
            Debug.LogWarning("Vortex prefab not set!");
            return;
        }

        // En yakın enemy'yi bul
        enemy[] enemies = Object.FindObjectsByType<enemy>(FindObjectsSortMode.None);

        Vector3 spawnPos = shipPos; // fallback: ship
        float best = float.MaxValue;
        enemy bestEnemy = null;

        foreach (var e in enemies)
        {
            if (e == null) continue;

            float d = Vector3.Distance(shipPos, e.transform.position);
            if (d < best)
            {
                best = d;
                bestEnemy = e;
            }
        }

        // Eğer enemy bulunduysa onun konumuna spawn et
        if (bestEnemy != null)
            spawnPos = bestEnemy.transform.position;

        spawnPos.y += spawnYOffset;

        GameObject v = Instantiate(vortexPrefab, spawnPos, Quaternion.identity);
        Destroy(v, vortexDuration);
    }

}
