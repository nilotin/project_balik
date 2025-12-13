using UnityEngine;
using System.Collections;

public class icePowerUp : MonoBehaviour
{
    public Animator anim;
    public GameObject iceObj;
    List<GameObject> objs = new List<GameObject>();

    void Start()
    {
        float waitTime = 0f;
        if(GameManager.Instance.freezeLevel == 1)
            waitTime = 3f;
        StartCoroutine(DestroyAfterTime(waitTime));
           
    }

    IEnumerator DestroyAfterTime(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        anim.SetTrigger("finish"); 
    }

    IEnumerator Defrost(List<GameObject> objs)
    {

        yield return new WaitForSeconds(waitTime);

    }

    void OnTriggerEnter(Collider coll)
    {
        objs.add(coll.transform.GameObject);
        if(coll.tag == "shark")
        {
            coll.GetComponent<enemy>().enabled = false;
            coll.GetComponent<state>().isFrozen = true;
            Instatiate(iceObj,coll.transform.position, Quaternion.identity);
        }
        if(coll.tag == "worm")
        {
            coll.GetComponent<worm_enemy>().enabled = false;
            coll.GetComponent<state>().isFrozen = true;
            Instatiate(iceObj,coll.transform.position, Quaternion.identity);
        }
    }


}
