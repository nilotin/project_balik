using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightningPow : MonoBehaviour
{
    [Header("Targeting")]
    public string[] enemyTags = { "shark", "worm", "leviathan" };
    public float firstTargetRange = 12f;
    public float jumpRange = 8f;
    public int maxChains = 3;
    public float hitDelay = 0.07f;

    [Header("Effect")]
    public bool destroyOnHit = true;
    public float stunSeconds = 1.5f;

    public GameObject lightning;

    public GameObject lightningLinePrefab;
    public float lineLifeTime = 0.15f;

    public IEnumerator ChainLightning(Vector3 origin)
    {
        HashSet<GameObject> hit = new HashSet<GameObject>();

        GameObject current = FindClosestTarget(origin, firstTargetRange, hit);

        if (current == null)
        {
            yield break;
        }

        Vector3 spawnPoint = current.transform.position;
        spawnPoint.y += 1f;
        Instantiate(lightning, spawnPoint, Quaternion.identity);
        yield return new WaitForSeconds(0.3f);

        int count = 0;

        while (current != null && count < maxChains)
        {
            Vector3 lastPos = current.transform.position;

            DrawLightning(origin, lastPos);

            HitTarget(current);
            hit.Add(current);
            count++;

            if (hitDelay > 0f)
            {
                yield return new WaitForSeconds(hitDelay);
            }

            origin = lastPos;
            current = FindClosestTarget(origin, jumpRange, hit);
        }
    }

    private GameObject FindClosestTarget(Vector3 from, float range, HashSet<GameObject> ignore)
    {
        // range içindeki collider'ları topla
        Collider[] cols = Physics.OverlapSphere(from, range);

        GameObject best = null;
        float bestDist = range;

        foreach (var c in cols)
        {
            if (c == null)
            {
                continue;
            }

            // child collider olabilir → root/pivot olarak parent objeyi al
            GameObject go = c.attachedRigidbody != null ? c.attachedRigidbody.gameObject : c.gameObject;

            if (go == null)
            {
                continue;
            }

            if (ignore.Contains(go))
            {
                continue;
            }

            if (!HasAnyTag(go))
            {
                continue;
            }

            float d = Vector3.Distance(from, go.transform.position);
            if (d <= bestDist)
            {
                bestDist = d;
                best = go;
            }
        }

        return best;
    }

    private bool HasAnyTag(GameObject go)
    {
        for (int i = 0; i < enemyTags.Length; i++)
        {
            if (go.CompareTag(enemyTags[i]))
            {
                return true;
            }
        }

        return false;
    }

    private void HitTarget(GameObject target)
    {
        if (target == null)
        {
            return;
        }

        Debug.Log("LIGHTNING HIT: " + target.name + " tag=" + target.tag);

        if (destroyOnHit)
        {
            Destroy(target);
            return;
        }

        StartCoroutine(StunTarget(target, stunSeconds));
    }

    private IEnumerator StunTarget(GameObject target, float seconds)
    {
        if (target == null)
        {
            yield break;
        }

        // Hangi tipse onu disable et
        enemy shark = target.GetComponentInParent<enemy>();
        worm_enemy worm = target.GetComponentInParent<worm_enemy>();
        //leviathan_enemy lev = target.GetComponentInParent<leviathan_enemy>(); // <- script adın farklıysa değiştir

        if (shark != null) shark.enabled = false;
        if (worm != null) worm.enabled = false;
        //if (lev != null) lev.enabled = false;

        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Unity 2021/2022: velocity
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Unity 6 ise istersen:
            // rb.linearVelocity = Vector3.zero;
        }

        yield return new WaitForSeconds(seconds);

        if (target != null)
        {
            if (shark != null) shark.enabled = true;
            if (worm != null) worm.enabled = true;
            //if (lev != null) lev.enabled = true;
        }
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

        lr.positionCount = 2;
        lr.useWorldSpace = true;
        lr.SetPosition(0, from + Vector3.up * 0.5f);
        lr.SetPosition(1, to + Vector3.up * 0.5f);

        Destroy(lineObj, lineLifeTime);
    }
}
