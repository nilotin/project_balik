using UnityEngine;
using System.Collections;

public class attack_warning : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Destoyer());
    }
    IEnumerator Destoyer()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
    
}
