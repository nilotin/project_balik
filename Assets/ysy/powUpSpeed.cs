using UnityEngine;
using System.Collections;

public class powUpSpeed : MonoBehaviour
{

    private Coroutine slowDownRoutine;
    private float speedBefore;
    private float speed;
    private float currentSpeed;
    public Collider col;
    public GameObject spriteObject;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ship"))
        {
            col.enabled = false;
            spriteObject.SetActive(false);
            powerUp(other.gameObject);
        }
    }

    private void powerUp(GameObject ship)
    {
        GameManager.Instance.IsInvincible = true;
        int level = GameManager.Instance.GetOverDriveLevel();
        BoostThenStop();
    }


    public void BoostThenStop()
    {
        if (slowDownRoutine != null)
        {
            StopCoroutine(slowDownRoutine);
        }

        slowDownRoutine = StartCoroutine(BoostAndSlow());
    }

    private IEnumerator BoostAndSlow()
    {
        speedBefore = GameManager.Instance.GetSpeed();

        float startSpeed = speedBefore * 3f;
        float duration = 5f;
        float elapsedTime = 0f;

        currentSpeed = startSpeed;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Linear yavaşlama
            currentSpeed = Mathf.Lerp(startSpeed, 0f, t);
            GameManager.Instance.SetSpeed(currentSpeed);

            yield return null;
        }

        GameManager.Instance.SetSpeed(speedBefore);
        GameManager.Instance.IsInvincible = false; 

    }
}

