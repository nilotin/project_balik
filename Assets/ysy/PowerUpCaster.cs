using UnityEngine;
using UnityEngine.InputSystem;

public class PowerUpCaster : MonoBehaviour
{
    [Header("Refs")]
    public Camera cam;
    public powUpsOnHand inventory;

    [Header("Prefabs")]
    public GameObject iceBombPrefab;
    public GameObject vortexPrefab;
    public GameObject lightningPrefab;

    [Header("Shoot")]
    public float shootForce = 12f;
    public float spawnForward = 1.5f;

    void Awake()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }

        if (inventory == null)
        {
            inventory = GetComponent<powUpsOnHand>();
        }
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            UseSlot1();
        }
    }

    void UseSlot1()
    {
        if (inventory == null)
        {
            return;
        }

        string powerUp = inventory.PeekFirst();
        if (string.IsNullOrEmpty(powerUp))
        {
            return;
        }

        bool casted = Cast(powerUp);

        // Cast başarılıysa envanterden düşür (kaydırma otomatik)
        if (casted)
        {
            inventory.ConsumeFirst();
        }
    }

    bool Cast(string powerUp)
    {
        switch (powerUp)
        {
            case "ice":
                FireFromScreenCenter(iceBombPrefab);
                return true;

            case "vortex":
                FireFromScreenCenter(vortexPrefab);
                return true;

            case "lightning":
                FireFromScreenCenter(lightningPrefab);
                return true;

            default:
                Debug.LogWarning("Unknown powerUp: " + powerUp);
                return false;
        }
    }

    void FireFromScreenCenter(GameObject prefab)
    {
        if (prefab == null || cam == null)
        {
            return;
        }

        Vector3 screenCenter = new Vector3(Screen.width / 2f, (Screen.height / 2f) +5f, 0f);
        Ray ray = cam.ScreenPointToRay(screenCenter);

        Vector3 spawnPos = cam.transform.position + ray.direction * spawnForward;

        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.LookRotation(ray.direction));

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = ray.direction * shootForce;
        }
    }
}
