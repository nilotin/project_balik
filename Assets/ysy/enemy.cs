using UnityEngine;

public class enemy : MonoBehaviour
{
    public Transform ship;
    public string shipTag = "ship";

    public float moveSpeed = 3f;

    public float chaseRange = 10f;   // <-- YENİ
    public float stopDistance = 2.5f;

    public bool lockY = true;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (ship == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag(shipTag);
            if (found != null)
            {
                ship = found.transform;
            }
        }
        ship = GameManager.Instance.ship.GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        if (ship == null)
        {
            return;
        }

        Vector3 target = ship.position;

        if (lockY)
        {
            target.y = transform.position.y;
        }

        float dist = Vector3.Distance(transform.position, target);

        if (dist > chaseRange)
        {
            Stop();
            return;
        }

        if (dist <= stopDistance)
        {
            Stop();
            return;
        }

        Vector3 dir = (target - transform.position).normalized;

        if (rb != null)
        {
            rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            transform.position += dir * moveSpeed * Time.fixedDeltaTime;
        }
    }
    
    // Örnek Düşman Çarpışma Scripti (Enemy.cs)

// Düşmanın tipine göre hasar miktarını belirle
    public int damageAmount = 1; // Varsayılan: Worm ve Shark için 1

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ship")) // Geminizin tag'inin "Ship" olduğunu varsayıyorum
        {
            ShipHealth shipHealth = other.GetComponent<ShipHealth>();
        
            if(GetComponent<state>().isFrozen == false && GameManager.Instance.IsInvincible == false && GameManager.Instance.IsUntouchable == false)
            {
                
                shipHealth.TakeDamage(damageAmount);    

                Vector3 knockDir =
                    transform.position + other.transform.position;

                ShipMovement move = GameManager.Instance.ship.GetComponent<ShipMovement>();
                if (move != null)
                {
                    move.ApplyKnockback(knockDir, GameManager.Instance.knockbackForce, GameManager.Instance.knockbackDuration);
                }

            }
                
            else if(GetComponent<state>().isFrozen)
            {
                Instantiate(GameManager.Instance.IceCollideEffect, transform.position,Quaternion.identity);
                int cur = GameManager.Instance.GetCurrency();
                GameManager.Instance.SetCurrency(cur+4);
                Destroy(gameObject); 
            }
            else if(GameManager.Instance.IsInvincible)
            {
                //Instantiate(GameManager.Instance.collideEffect, transform.position,Quaternion.identity);
                int cur = GameManager.Instance.GetCurrency();
                GameManager.Instance.SetCurrency(cur+4);
                Destroy(gameObject); 
            }
        
        }
    }

    void Stop()
    {
        if (rb != null)
        {
            // Unity 2021 / 2022
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Unity 6 ise:
            // rb.linearVelocity = Vector3.zero;
        }
    }
}
