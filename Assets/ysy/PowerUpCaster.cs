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

    [Header("Spawn (Forward Based)")]
    public float spawnDistance = 2.0f;
    public float spawnYOffset = 0.0f;   // 2.5D düzleminde yukarı kaldırmak istersen
    public bool lockYToShip = true;     // spawnY = ship.fixedY gibi


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
                FireForwardFromShip(iceBombPrefab,"ice");
                return true;

            case "vortex":
                FireForwardFromShip(vortexPrefab,"vortex");
                return true;

            case "lightning":
                GetComponent<lightningPow>().StartCoroutine(GetComponent<lightningPow>().ChainLightning(transform.position));
                return true;
            case "overdrive":
                GetComponent<powUpSpeed>().powerUp();
                return true;


            default:
                Debug.LogWarning("Unknown powerUp: " + powerUp);
                return false;
        }
    }

    void FireForwardFromShip(GameObject prefab, string name)
    {
        if (prefab == null)
        {
            return;
        }

        // 1) Yön: ship'in baktığı yön
        Vector3 dir = transform.forward;
        dir.y = 0f;
        dir.Normalize();

        // 2) Pozisyon: ship’in biraz önü
        Vector3 spawnPos = transform.position + dir * spawnDistance;

        // 2.5D düzleminde sabitle
        if (lockYToShip)
        {
            spawnPos.y = transform.position.y;
        }

        // Ek offset (vortex’i biraz yukarı koymak vs.)
        spawnPos.y = 0;

        if (name == "vortex")
        {
            spawnPos.y += 0.5f;
        }

        // 3) Rotasyon: ileri baksın (2.5D düzgün)
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

        GameObject obj = Instantiate(prefab, spawnPos, rot);

        // 4) Fırlatma
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Unity 2021/2022:
            rb.linearVelocity = dir * shootForce;

            // Unity 6 ise:
            // rb.linearVelocity = dir * shootForce;
        }
    }

}
