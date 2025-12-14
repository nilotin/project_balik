using UnityEngine;
using UnityEngine.InputSystem;

public class MerchantCatBehaviour : MonoBehaviour
{
    [Header("Face Ship")]
    public Transform ship;
    public string shipTag = "ship";
    public bool lockY = true;

    [Header("Interaction UI")]
    public GameObject pressEUI;

    [Header("Market")]
    public GameObject marketCanvas;
    public ShipMovement shipMovement;

    private bool shipInRange = false;
    private bool marketOpen = false;

    void Start()
    {
        if (ship == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag(shipTag);
            if (found != null)
                ship = found.transform;
        }

        if (shipMovement == null && ship != null)
            shipMovement = ship.GetComponent<ShipMovement>();

        if (pressEUI != null)
            pressEUI.SetActive(false);

        if (marketCanvas != null)
            marketCanvas.SetActive(false);
    }

    void LateUpdate()
    {
        if (shipMovement == null && ship != null)
        {
            shipMovement = ship.GetComponent<ShipMovement>();
        }

        FaceShip();

        // 🔥 MARKET AÇIKKEN — KONUMDAN BAĞIMSIZ
        if (marketOpen)
        {
            if (Keyboard.current != null &&
                (Keyboard.current.eKey.wasPressedThisFrame ||
                 Keyboard.current.escapeKey.wasPressedThisFrame))
            {
                CloseMarket();
            }
            return;
        }

        // 🔹 MARKET KAPALIYKEN — SADECE YAKINDAYSA AÇ
        if (shipInRange &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            OpenMarket();
        }
    }

    void FaceShip()
    {
        if (ship == null) return;

        Vector3 target = ship.position;
        if (lockY) target.y = transform.position.y;

        Vector3 dir = target - transform.position;
        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion lookRot = Quaternion.LookRotation(dir);

        // 🔥 PARENT NULL KORUMASI
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


    void OpenMarket()
    {
        if (shipMovement == null)
        {
            Debug.LogError("ShipMovement NULL — market açılmadı");
            return;
        }

        marketOpen = true;

        if (pressEUI != null)
            pressEUI.SetActive(false);

        if (marketCanvas != null)
            marketCanvas.SetActive(true);

        shipMovement.enabled = false;
    }


    public void CloseMarket()
    {
        marketOpen = false;

        if (marketCanvas != null)
            marketCanvas.SetActive(false);

        if (shipMovement != null)
            shipMovement.enabled = true;

        // Yakındaysa E tekrar görünsün
        if (shipInRange && pressEUI != null)
            pressEUI.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(shipTag)) return;

        shipInRange = true;

        if (!marketOpen && pressEUI != null)
            pressEUI.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(shipTag)) return;

        shipInRange = false;

        if (pressEUI != null)
            pressEUI.SetActive(false);
    }
}
