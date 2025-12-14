using UnityEngine;
using System.Collections;

public class powUpSpeed : MonoBehaviour
{
    public float impulseStrength = 20f;
    public float impulseDuration = 0.25f;
    public float invincibleDuration = 0.4f;

    private Coroutine invRoutine;

    public void powerUp()
    {
        ShipMovement move = GameManager.Instance.ship.GetComponent<ShipMovement>();
        if (move != null)
        {
            move.AddImpulse(move.transform.forward, impulseStrength, impulseDuration);
        }

        if (invRoutine != null)
        {
            StopCoroutine(invRoutine);
        }

        invRoutine = StartCoroutine(InvincibleFor(invincibleDuration));
    }

    private IEnumerator InvincibleFor(float seconds)
    {
        GameManager.Instance.IsInvincible = true;
        yield return new WaitForSeconds(seconds);
        GameManager.Instance.IsInvincible = false;
    }
}
