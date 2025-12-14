using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class icePowerUp : MonoBehaviour
{
    public Animator anim;
    public GameObject iceObj; // prefab

    private float waitTime = 0f;

    // Donan objeler + üstlerine bastığımız buz görseli
    private readonly List<GameObject> frozenTargets = new List<GameObject>();
    private readonly Dictionary<GameObject, GameObject> iceInstances = new Dictionary<GameObject, GameObject>();

    void Start()
    {
        waitTime = 0f;

        if (GameManager.Instance.freezeLevel == 1)
        {
            waitTime = 3f;
        }
        else if (GameManager.Instance.freezeLevel == 2)
        {
            waitTime = 5f;
        }
        else
        {
            waitTime = 3f;
        }

        StartCoroutine(DefrostAfter(waitTime));
    }

    private IEnumerator DefrostAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (anim != null)
        {
            anim.SetTrigger("finish");
        }

        // Listedeki herkesi çöz
        for (int i = 0; i < frozenTargets.Count; i++)
        {
            GameObject target = frozenTargets[i];

            if (target == null)
            {
                continue;
            }

            // shark ise
            enemy e = target.GetComponent<enemy>();
            if (e != null)
            {
                e.enabled = true;
            }

            // worm ise
            worm_enemy w = target.GetComponent<worm_enemy>();
            if (w != null)
            {
                w.enabled = true;
            }

            state st = target.GetComponent<state>();
            if (st != null)
            {
                st.isFrozen = false;
            }

            // buz görselini sil
            if (iceInstances.TryGetValue(target, out GameObject iceInstance))
            {
                if (iceInstance != null)
                {
                    Destroy(iceInstance);
                }
            }
        }

        frozenTargets.Clear();
        iceInstances.Clear();

        // İstersen powerup objesini de yok et:
        // Destroy(gameObject);
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll == null)
        {
            return;
        }

        GameObject target = coll.gameObject;

        // Sadece shark/worm ile ilgilen
        bool isShark = coll.CompareTag("shark");
        bool isWorm = coll.CompareTag("worm");

        if (!isShark && !isWorm)
        {
            return;
        }
        else    
            Debug.Log("collided");

        // Aynı target'ı iki kere ekleme
        if (!frozenTargets.Contains(target))
        {
            frozenTargets.Add(target);
        }

        // Freeze flag
        state st = target.GetComponent<state>();
        if (st != null)
        {
            st.isFrozen = true;
        }

        // Hareket scriptlerini kapat
        if (isShark)
        {
            enemy e = target.GetComponent<enemy>();
            if (e != null)
            {
                e.enabled = false;
            }
        }
        else if (isWorm)
        {
            worm_enemy w = target.GetComponent<worm_enemy>();
            if (w != null)
            {
                w.enabled = false;
            }
        }

        // Buz objesini spawnla (zaten spawnladıysa tekrar spawnlama)
        if (iceObj != null && !iceInstances.ContainsKey(target))
        {
            GameObject iceInstance = Instantiate(iceObj, target.transform.position, Quaternion.identity);
            iceInstances[target] = iceInstance;
        }
    }
}
