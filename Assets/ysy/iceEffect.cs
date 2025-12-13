using UnityEngine;
using System.Collections;

public class iceEffect : MonoBehaviour
{
    public Animator anim;

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
}
