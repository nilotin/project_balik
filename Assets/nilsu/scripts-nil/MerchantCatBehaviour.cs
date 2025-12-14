using UnityEngine;

public class MerchantCatBehaviour : MonoBehaviour
{
    [Header("Face Ship")]
    public Transform ship;
    public string shipTag = "ship";
    public bool lockY = true;

    [Header("Interaction UI")]
    public GameObject pressEUI;

    private MarketCanvasController marketController;
    private bool shipInRange;

    void Start()
    {
        // Prefab-safe: scene içinden market controller bul
        marketController = FindObjectOfType<MarketCanvasController>();
        if (marketController == null)
        {
            Debug.LogError("MarketCanvasController not found in scene!");
        }

        // Prefab-safe: ship'i tag ile bul
        if (ship == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag(shipTag);
            if (found != null)
                ship = found.transform;
        }

        if (pressEUI != null)
            pressEUI.SetActive(false);
    }

    void LateUpdate()
    {
        FaceShip();
    }

    void FaceShip()
    {
        if (ship == null) return;

        Vector3 target = ship.position;
        if (lockY) target.y = transform.position.y;

        Vector3 dir = target - transform.position;
        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion lookRot = Quaternion.LookRotation(dir);

        if (transform.parent != null)
        {
            transform.localRotation =
                Quaternion.Inverse(transform.parent.rotation) * lookRot;
        }
        else
        {
            transform.rotation = lookRot;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(shipTag)) return;

        if (pressEUI != null)
            pressEUI.SetActive(true);

        if (marketController != null)
            marketController.SetPlayerInRange(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(shipTag)) return;

        if (pressEUI != null)
            pressEUI.SetActive(false);

        if (marketController != null)
            marketController.SetPlayerInRange(false);
    }

}
