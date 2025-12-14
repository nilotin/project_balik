using UnityEngine;
using System.Collections;

public class worm_enemy : MonoBehaviour
{
    [Header("References")]
    public Transform ship;

    [Header("Spawn Prefabs")]
    public GameObject warningPrefab1;
    public GameObject warningPrefab2;

    [Header("Logic")]
    public float triggerDistance = 6f;
    public float secondSpawnDelay = 1.5f;
    public float cooldown = 3f;
    public bool lockY = true;

    private bool isRunning;

    void Start()
    {
        if (ship == null)
        {
            GameObject found = GameManager.Instance.ship;
            if (found != null)
            {
                ship = found.transform;
            }
        }
    }

    void Update()
    {
        if (isRunning || ship == null)
        {
            return;
        }

        float dist = Vector3.Distance(transform.position, ship.position);

        if (dist <= triggerDistance)
        {
            StartCoroutine(SpawnWarningSequence());
        }
    }

    private IEnumerator SpawnWarningSequence()
    {
        isRunning = true;

        Vector3 spawnPosWarning = ship.transform.position;
        Vector3 spawnPosAttack = ship.transform.position;

        if (lockY)
        {
            spawnPosWarning.y = ship.position.y -0.5f; // istersen enemy.y da yapabilirsin
            spawnPosAttack.y = spawnPosAttack.y+1.5f;
        }

        if (warningPrefab1 != null)
        {
            Instantiate(warningPrefab1, spawnPosWarning, Quaternion.identity);
        }

        yield return new WaitForSeconds(secondSpawnDelay);

        if (warningPrefab2 != null)
        {
            Instantiate(warningPrefab2, spawnPosAttack, Quaternion.identity);
        }

        yield return new WaitForSeconds(cooldown);

        isRunning = false;
    }
    
    // Örnek Düşman Çarpışma Scripti (Enemy.cs)

// Düşmanın tipine göre hasar miktarını belirle
    public int damageAmount = 1; // Varsayılan: Worm ve Shark için 1

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ship")) // Geminizin tag'inin "Ship" olduğunu varsayıyorum
        {
            ShipHealth shipHealth = other.GetComponent<ShipHealth>();
        
            if (shipHealth != null)
            {
                // damageAmount'ı düşman tipine göre Inspector'da ayarlayın:
                // Worm/Shark: 1
                // Leviathan: 2
                shipHealth.TakeDamage(damageAmount);
            }
        
            // Düşman hasar verdikten sonra yok edilebilir:
            Destroy(gameObject); 
        }
    }
}
