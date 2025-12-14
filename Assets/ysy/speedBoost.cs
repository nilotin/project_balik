using UnityEngine;
using System.Collections;

public class speedBoost : MonoBehaviour
{
    private Coroutine slowDownRoutine;

    private float speedBefore;
    private float speed;
    private float currentSpeed;
    public GameObject sprite;
    public Collider collider;

    void OnTriggerEnter(Collider coll)
    {
        if(coll.transform.tag == "ship")
        {
            sprite.SetActive(false);
            collider.enabled = false;
        }
    }

    public void powerUp()
    {

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
        float targetSpeed = speedBefore;
        float duration = 5f;
        float elapsedTime = 0f;

        // Boostu anında uygula
        GameManager.Instance.SetSpeed(startSpeed);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            currentSpeed = Mathf.Lerp(startSpeed, targetSpeed, t);

            // Güvenlik: asla eski hızın altına düşmesin
            currentSpeed = Mathf.Max(currentSpeed, targetSpeed);

            GameManager.Instance.SetSpeed(currentSpeed);

            yield return null;
        }

        // Final garanti
        GameManager.Instance.SetSpeed(speedBefore);
    }
}
