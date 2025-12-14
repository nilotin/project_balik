using UnityEngine;
using System.Collections;

public class powUpSpeed : MonoBehaviour
{

    private Coroutine slowDownRoutine;
    private float speedBefore;
    private float speed;
    private float currentSpeed;
    


    public void powerUp()
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

            // Güvenlik: asla eski hızı aşmasın
            currentSpeed = Mathf.Max(currentSpeed, targetSpeed);

            GameManager.Instance.SetSpeed(currentSpeed);

            yield return null;
        }

        // Final garanti
        GameManager.Instance.SetSpeed(speedBefore);
        GameManager.Instance.IsInvincible = false;
    }

}

