using UnityEngine;

public class wormAttack : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ship")) // Geminizin tag'inin "Ship" olduğunu varsayıyorum
        {
            ShipHealth shipHealth = other.GetComponent<ShipHealth>();
        
            if(GameManager.Instance.IsInvincible == false && GameManager.Instance.IsUntouchable == false)
            {
                
                shipHealth.TakeDamage(1);    

                Vector3 knockDir =
                    transform.position + other.transform.position;

                ShipMovement move = GameManager.Instance.ship.GetComponent<ShipMovement>();
                if (move != null)
                {
                    move.ApplyKnockback(knockDir, GameManager.Instance.knockbackForce, GameManager.Instance.knockbackDuration);
                }

            }
        }
    }
}
