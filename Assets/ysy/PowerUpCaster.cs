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
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            UseSlot(0);
        }
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            UseSlot(1);
        }
        else if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            UseSlot(2);
        }

        // İstersen numpad de ekleyebilirsin:
        // if (Keyboard.current.numpad1Key.wasPressedThisFrame) UseSlot(0);
    }

    void UseSlot(int slotIndex)
    {
        if (inventory == null)
        {
            return;
        }

        string powerUp = inventory.PeekAt(slotIndex);

        if (string.IsNullOrEmpty(powerUp))
        {
            return;
        }

        bool casted = Cast(powerUp);

        if (casted)
        {
            inventory.ConsumeAt(slotIndex);
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
                GetComponent<lightningPow>().StartCoroutine(GetComponent<lightningPow>().ChainLightning(transform.position));
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

        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Ray ray = cam.ScreenPointToRay(screenCenter);

        Vector3 spawnPos = cam.transform.position + ray.direction * spawnForward;

        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.LookRotation(ray.direction));

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Unity 6 -> linearVelocity, Unity 2021/2022 -> velocity
            rb.linearVelocity = ray.direction * shootForce;
            // rb.linearVelocity = ray.direction * shootForce;
        }
    }
}
