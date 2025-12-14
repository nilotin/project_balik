using UnityEngine;
using System.Collections;

public class VortexField : MonoBehaviour
{
    [Header("Vortex Settings")]
    public float radius = 8f;
    public float pullStrength = 18f;      // çekiş gücü
    public bool lockY = true;             // 2.5D için Y kilit
    public float yOffset = 0f;            // merkez yüksekliği

    [Header("Affect")]
    public bool disableEnemyScriptWhilePulling = false; // true yaparsan enemy kovalamayı keser

    public Animator anim;

    void Start()
    {
        int cooldown = GameManager.Instance.vortexLevel;
        StartCoroutine(Dest(cooldown));
    }

    IEnumerator Dest(int level)
    {
        float cooldown = 3f;
        if(level == 1)
            cooldown = 3f;
        else if(level == 2)
            cooldown =5f;
        else if(level == 3)
            cooldown = 7f;

        yield return new WaitForSeconds(cooldown);
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
            enemy e = cols[i].GetComponentInParent<enemy>();
            if (e == null) continue;

            Rigidbody rb = e.GetComponent<Rigidbody>();
            if (rb == null) continue;

            if (disableEnemyScriptWhilePulling)
                e.enabled = false;

            Vector3 target = center;
            if (lockY)
                target.y = rb.position.y;

            Vector3 dir = (target - rb.position);
            float dist = dir.magnitude;
            if (dist < 0.01f) continue;

            dir /= dist;

            // Merkeze yaklaştıkça biraz yumuşasın
            float strength = pullStrength * Mathf.Clamp01(dist / radius);

            // Unity 6: linearVelocity kullanıyorsun, o yüzden burada da onu setleyelim
            rb.linearVelocity = dir * strength;
        }
    }

    private void OnDisable()
    {
        // Vortex yok olunca enemy scriptlerini geri açmak istersen:
        if (!disableEnemyScriptWhilePulling) return;

        enemy[] enemies = Object.FindObjectsByType<enemy>(FindObjectsSortMode.None);
        foreach (var e in enemies)
        {
            if (e != null) e.enabled = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * yOffset, radius);
    }
}
