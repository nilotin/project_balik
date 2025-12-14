using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powUpLightning : MonoBehaviour
{
    [Header("Pickup")]
    public Collider col;
    public GameObject spriteObject;

    [Header("Targeting")]
    public string enemyTag = "shark";     // senin shark tag'in neyse birebir yaz
    public float firstTargetRange = 12f;  // ship'ten ilk hedef arama
    public float jumpRange = 8f;          // sekme mesafesi
    public int maxChains = 3;             // kaç hedef (ilk dahil)
    public float hitDelay = 0.07f;

    [Header("Effect")]
    public bool destroyOnHit = true;      // TRUE: öldür, FALSE: stun
    public float stunSeconds = 1.5f;      // destroyOff iken kullanılır

    [Header("Chain Lightning Line")]
    public GameObject lightningLinePrefab;
    public float lineLifeTime = 0.15f;



    void Start()
    {
        //lightningLinePrefab = lightningLine.GetComponent<LineRenderer>();
    }

    private void Reset()
    {
        // otomatik doldursun diye
        if (col == null) col = GetComponent<Collider>();
        if (spriteObject == null && transform.childCount > 0) spriteObject = transform.GetChild(0).gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ship"))
        {
            if (col == null) col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            if (spriteObject != null) spriteObject.SetActive(false);

            StartCoroutine(ChainLightning(other.transform.position));
        }
    }

    private IEnumerator ChainLightning(Vector3 origin)
    {
         HashSet<enemy> hit = new HashSet<enemy>();
         enemy current = FindClosestEnemy(origin, firstTargetRange, hit);

        int count = 0;

        while (current != null && count < maxChains)
        {
            Vector3 lastPos = current.transform.position;

            // <<< İŞTE BURASI >>>
            if (count >= 0) // TEST
            {
                DrawLightning(origin, lastPos);
            }

            HitEnemy(current);
            hit.Add(current);
            count++;

            if (hitDelay > 0f)
                yield return new WaitForSeconds(hitDelay);

            origin = lastPos;
            current = FindClosestEnemy(origin, jumpRange, hit);
        }

    }

  private enemy FindClosestEnemy(Vector3 from, float range, HashSet<enemy> ignore)
{
    // Unity 6: sahnedeki tüm enemy'leri bul
    enemy[] enemies = Object.FindObjectsByType<enemy>(FindObjectsSortMode.None);

    enemy best = null;
    float bestDist = range;

    foreach (var e in enemies)
    {
        if (e == null) continue;
        if (ignore.Contains(e)) continue;

        float d = Vector3.Distance(from, e.transform.position);
        if (d <= bestDist)
        {
            bestDist = d;
            best = e;
        }
    }

    Debug.Log("Enemies found: " + enemies.Length + " best=" + (best ? best.name : "NONE"));
    return best;
}


    private void HitEnemy(enemy e)
    {
        if (e == null) return;

        Debug.Log("LIGHTNING HIT: " + e.name);

        if (destroyOnHit)
        {
            // EN ÖNEMLİ: root/parent silinsin diye enemy'nin gameObject'ini sil
            Destroy(e.gameObject);
            return;
        }

        // Stun mode: enemy scriptini kapat, rigidbody'yi durdur, sonra aç
        StartCoroutine(StunEnemy(e, stunSeconds));
    }

    private IEnumerator StunEnemy(enemy e, float seconds)
    {
        if (e == null) yield break;

        // enemy hareketini durdur
        e.enabled = false;

        Rigidbody rb = e.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        yield return new WaitForSeconds(seconds);

        if (e != null)
            e.enabled = true;
    }

    private void DrawLightning(Vector3 from, Vector3 to)
    {
         Debug.DrawLine(from + Vector3.up * 0.5f, to + Vector3.up * 0.5f, Color.cyan, 1.0f);

        if (lightningLinePrefab == null)
        {
            Debug.LogWarning("lightningLinePrefab is NULL!");
            return;
        }

        GameObject lineObj = Instantiate(lightningLinePrefab);

        LineRenderer lr = lineObj.GetComponent<LineRenderer>();
        if (lr == null)
        {
            lr = lineObj.GetComponentInChildren<LineRenderer>();
        }

        if (lr == null)
        {
            Debug.LogError("No LineRenderer found on lightningLinePrefab instance!");
            Destroy(lineObj);
            return;
        }

        Debug.Log("Drawing line from " + from + " to " + to);

        lr.positionCount = 2;
        lr.useWorldSpace = true; // garanti
        lr.SetPosition(0, from + Vector3.up * 0.5f);
        lr.SetPosition(1, to + Vector3.up * 0.5f);

        Destroy(lineObj, lineLifeTime);
    }


}
