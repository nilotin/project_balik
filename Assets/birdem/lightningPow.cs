using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightningPow : MonoBehaviour
{


    [Header("Targeting")]
    public string enemyTag = "shark";     // senin shark tag'in neyse birebir yaz
    public float firstTargetRange = 12f;  // ship'ten ilk hedef arama
    public float jumpRange = 8f;          // sekme mesafesi
    public int maxChains = 3;             // kaç hedef (ilk dahil)
    public float hitDelay = 0.07f;

    [Header("Effect")]
    public bool destroyOnHit = true;      // TRUE: öldür, FALSE: stun
    public float stunSeconds = 1.5f;      // destroyOff iken kullanılır
    public GameObject lightning;

    public IEnumerator ChainLightning(Vector3 origin)
    {
        HashSet<enemy> hit = new HashSet<enemy>();
        enemy current = FindClosestEnemy(origin, firstTargetRange, hit);
        Vector3 spawnPoint = current.transform.position;
        spawnPoint.y+=1f;
        Instantiate(lightning,spawnPoint ,Quaternion.identity);
        yield return new WaitForSeconds(0.3f);


        int count = 0;

       while (current != null && count < maxChains)
        {
            Vector3 lastPos = current.transform.position; // ✅ destroy öncesi snapshot

            HitEnemy(current);
            hit.Add(current);
            count++;

            if (hitDelay > 0f)
                yield return new WaitForSeconds(hitDelay);

            current = FindClosestEnemy(lastPos, jumpRange, hit); // ✅ artık current'a dokunmuyor
        }

    }

    private enemy FindClosestEnemy(Vector3 from, float range, HashSet<enemy> ignore)
    {
        // Tag ile buluyoruz (jam için kolay). Obje child olabilir, InParent ile enemy buluyoruz.
        GameObject[] tagged = GameObject.FindGameObjectsWithTag(enemyTag);

        enemy best = null;
        float bestDist = range;

        foreach (var go in tagged)
        {
            if (go == null) continue;

            enemy e = go.GetComponentInParent<enemy>();
            if (e == null) continue;
            if (ignore.Contains(e)) continue;

            float d = Vector3.Distance(from, e.transform.position);
            if (d <= bestDist)
            {
                bestDist = d;
                best = e;
            }
        }

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
}
