using UnityEngine;
using System.Collections;

public class VortexField : MonoBehaviour
{
    [Header("Vortex Settings")]
    public float radius = 8f;
    public float pullStrength = 18f;
    public bool lockY = true;
    public float yOffset = 0f;

    [Header("Affect")]
    public bool disableEnemyScriptWhilePulling = false;

    public Animator anim;

    void Start()
    {
        int level = GameManager.Instance.vortexLevel;
        StartCoroutine(Dest(level));
    }

    IEnumerator Dest(int level)
    {
        float cooldown = 3f;

        if (level == 1) cooldown = 3f;
        else if (level == 2) cooldown = 5f;
        else if (level == 3) cooldown = 7f;

        yield return new WaitForSeconds(cooldown);

        if (anim != null)
            anim.SetTrigger("Disappear");

        yield return new WaitForSeconds(0.3f);
        finish();
    }

    public void finish()
    {
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        Vector3 center = transform.position + Vector3.up * yOffset;

        Collider[] cols = Physics.OverlapSphere(center, radius);
        for (int i = 0; i < cols.Length; i++)
        {
            GameObject obj = cols[i].gameObject;

            // 🔹 SADECE enemy ve worm tag’li objeler
            if (!obj.CompareTag("shark") && !obj.CompareTag("worm"))
                continue;

            enemy enemyScript = obj.GetComponent<enemy>();
            worm_enemy wormScript = obj.GetComponent<worm_enemy>();

            // İkisinden biri yoksa çekme
            if (enemyScript == null && wormScript == null)
                continue;

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb == null)
                continue;

            // 🔹 Scriptleri kapat
            if (disableEnemyScriptWhilePulling)
            {
                if (enemyScript != null)
                    enemyScript.enabled = false;

                if (wormScript != null)
                    wormScript.enabled = false;
            }

            Vector3 target = center;
            if (lockY)
                target.y = rb.position.y;

            Vector3 dir = target - rb.position;
            float dist = dir.magnitude;
            if (dist < 0.01f)
                continue;

            dir.Normalize();

            float strength = pullStrength * Mathf.Clamp01(dist / radius);

            // Unity 6
            rb.linearVelocity = dir * strength;
            // Unity 2021/2022 ise:
            // rb.velocity = dir * strength;
        }
    }

    private void OnDisable()
    {
        if (!disableEnemyScriptWhilePulling)
            return;

        // 🔹 Kapatılan scriptleri geri aç
        enemy[] enemies = Object.FindObjectsByType<enemy>(FindObjectsSortMode.None);
        foreach (var e in enemies)
        {
            if (e != null)
                e.enabled = true;
        }

        worm_enemy[] worms = Object.FindObjectsByType<worm_enemy>(FindObjectsSortMode.None);
        foreach (var w in worms)
        {
            if (w != null)
                w.enabled = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * yOffset, radius);
    }
}
