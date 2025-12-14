using UnityEngine;
using UnityEngine.InputSystem;

public class MarketCanvasController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject marketCanvas;
    [SerializeField] private ShipMovement shipMovement;

    private bool isOpen;
    private bool playerInRange;

    void Awake()
    {
        marketCanvas.SetActive(false);
        isOpen = false;
    }

    void Update()
    {
        if (!playerInRange) return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            ToggleMarket();
        }
    }

    private void ToggleMarket()
    {
        isOpen = !isOpen;
        marketCanvas.SetActive(isOpen);

        // 🚢 SHIP HAREKET KONTROLÜ
        if (shipMovement != null)
            shipMovement.enabled = !isOpen;
    }

    public void SetPlayerInRange(bool value)
    {
        playerInRange = value;

        // Kedi alanından çıkınca marketi kapat
        if (!playerInRange && isOpen)
        {
            isOpen = false;
            marketCanvas.SetActive(false);

            if (shipMovement != null)
                shipMovement.enabled = true;
        }
    }

    public bool IsOpen() => isOpen;
}