using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    public string powerUpName; // ice, vortex, lightning

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("ship"))
        {
            return;
        }


        powUpsOnHand inv = other.GetComponent<powUpsOnHand>();
        SoundManager.Instance?.PlayPowerUpPickup();


        if (inv == null)
        {
            return;
        }

        bool added = inv.AddPowerUp(powerUpName);

        if (added)
        {
            Destroy(gameObject);
        }
    }
}
