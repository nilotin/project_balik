using UnityEngine;

public class vortexBomb : MonoBehaviour
{
    public GameObject explosionPrefab;
    public float extraGravity = 2.0f;
    public float groundY = 0f;

    private Rigidbody rb;
    private bool exploded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false; // kendi gravity’mizi ekliyoruz
        }
    }

    void FixedUpdate()
    {
        if (exploded || rb == null)
        {
            return;
        }

        rb.linearVelocity += Vector3.down * extraGravity * Time.fixedDeltaTime;

        if (transform.position.y <= groundY)
        {
            Vector2 newPos = transform.position;
            newPos.y = 0;
            transform.position = newPos;
            Explode();
        }
    }

    void Explode()
    {
        exploded = true;

        if (explosionPrefab != null)
        {
            Vector3 spawnPos = transform.position;
            spawnPos.y -=0.3f;
            Instantiate(explosionPrefab, spawnPos, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
